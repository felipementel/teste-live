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
}
