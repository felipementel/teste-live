using Products.Domain;

namespace Products.Application.Products;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return products.Select(MapToResponse).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Description, request.Price, request.Stock);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        var created = await repository.AddAsync(product, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Description, request.Price, request.Stock);

        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = request.Name.Trim();
        existing.Description = request.Description.Trim();
        existing.Price = request.Price;
        existing.Stock = request.Stock;
        existing.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await repository.UpdateAsync(existing, cancellationToken);
        return MapToResponse(existing);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await repository.ExistsAsync(id, cancellationToken))
        {
            return false;
        }

        await repository.DeleteAsync(id, cancellationToken);
        return true;
    }

    private static void Validate(string name, string description, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        }

        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stock), "Stock cannot be negative.");
        }
    }

    private static ProductResponse MapToResponse(Product product) =>
        new(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CreatedAtUtc, product.UpdatedAtUtc);
}
