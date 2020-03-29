using Microsoft.EntityFrameworkCore;
using SpentBook.Importer.Domain.Models;

namespace SpentBook.Importer.Infrastructure.EntityFramework
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        
        public DatabaseContext(DbContextOptions options) : base (options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(builder =>
            {
                builder.HasKey(f => f.Id);
                builder.Property(f => f.Id).ValueGeneratedNever();
                builder.Property(f => f.BankId).HasMaxLength(10);
                builder.Property(f => f.AccountAgency).HasMaxLength(10);
                builder.Property(f => f.AccountId).HasMaxLength(10);
                builder.HasAlternateKey(c => new { c.BankId, c.AccountAgency, c.AccountId, c.Date, c.Name, c.Value });
            });
        }
    }
}
