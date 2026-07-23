using Products.Domain;

namespace Products.Application.Categories;

public sealed class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await repository.GetAllAsync(cancellationToken);
        return categories.Select(MapToResponse).ToList();
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetByIdAsync(id, cancellationToken);
        return category is null ? null : MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name);

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim()
        };

        var created = await repository.AddAsync(category, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name);

        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = request.Name.Trim();

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

    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }
    }

    private static CategoryResponse MapToResponse(Category category) =>
        new(category.Id, category.Name);
}
