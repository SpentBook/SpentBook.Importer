using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpentBook.Importer.Bradesco.Domain.File
{
    /// <summary>
    /// Responsible for managing files and folders of file import processes.
    /// </summary>
    public interface IFileDirectoryUtils
    {
        IEnumerable<FileInfo> GetFileNamesOfErrorFolder(string fileNamePattern);
        IEnumerable<FileInfo> GetFileNamesOfProcessedFolder(string fileNamePattern);
        IEnumerable<FileInfo> GetFileNamesOfWorkingFolder(string fileNamePattern);
        IEnumerable<FileInfo> GetFilesOfSourceFolder(string fileNamePattern);
        void MoveToErrorFolder(FileInfo file);
        void MoveToProcessedFolder(FileInfo fileInfo);
        void MoveToWorkingFolder(IEnumerable<FileInfo> files);
        StreamReader OpenText(FileInfo fileInfo);
        void RemoveFiles(IEnumerable<FileInfo> fileInfos);
        void RemoveWorkingFiles(params FileInfo[] files);
        void UnzipFileToWorkingFolder(FileInfo fileInfo);
        void UnzipToPath(FileInfo fileInfo, PathType to);
    }
}
