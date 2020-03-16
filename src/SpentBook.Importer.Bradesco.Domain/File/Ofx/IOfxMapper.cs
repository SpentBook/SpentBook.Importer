using SpentBook.Importer.Bradesco.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpentBook.Importer.Bradesco.Domain.File
{
    public interface IOfxMapper
    {
        IEnumerable<Transaction> Map(FileInfo fileInfo, Guid idImport);
    }
}
