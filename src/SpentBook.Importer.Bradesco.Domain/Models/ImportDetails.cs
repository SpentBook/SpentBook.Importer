using System;
using System.Collections.Generic;
using System.Text;

namespace SpentBook.Importer.Bradesco.Domain.Models
{
    public class ImportDetails
    {
        public Guid IdImport { get; set; }
        public long MappingTime { get; set; }
        public long SavingIntoRepositoryTime { get; set; }
        public int Total { get; set; }
        public int TotalWithoutDuplicates { get; set; }
        public string Status { get; set; }
        public DateTime FinishTime { get; set; }
        public string Message { get; set; }
        public DateTime StartTime { get; set; }
        public object FileName { get; set; }
        public string User { get; set; }
    }
}
