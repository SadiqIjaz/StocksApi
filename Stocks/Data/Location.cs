using System.ComponentModel.DataAnnotations;

namespace Stocks.Data
{
    public class Location
    {
        public Location()
        {

        }

        public Location(string name, string addressLine, string postCode)
        {
            Name = name;
            AddressLine = addressLine;
            PostCode = postCode;
        }

        [Required]
        public int LocationId { get; set; }
        public string Name { get; set; }
        public string AddressLine { get; set; }
        public string PostCode { get; set; }
    }
}