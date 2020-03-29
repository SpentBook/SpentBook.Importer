using SpentBook.Importer.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpentBook.Importer.Domain.File
{
    public interface IOfxMapper
    {
        IEnumerable<Transaction> Map(FileInfo fileInfo, Guid idImport);
    }
}
