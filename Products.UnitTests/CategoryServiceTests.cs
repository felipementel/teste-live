using FluentAssertions;
using Moq;
using Products.Application.Categories;
using Products.Domain;

namespace Products.UnitTests;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistAndMapCategory()
    {
        var repository = new Mock<ICategoryRepository>();
        repository
            .Setup(repo => repo.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category category, CancellationToken _) => category);

        var service = new CategoryService(repository.Object);

        var request = new CreateCategoryRequest("Electronics");
        var result = await service.CreateAsync(request);

        result.Name.Should().Be("Electronics");
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        var repository = new Mock<ICategoryRepository>();
        repository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var service = new CategoryService(repository.Object);

        var result = await service.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenNameIsEmpty()
    {
        var repository = new Mock<ICategoryRepository>();
        var service = new CategoryService(repository.Object);

        var act = async () => await service.CreateAsync(new CreateCategoryRequest(""));

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Name is required*");
    }
}
