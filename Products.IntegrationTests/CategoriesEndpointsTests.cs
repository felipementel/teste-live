using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Products.Application.Categories;

namespace Products.IntegrationTests;

public class CategoriesEndpointsTests(ProductsApiFactory factory) : IClassFixture<ProductsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateCategory_ShouldReturnCreated()
    {
        if (!factory.IsDockerAvailable)
        {
            return;
        }

        var request = new CreateCategoryRequest("Electronics");

        var response = await _client.PostAsJsonAsync("/categories", request);
        var created = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        created.Should().NotBeNull();
        created!.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnOk()
    {
        if (!factory.IsDockerAvailable)
        {
            return;
        }

        var response = await _client.GetAsync("/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnCategory_WhenExists()
    {
        if (!factory.IsDockerAvailable)
        {
            return;
        }

        var createResponse = await _client.PostAsJsonAsync("/categories", new CreateCategoryRequest("Books"));
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryResponse>();

        var response = await _client.GetAsync($"/categories/{created!.Id}");
        var fetched = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        fetched!.Name.Should().Be("Books");
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnNotFound_WhenNotExists()
    {
        if (!factory.IsDockerAvailable)
        {
            return;
        }

        var response = await _client.GetAsync($"/categories/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCategory_ShouldReturnOk_WhenExists()
    {
        if (!factory.IsDockerAvailable)
        {
            return;
        }

        var createResponse = await _client.PostAsJsonAsync("/categories", new CreateCategoryRequest("Sports"));
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryResponse>();

        var updateRequest = new UpdateCategoryRequest("Sports & Outdoors");
        var response = await _client.PutAsJsonAsync($"/categories/{created!.Id}", updateRequest);
        var updated = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updated!.Name.Should().Be("Sports & Outdoors");
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNoContent_WhenExists()
    {
        if (!factory.IsDockerAvailable)
        {
            return;
        }

        var createResponse = await _client.PostAsJsonAsync("/categories", new CreateCategoryRequest("Toys"));
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryResponse>();

        var response = await _client.DeleteAsync($"/categories/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
