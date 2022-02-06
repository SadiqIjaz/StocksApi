using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Stocks.Services
{
    public class ProductsService : IProductsService
    {
        private readonly HttpClient _client;

        public ProductsService(HttpClient client, IConfiguration config)
        {
            string baseUrl = config["BaseUrls:ProductsService"];
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }

        public async Task<ProductDTO> GetProductAsync(int productId)
        {
            var response = await _client.GetAsync("api/Products/" + productId);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadAsAsync<ProductDTO>();
            return product;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync(string productName)
        {
            var uri = "api/Products";
            if (productName != null)
            {
                uri = uri + "?productName=" + Uri.EscapeDataString(productName);
            }
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<ProductDTO>>();
            return products;
        }
    }   
}
