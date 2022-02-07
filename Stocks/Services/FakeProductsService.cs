using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks.Services
{
    public class FakeProductsService : IProductsService
    {
        private readonly ProductDTO[] _products =
        {
            new ProductDTO { ProductId = 1, ProductName="1tb SSD", ProductStockist="PcWholesale", ProductStockNo = 13, BrandId = 1, CategoryId = 1},
            new ProductDTO { ProductId = 2, ProductName = "21\" Monitor", ProductStockist="MonitorWarehouse", ProductStockNo = 1,  BrandId = 2, CategoryId = 1 },
            new ProductDTO { ProductId = 3, ProductName = "2tb HDD", ProductStockist = "PcWholesale", ProductStockNo = 41,  BrandId = 1, CategoryId = 2 }
        };

        public Task<ProductDTO> GetProductAsync(int productId)
        {
            var product = _products.FirstOrDefault(r => r.ProductId == productId);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<ProductDTO>> GetProductsAsync(string productName)
        {
            var products = _products.AsEnumerable();
            if (productName != null)
            {
                products = products.Where(r => r.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));
            }
            return Task.FromResult(products);
        }
    }
}
