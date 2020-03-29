using System;

namespace SpentBook.Importer.Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid IdImport { get; set; }
        public string BankId { get; set; }
        public string AccountAgency { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string CheckNum { get; set; }
        public string FitId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
