using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Controllers;
using ProductService.ProductRepository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<ProductController>> _mockLogger;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_mockLogger.Object, _mockProductService.Object);
    }

    [Fact]
    public async Task Products_ReturnsProductList_WhenServiceIsSuccessful()
    {
        // Arrange
        var productList = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1" },
            new ProductDto { Id = 2, Name = "Product 2" }
        };
        _mockProductService.Setup(service => service.GetProductsAsync()).ReturnsAsync(productList);

        // Act
        var result = await _controller.Products() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var products = result.Value as List<ProductDto>;
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task Products_ReturnsEmptyList_WhenServiceFails()
    {
        // Arrange
        _mockProductService.Setup(service => service.GetProductsAsync()).ThrowsAsync(new System.Exception());

        // Act
        var result = await _controller.Products() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var products = result.Value as List<ProductDto>;
        Assert.NotNull(products);
        Assert.Empty(products);
    }
}
