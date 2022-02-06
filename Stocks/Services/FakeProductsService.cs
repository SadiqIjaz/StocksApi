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
            new ProductDTO { ProductId = 1, ProductName="Item1", ProductStockist="Company1", ProductStockNo = 12, BrandId = 1, CategoryId = 1},
            new ProductDTO { ProductId = 2, ProductName = "Item 2", ProductStockist="Company2", ProductStockNo = 1,  BrandId = 2, CategoryId = 1 },
            new ProductDTO { ProductId = 3, ProductName = "Item 3", ProductStockist = "Company2", ProductStockNo = 41,  BrandId = 1, CategoryId = 2 }
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
