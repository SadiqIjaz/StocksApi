using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWeb.Models
{
    public class StockDTO
    {
        public StockDTO()
        {

        }

        public StockDTO(int locationId, string name, string description, bool isAvailable)
        {
            LocationId = locationId;
            Name = name;
            Description = description;
            IsAvailable = isAvailable;
        }

        public int StockId { get; set; }

        public int LocationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }

    }
}
