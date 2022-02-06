using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks.Services
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync(string productName);

        Task<ProductDTO> GetProductAsync(int productId);
    }
}
