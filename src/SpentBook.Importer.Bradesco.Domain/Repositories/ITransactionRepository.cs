using SpentBook.Importer.Bradesco.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpentBook.Importer.Bradesco.Domain.Repository
{
    public interface ITransactionRepository
    {
        Task BulkInsertOrUpdate(Transaction[] transactions);
    }
}