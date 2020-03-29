using SpentBook.Importer.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpentBook.Importer.Domain.Repository
{
    public interface ITransactionRepository
    {
        Task BulkInsertOrUpdate(Transaction[] transactions);
    }
}