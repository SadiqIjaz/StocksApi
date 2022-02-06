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
using System.Net.Http;
using System.Net;
using System.Text;
using Moq.Protected;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Stocks.Tests
{
    public class ProductsServiceTest
    {
        private ProductDTO[] GetTestProducts() => new ProductDTO[]
        {
            new ProductDTO { ProductId = 1, ProductName="Item 1", ProductStockist="Company1", ProductStockNo = 12, BrandId = 1, CategoryId = 1},
            new ProductDTO { ProductId = 2, ProductName = "Item 2", ProductStockist="Company2", ProductStockNo = 1,  BrandId = 2, CategoryId = 1 },
            new ProductDTO { ProductId = 3, ProductName = "Item 3", ProductStockist = "Company2", ProductStockNo = 41,  BrandId = 1, CategoryId = 2 }
        };

        private Mock<HttpMessageHandler> CreateHttpMock(HttpStatusCode expectedCode, string expectedJson)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = expectedCode
            };

            if (expectedJson != null)
            {
                response.Content = new StringContent(expectedJson, Encoding.UTF8, "application/json");
            }

            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();

            return mock;
        }

        private IProductsService CreateProductsService(HttpClient client)
        {
            var mockConfiguration = new Mock<IConfiguration>(MockBehavior.Strict);
            mockConfiguration.Setup(c => c["BaseUrls:ProductsService"])
                             .Returns("https://thamco-productapi.azurewebsites.net/");

            return new ProductsService(client, mockConfiguration.Object);
        }

        [Fact]
        public async Task GetProductAsync_WithValid_ShouldOkEntity()
        {
            // Arrange
            var expectedResult = GetTestProducts().First();
            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var expectedUri = new Uri("https://thamco-productapi.azurewebsites.net/api/Products/1");
            var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsService(client);

            // Act
            var result = await service.GetProductAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.ProductId, result.ProductId);
            // FIXME: could assert other result property values
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetProductAsync_WithInvalid_ShouldReturnNull()
        {
            // Arrange
            var expectedUri = new Uri("https://thamco-productapi.azurewebsites.net/api/Products/1000");
            var mock = CreateHttpMock(HttpStatusCode.NotFound, null);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsService(client);

            // Act
            var result = await service.GetProductAsync(1000);

            // Assert
            Assert.Null(result);
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetProductAsync_OnHttpBad_ShouldThrow()
        {
            // Arrange
            var expectedUri = new Uri("https://thamco-productapi.azurewebsites.net/api/Products/1");
            var mock = CreateHttpMock(HttpStatusCode.ServiceUnavailable, null);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsService(client);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                () => service.GetProductAsync(1));
        }

        [Fact]
        public async Task GetProductsAsync_WithNull_ShouldReturnAll()
        {
            // Arrange
            var expectedResult = GetTestProducts();
            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var expectedUri = new Uri("https://thamco-productapi.azurewebsites.net/api/Products");
            var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsService(client);

            // Act
            var result = (await service.GetProductsAsync(null)).ToArray();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
            {
                Assert.Equal(expectedResult[i].ProductId, result[i].ProductId);
                // FIXME: could assert other result property values
            }
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>());
        }

        
        [Fact]
        public async Task GetProductsAsync_WithValid_ShouldReturnList()
        {
            // Arrange
            var expectedResult = GetTestProducts();
            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var expectedUri = new Uri("https://thamco-productapi.azurewebsites.net/api/Products?productName=Item%201");
            var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsService(client);

            // Act
            var result = (await service.GetProductsAsync("Item 1")).ToArray();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
            {
                Assert.Equal(expectedResult[i].ProductName, result[i].ProductName);
                // FIXME: could assert other result property values
            }
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>());
        }
        
        [Fact]
        public async Task GetProductsAsync_WithInvalid_ShouldReturnEmpty()
        {
            // Arrange
            var expectedResult = new ProductDTO[0];
            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var expectedUri = new Uri("https://thamco-productapi.azurewebsites.net/api/Products?productName=AAAAAA");
            var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsService(client);

            // Act
            var result = (await service.GetProductsAsync("AAAAAA")).ToArray();

            // Assert
            Assert.Empty(result);
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>());

        }

        [Fact]
        public async Task GetProductsAsync_OnHttpBad_ShouldThrow()
        {
            // Arrange
            var expectedUri = new Uri("https://thamco-productapi.azurewebsites.net/api/Products");
            var mock = CreateHttpMock(HttpStatusCode.ServiceUnavailable, null);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsService(client);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                () => service.GetProductsAsync(null));
        }
    }
}
