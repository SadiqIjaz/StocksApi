using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks.Data
{
    public static class StocksDbInitialiser
    {
        public static async Task SeedTestData(StocksContext context)
        {
            if (!context.Locations.Any())
            {
                // Add seed data if locations is empty

                var locations = new List<Location>
                {
                    new Location { LocationId = 1, Name = "Liverpool Warehouse", AddressLine = "10 Finch Meadow Close", PostCode = "L9 6EB" },
                    new Location { LocationId = 2, Name = "Middlesbrough Warehouse", AddressLine = "139 Gresham Road", PostCode = "TS1 4LR" }
                };

                locations.ForEach(n => context.Locations.Add(n));
                await context.SaveChangesAsync();
            }

            if (!context.Stocks.Any())
            {
                // Add seed data if stocks is empty

                var stocks = new List<Stock>
                {
                    new Stock { StockId = 1, LocationId = 1, Name = "Monitors", Description = "Any 21-27inch monitors", IsAvailable = true },
                    new Stock { StockId = 2, LocationId = 2, Name = "Desktop Cases", Description = "Any size tower cases", IsAvailable = false },
                    new Stock { StockId = 3, LocationId = 1, Name = "Accessories", Description = "Any accessories for computers", IsAvailable = true },
                    new Stock { StockId = 4, LocationId = 2, Name = "Graphics Cards", Description = "All graphics cards", IsAvailable = true }
                };

                stocks.ForEach(n => context.Stocks.Add(n));
                await context.SaveChangesAsync();
            }
        }
    }
}
