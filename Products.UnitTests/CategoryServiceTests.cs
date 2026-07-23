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
    public async Task GetByIdAsync_ShouldReturnCategory_WhenCategoryExists()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Electronics" };

        var repository = new Mock<ICategoryRepository>();
        repository
            .Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var service = new CategoryService(repository.Object);

        var result = await service.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenNameIsEmpty()
    {
        var repository = new Mock<ICategoryRepository>();
        var service = new CategoryService(repository.Object);

        var act = async () => await service.CreateAsync(new CreateCategoryRequest(""));

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Name is required*");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedCategory_WhenCategoryExists()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Electronics" };

        var repository = new Mock<ICategoryRepository>();
        repository
            .Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        repository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new CategoryService(repository.Object);

        var result = await service.UpdateAsync(id, new UpdateCategoryRequest("Books"));

        result.Should().NotBeNull();
        result!.Name.Should().Be("Books");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        var repository = new Mock<ICategoryRepository>();
        repository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var service = new CategoryService(repository.Object);

        var result = await service.UpdateAsync(Guid.NewGuid(), new UpdateCategoryRequest("Books"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenCategoryExists()
    {
        var id = Guid.NewGuid();

        var repository = new Mock<ICategoryRepository>();
        repository
            .Setup(repo => repo.ExistsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        repository
            .Setup(repo => repo.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new CategoryService(repository.Object);

        var result = await service.DeleteAsync(id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
    {
        var repository = new Mock<ICategoryRepository>();
        repository
            .Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new CategoryService(repository.Object);

        var result = await service.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }
}
