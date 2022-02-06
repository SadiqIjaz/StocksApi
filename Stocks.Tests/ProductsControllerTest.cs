using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Stocks.Data;
using Stocks.Controllers;
using Stocks.Services;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Stocks.Tests
{
    public class ProductsControllerTest
    {
        private ProductDTO[] GetTestProducts() => new ProductDTO[]
        {
            new ProductDTO { ProductId = 1, ProductName="Item 1", ProductStockist="Company1", ProductStockNo = 12, BrandId = 1, CategoryId = 1},
            new ProductDTO { ProductId = 2, ProductName = "Item 2", ProductStockist="Company2", ProductStockNo = 1,  BrandId = 2, CategoryId = 1 },
            new ProductDTO { ProductId = 3, ProductName = "Item 3", ProductStockist = "Company2", ProductStockNo = 41,  BrandId = 1, CategoryId = 2 }
        };

        [Fact]
        public async Task GetIndex_WithInvalidModelState_ShouldBadResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);
            controller.ModelState.AddModelError("Something", "Something");

            // Act
            var result = await controller.Index(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetIndex_WithNullName_ShouldViewServiceEnumerable()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>(MockBehavior.Strict);
            var expected = GetTestProducts();
            mockProducts.Setup(r => r.GetProductsAsync(null))
                        .ReturnsAsync(expected)
                        .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);

            // Act
            var result = await controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(viewResult.Value);
            Assert.Equal(expected.Length, model.Count());
            // FIXME: could assert other result property values here

            mockProducts.Verify(r => r.GetProductsAsync(null), Times.Once);
        }

        [Fact]
        public async Task GetIndex_WithName_ShouldViewServiceEnumerable()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>(MockBehavior.Strict);
            var expected = GetTestProducts();
            mockProducts.Setup(r => r.GetProductsAsync("test name"))
                        .ReturnsAsync(expected)
                        .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);
            // Act
            var result = await controller.Index("test name");

            // Assert
            var viewResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(viewResult.Value);
            Assert.Equal(expected.Length, model.Count());
            // FIXME: could assert other result property values here

            mockProducts.Verify(r => r.GetProductsAsync("test name"), Times.Once);
        }

        [Fact]
        public async Task GetIndex_WhenBadServiceCall_ShouldViewEmptyEnumerable()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>(MockBehavior.Strict);
            mockProducts.Setup(r => r.GetProductsAsync(null))
                        .ThrowsAsync(new Exception())
                        .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);

            // Act
            var result = await controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(viewResult.Value);
            Assert.Empty(model);
            mockProducts.Verify(r => r.GetProductsAsync(null), Times.Once);
        }

        [Fact]
        public async Task GetDetails_WithInvalidModelState_ShouldBadResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);
            controller.ModelState.AddModelError("Something", "Something");

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetDetails_WithNullId_ShouldBadResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetDetails_WhenBadServiceCall_ShouldInternalError()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>(MockBehavior.Strict);
            mockProducts.Setup(r => r.GetProductAsync(3))
                        .ThrowsAsync(new Exception())
                        .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);

            // Act
            var result = await controller.Details(3);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError,
                         statusCodeResult.StatusCode);
            mockProducts.Verify(r => r.GetProductAsync(3), Times.Once);
        }

        [Fact]
        public async Task GetDetails_WithUnknownId_ShouldNotFound()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>(MockBehavior.Strict);
            mockProducts.Setup(r => r.GetProductAsync(1000))
                        .ReturnsAsync((ProductDTO)null)
                        .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);

            // Act
            var result = await controller.Details(1000);

            // Assert
            var statusCodeResult = Assert.IsType<NotFoundResult>(result);
            mockProducts.Verify(r => r.GetProductAsync(1000), Times.Once);
        }

        [Fact]
        public async Task GetDetails_WithId_ShouldViewServiceObject()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsService>(MockBehavior.Strict);
            var expected = GetTestProducts().First();
            mockProducts.Setup(r => r.GetProductAsync(expected.ProductId))
                        .ReturnsAsync(expected)
                        .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                    mockProducts.Object);
            // Act
            var result = await controller.Details(expected.ProductId);

            // Assert
            var viewResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsAssignableFrom<ProductDTO>(viewResult.Value);
            Assert.Equal(expected.ProductId, model.ProductId);
            // FIXME: could assert other result property values here

            mockProducts.Verify(r => r.GetProductAsync(expected.ProductId), Times.Once);
        }
    }
}
