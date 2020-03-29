using SpentBook.Importer.Domain.File;
using SpentBook.Importer.Infrastructure.File.Ofx.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using SpentBook.Importer.Domain.Models;
using System.Linq;

namespace SpentBook.Importer.Infrastructure.File.Ofx
{
    public class OfxMapper : IOfxMapper
    {
        public IEnumerable<Transaction> Map(FileInfo fileInfo, Guid idImport)
        {
            var parser = new OFXDocumentParser();
            var ofxDocument = parser.ParseOfxDocument(fileInfo.FullName);

            foreach (var t in ofxDocument.Transactions)
            {
                var transaction = new Transaction()
                {

                    Id = Guid.NewGuid(),
                    IdImport = idImport,
                    BankId = ofxDocument.Account.BankID,
                    AccountAgency = ParseAccount(ofxDocument.Account.AccountID, 0),
                    AccountId = ParseAccount(ofxDocument.Account.AccountID, 1),
                    FitId = t.TransactionID,
                    Date = t.Date,
                    Name = t.Memo.Trim(),
                    Value = t.Amount,
                    CheckNum = t.CheckNum.Trim(),
                    Category = null,
                    SubCategory = null,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                };

                yield return transaction;
            }
        }

        private string ParseAccount(string account, int index)
        {
            return account?.Split("/").ElementAtOrDefault(index)?.Trim();
        }
    }
}
