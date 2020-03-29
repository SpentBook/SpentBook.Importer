using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpentBook.Importer.Domain.Models;
using SpentBook.Importer.Domain.Repository;
using System.Linq;

namespace SpentBook.Importer.Infrastructure.EntityFramework
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DatabaseContext databaseContext;

        public TransactionRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task BulkInsertOrUpdate(Transaction[] transactions)
        {
            using (var transaction = databaseContext.Database.BeginTransaction())
            {
                await databaseContext
                    .Transactions
                    .UpsertRange(transactions)
                    //.AllowIdentityMatch()
                    .On(t => new { t.BankId, t.AccountAgency, t.AccountId, t.Date, t.Name, t.Value })
                    .WhenMatched((cDb, cIns) => new Transaction
                    {
                        Category = cIns.Category,
                        SubCategory = cIns.SubCategory,
                        Updated = DateTime.Now
                    }).RunAsync();

                transaction.Commit();
            }
        }
    }
}
