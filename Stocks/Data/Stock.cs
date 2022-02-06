using System.ComponentModel.DataAnnotations;

namespace Stocks.Data
{
    public class Stock
    {
        public Stock()
        {

        }

        public Stock(int locationId, string name, string description, bool isAvailable)
        {
            LocationId = locationId;
            Name = name;
            Description = description;
            IsAvailable = isAvailable;
        }

        [Required]
        public int StockId { get; set; }
        [Required]
        public int LocationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }

        public Location Location { get; set; }
    }
}