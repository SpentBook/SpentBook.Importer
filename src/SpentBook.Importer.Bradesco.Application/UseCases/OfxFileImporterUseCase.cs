using Microsoft.Extensions.Logging;
using MoreLinq;
using Newtonsoft.Json;
using SpentBook.Importer.Bradesco.Domain.File;
using SpentBook.Importer.Bradesco.Domain.Models;
using SpentBook.Importer.Bradesco.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static SpentBook.Importer.Bradesco.Domain.Constants;

namespace SpentBook.Importer.Bradesco.Application.UseCases
{
    public class OfxFileImporterUseCase : IOfxFileImporterUseCase
    {
        private readonly IFileAvailabilityChecker fileTracking;
        private readonly ITransactionRepository repository;
        private readonly IFileDirectoryUtils fileDirectoryUtils;
        private readonly IOfxMapper mapper;

        private readonly ILogger<OfxFileImporterUseCase> logger;

        public OfxFileImporterUseCase(
            ILogger<OfxFileImporterUseCase> logger,
            IFileDirectoryUtils fileDirectoryUtils,
            IFileAvailabilityChecker fileTracking,
            ITransactionRepository repository,
            IOfxMapper mapper
        )
        {
            this.logger = logger;
            this.fileDirectoryUtils = fileDirectoryUtils;
            this.fileTracking = fileTracking;
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task Run()
        {
            try
            {
                // Step1: Move all available files to working folder
                this.fileTracking.CheckForAvailableFilesAndMoveToWorking(EXTENSION_OFX);

                foreach (var file in fileDirectoryUtils.GetFileNamesOfWorkingFolder(EXTENSION_OFX))
                {
                    // Step2: Process file
                    var success = await ProcessFile(file);

                    // Step3: Move files to Processed or Error folder
                    MoveFile(file, success);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed trying to get available files for processing.");
            }
        }

        /// <summary>
        /// Execute the processing of a given file.
        /// </summary>
        /// <param name="fileInfo">File path to import</param>
        /// <returns>True if success</returns>
        private async Task<bool> ProcessFile(FileInfo fileInfo)
        {
            var details = new ImportDetails()
            {
                IdImport = Guid.NewGuid(),
                StartTime = DateTime.Now,
                FileName = fileInfo.Name,
                User = "SYSTEM"
            };

            try
            {
                logger.LogInformation($"Start processing the XML file {fileInfo.Name}");

                //Step 1: Map entities
                var entities = MapEntities(fileInfo, details);

                //Step 2: Saving entities
                await SaveEntities(entities, details);

                //Step 3: Log Success
                LogSuccess(fileInfo, details);
                return true;
            }
            catch (Exception e)
            {
                LogError(fileInfo, details, e);
                return false;
            }
        }


        /// <summary>
        /// Move file to a success or error folder
        /// </summary>
        /// <param name="fileInfo">File to move</param>
        /// <param name="success">If true move to Success folder, if false, move to Error folder</param>
        private void MoveFile(FileInfo fileInfo, bool success)
        {
            try
            {
                if (success)
                    fileDirectoryUtils.MoveToProcessedFolder(fileInfo);
                else
                    fileDirectoryUtils.MoveToErrorFolder(fileInfo);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed trying to move file: '{fileInfo}'");
            }
        }

        /// <summary>
        /// Map file to domain entities
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        private Transaction[] MapEntities(FileInfo fileInfo, ImportDetails details)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var items = this.mapper.Map(fileInfo, details.IdImport);
                details.Total = items.Count();

                var itemsWithoutDuplicates = from c in items
                                             group c by new
                                             {
                                                c.BankId,
                                                c.AccountAgency,
                                                c.AccountId,
                                                c.Date,
                                                c.Name,
                                                c.Value
                                             } into grp
                                             select grp.FirstOrDefault();

                details.TotalWithoutDuplicates = itemsWithoutDuplicates.Count();
                return itemsWithoutDuplicates.ToArray();
            }
            finally
            {
                sw.Stop();
                details.MappingTime = sw.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Save entities into database
        /// </summary>
        /// <param name="transactions">Transaction to import into database</param>
        /// <param name="details">Import details to log</param>
        private async Task SaveEntities(Transaction[] transactions, ImportDetails details)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                await this.repository.BulkInsertOrUpdate(transactions);
            }
            finally
            {
                sw.Stop();
                details.SavingIntoRepositoryTime = sw.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Logs error for monitoring
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="details">Instance of <see cref="ImportDetails" /></param>
        /// <param name="exception">Instance of <see cref="Exception"/></param>
        private void LogError(FileInfo fileInfo, ImportDetails details, Exception exception)
        {
            details.Status = nameof(ImportDetailsStatus.Error);
            details.Message = $"Fail to import file from '{fileInfo.Name}' - Message: {exception.Message} >> StackStrace: {exception.StackTrace}";
            details.FinishTime = DateTime.Now;
            logger.LogInformation($"Finishing import file '{fileInfo.Name}'");
            logger.LogError(JsonConvert.SerializeObject(details));
        }

        /// <summary>
        /// Logs information for monitoring
        /// </summary>
        /// <param name="path">Name of the file.</param>
        /// <param name="details">Instance of <see cref="ImportDetails" /></param>
        private void LogSuccess(FileInfo fileInfo, ImportDetails details)
        {
            details.Status = nameof(ImportDetailsStatus.Success);
            details.Message = $"Finishing OFX file '{fileInfo.Name}' processing with success";
            details.FinishTime = DateTime.Now;
            logger.LogInformation($"Finishing import file '{fileInfo.Name}'");
            logger.LogInformation(JsonConvert.SerializeObject(details));
        }
    }
}
