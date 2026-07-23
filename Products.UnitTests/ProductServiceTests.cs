using FluentAssertions;
using Moq;
using Products.Application.Products;
using Products.Domain;

namespace Products.UnitTests;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistAndMapProduct()
    {
        var repository = new Mock<IProductRepository>();
        repository
            .Setup(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product product, CancellationToken _) => product);

        var service = new ProductService(repository.Object);

        var request = new CreateProductRequest("Laptop", "Gaming laptop", 2999.99m, 5);
        var result = await service.CreateAsync(request);

        result.Name.Should().Be("Laptop");
        result.Description.Should().Be("Gaming laptop");
        result.Price.Should().Be(2999.99m);
        result.Stock.Should().Be(5);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var repository = new Mock<IProductRepository>();
        repository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(repository.Object);

        var result = await service.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}
