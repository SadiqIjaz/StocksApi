using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Data
{
    public class StocksContext : DbContext
    {
        public StocksContext (DbContextOptions<StocksContext> options) : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }

        public DbSet<Location> Locations { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Stock>(x =>
            {
                x.Property(n => n.StockId).ValueGeneratedNever();
            });
            builder.Entity<Location>(x =>
            {
                x.Property(n => n.LocationId).ValueGeneratedNever();
            });
        }
    }
}
