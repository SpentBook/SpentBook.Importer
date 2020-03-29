using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpentBook.Importer.Domain.File;
using SpentBook.Importer.Infrastructure.Configuration;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SpentBook.Importer.Infrastructure.File
{
    /// <summary>
    /// Responsible for managing files and folders of file import processes.
    /// </summary>
    public class FileDirectoryUtils : IFileDirectoryUtils
    {
        private readonly AppSettings options;

        public FileDirectoryUtils(IOptions<AppSettings> options)
        {
            this.options = options.Value;
        }

        #region Get files

        private IEnumerable<FileInfo> GetFilesFromPath(PathType type, string searchPattern)
        {
            string path = this.GetPathFromType(type);

            var result = Directory.EnumerateFiles(path, searchPattern).Select(f => new FileInfo(f)).ToList();
            return result.OrderBy(f => f.Name);
        }

        /// <summary>
        /// Returns all files in the source folder according to a file name pattern.
        /// </summary>
        /// <param name="fileNamePattern">File pattern</param>
        /// <returns>List of Files Found</returns>
        public IEnumerable<FileInfo> GetFilesOfSourceFolder(string fileNamePattern)
        {
            return GetFilesFromPath(PathType.Source, fileNamePattern);
        }

        /// <summary>
        /// Returns all files in the working folder according to a file name pattern.
        /// </summary>
        /// <param name="fileNamePattern">File pattern</param>
        /// <returns>List of Files Found</returns>
        public IEnumerable<FileInfo> GetFileNamesOfWorkingFolder(string fileNamePattern)
        {
            var retorno = GetFilesFromPath(PathType.Working, fileNamePattern);
            return retorno;
        }

        /// <summary>
        /// Returns all xml files in the processed folder according to a file name pattern.
        /// </summary>
        /// <param name="fileNamePattern">File pattern</param>
        /// <returns>List of Files Found</returns>
        public IEnumerable<FileInfo> GetFileNamesOfProcessedFolder(string fileNamePattern)
        {
            return GetFilesFromPath(PathType.Processed, fileNamePattern);
        }

        /// <summary>
        /// Returns all xml files in the error folder according to a file name pattern.
        /// </summary>
        /// <param name="fileNamePattern">File pattern</param>
        /// <returns>List of Files Found</returns>
        public IEnumerable<FileInfo> GetFileNamesOfErrorFolder(string fileNamePattern)
        {
            return GetFilesFromPath(PathType.Error, fileNamePattern);
        }

        #endregion

        #region Move files

        private void MoveFromPathTo(IEnumerable<FileInfo> files, PathType to)
        {
            var filesToAndFrom = files
                .Select(f => new
                {
                    From = f.FullName,
                    To = Path.Combine(this.GetPathFromType(to), f.Name)
                });

            foreach (var file in filesToAndFrom)
            {
                if (System.IO.File.Exists(file.To))
                    System.IO.File.Delete(file.To);

                System.IO.File.Move(file.From, file.To);
            }
        }

        /// <summary>
        /// Move a list of files to the error folder
        /// </summary>
        /// <param name="file">File to move to error folder</param>
        public void MoveToErrorFolder(FileInfo file)
        {
            MoveFromPathTo(new[] { file }, PathType.Error);
        }

        /// <summary>
        /// Move a list of files to the processed folder
        /// </summary>
        /// <param name="fileInfo">File to move to processed folder</param>
        public void MoveToProcessedFolder(FileInfo fileInfo)
        {
            MoveFromPathTo(new[] { fileInfo }, PathType.Processed);
        }

        /// <summary>
        /// Move a list of files to the working folder
        /// </summary>
        /// <param name="files">List of files to move to working folder</param>
        public void MoveToWorkingFolder(IEnumerable<FileInfo> files)
        {
            MoveFromPathTo(files, PathType.Working);
        }

        #endregion

        #region Unzip

        public void UnzipToPath(FileInfo fileInfo, PathType to)
        {
            ZipFile.ExtractToDirectory(fileInfo.FullName, this.GetPathFromType(to), true);
        }

        /// <summary>
        /// Unzip specify file into working folder
        /// </summary>
        /// <param name="file">File to unzip</param>
        public void UnzipFileToWorkingFolder(FileInfo fileInfo)
        {
            UnzipToPath(fileInfo, PathType.Working);
        }

        #endregion

        #region Remove files

        public void RemoveFiles(IEnumerable<FileInfo> fileInfos)
        {
            foreach (var file in fileInfos)
            {
                System.IO.File.Delete(file.FullName);
            }
        }

        /// <summary>
        /// Remove specify files in working folder
        /// </summary>
        /// <param name="files">List of files to remove</param>
        public void RemoveWorkingFiles(params FileInfo[] files)
        {
            RemoveFiles(files);
        }

        #endregion

        #region Utils

        public StreamReader OpenText(FileInfo fileInfo)
        {
            return System.IO.File.OpenText(fileInfo.FullName);
        }

        private string GetPathFromType(PathType type)
        {
            string path = null;
            switch (type)
            {
                case PathType.Source:
                    path = options.SourceFolder;
                    break;
                case PathType.Working:
                    path = options.WorkingFolder;
                    break;
                case PathType.Processed:
                    path = options.ProcessedFolder;
                    break;
                case PathType.Error:
                    path = options.ErrorFolder;
                    break;
            }

            return path;
        }

        private string JoinFilesToLog(IEnumerable<FileInfo> files)
        {
            return string.Join(',', files.Select(s => s.Name));
        }

        #endregion
    }
}
