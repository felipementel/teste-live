using Products.Application.Categories;

namespace Products.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var categories = app.MapGroup("/categories");

        categories.MapGet("/", async (ICategoryService service, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetAllAsync(cancellationToken)));

        categories.MapGet("/{id:guid}", async (Guid id, ICategoryService service, CancellationToken cancellationToken) =>
        {
            var category = await service.GetByIdAsync(id, cancellationToken);
            return category is null ? Results.NotFound() : TypedResults.Ok(category);
        });

        categories.MapPost("/", async (CreateCategoryRequest request, ICategoryService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/categories/{created.Id}", created);
        });

        categories.MapPut("/{id:guid}", async (Guid id, UpdateCategoryRequest request, ICategoryService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.UpdateAsync(id, request, cancellationToken);
            return updated is null ? Results.NotFound() : TypedResults.Ok(updated);
        });

        categories.MapDelete("/{id:guid}", async (Guid id, ICategoryService service, CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
