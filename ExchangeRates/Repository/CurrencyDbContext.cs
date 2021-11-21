using ExchangeRates.Models;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRates.Repository
{
    public class CurrencyDbContext : DbContext
    {
        public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : base(options)
        {
        }

        public DbSet<CurrencyItem> Items { get; set; }
        public DbSet<PreparedDataItem> PreparedData { get; set; }
        public DbSet<VisitorDataItem> VisitorStatistics { get; set; }
        public DbSet<FingerPrintData> FingerPrintData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VisitorDataItem>()
                .HasMany(p => p.FingerPrints)
                .WithOne(x => x.VisitorDataItem);

            modelBuilder.Entity<FingerPrintData>()
                .HasOne(x => x.VisitorDataItem)
                .WithMany(x => x.FingerPrints);

            base.OnModelCreating(modelBuilder);
        }
    }
}
