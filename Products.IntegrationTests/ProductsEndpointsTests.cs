using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Products.Application.Products;

namespace Products.IntegrationTests;

public class ProductsEndpointsTests(ProductsApiFactory factory) : IClassFixture<ProductsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated()
    {
        if (!factory.IsDockerAvailable)
        {
            return;
        }

        var request = new CreateProductRequest("Keyboard", "Mechanical keyboard", 89.99m, 10);

        var response = await _client.PostAsJsonAsync("/products", request);
        var created = await response.Content.ReadFromJsonAsync<ProductResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        created.Should().NotBeNull();
        created!.Name.Should().Be("Keyboard");
        created.Description.Should().Be("Mechanical keyboard");
    }
}
