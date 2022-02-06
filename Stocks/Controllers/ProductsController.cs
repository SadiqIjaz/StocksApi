using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stocks.Services;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class ProductsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IProductsService _productsService;

        public ProductsController(ILogger<ProductsController> logger, IProductsService productsService)
        {
            _logger = logger;
            _productsService = productsService;
        }

        // GET: Products?productName=
        public async Task<IActionResult> Index([FromQuery] string productName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<ProductDTO> products;
            try
            {
                products = await _productsService.GetProductsAsync(productName);
            }
            catch
            {
                _logger.LogWarning("Exception occured using Products Service.");
                products = Array.Empty<ProductDTO>();
            }
            return CreatedAtAction(nameof(Index), products.ToList());
        }

        // GET: api/Products/1
        [HttpGet("{productId}")]
        public async Task<IActionResult> Details(int? productId)
        {
            if (productId == null)
            {
                return BadRequest();
            }

            try
            {
                var product = await _productsService.GetProductAsync(productId.Value);
                if (product == null)
                {
                    return NotFound();
                }
                return CreatedAtAction(nameof(Details), product);
            }
            catch
            {
                _logger.LogWarning("Exception occured using Products Service.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
