using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpentBook.Importer.Domain.File;
using SpentBook.Importer.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpentBook.Importer.Infrastructure.File
{
    public class FileAvailabilityChecker : IFileAvailabilityChecker
    {
        public readonly ILogger<FileAvailabilityChecker> logger;
        private readonly IFileDirectoryUtils fileDirectoryUtils;

        public FileAvailabilityChecker(
            ILogger<FileAvailabilityChecker> logger,
            IFileDirectoryUtils fileDirectoryUtils
        )
        {
            this.logger = logger;
            this.fileDirectoryUtils = fileDirectoryUtils;
        }

        public void CheckForAvailableFilesAndMoveToWorking(string extension)
        {
            var filesToCopy = fileDirectoryUtils.GetFilesOfSourceFolder(extension);
            if (filesToCopy.Any())
                fileDirectoryUtils.MoveToWorkingFolder(filesToCopy);
        }

        //public bool Delete(string path)
        //{
        //    try
        //    {
        //        System.IO.File.Delete(path);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, ex.Message);
        //        return false;
        //    }
        //}

        //public static IEnumerable<string> GetFilesByExtension(string directoryPath, string extension, SearchOption searchOption)
        //{
        //    return Directory.EnumerateFiles(directoryPath, "*" + extension, searchOption)
        //            .Where(x => string.Equals(Path.GetExtension(x), "." + extension, StringComparison.InvariantCultureIgnoreCase));
        //}
    }
}
