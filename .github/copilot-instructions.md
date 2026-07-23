# Copilot instructions for this repository

## Build, test, and validation

This repository targets .NET 10 (`net10.0`) and uses a solution file named `ProductsApi.slnx`.

Common commands:

- Restore dependencies: `dotnet restore ProductsApi.slnx`
- Run the full test suite: `dotnet test ProductsApi.slnx`
- Run only unit tests: `dotnet test Products.UnitTests/Products.UnitTests.csproj`
- Run a single unit test by name: `dotnet test Products.UnitTests/Products.UnitTests.csproj --filter FullyQualifiedName~ProductServiceTests`
- Run integration tests: `dotnet test Products.IntegrationTests/Products.IntegrationTests.csproj`
- Run a single integration test: `dotnet test Products.IntegrationTests/Products.IntegrationTests.csproj --filter FullyQualifiedName~ProductsEndpointsTests`
- Publish the API for container use: `dotnet publish Products.Api/Products.Api.csproj -c Release -o artifacts/publish /p:UseAppHost=false`

There is no separate lint command configured in this repository; the main validation path is `dotnet test` plus `dotnet publish`.

## Architecture at a glance

This is a small ports-and-adapters style .NET web API built around a Product CRUD flow.

- `Products.Api`: ASP.NET Core minimal API host. Endpoints live under `Products.Api/Endpoints` and are registered from `Program.cs`.
- `Products.Application`: Use cases and DTOs. `ProductService` contains the business logic and validation for create/update/delete/read operations.
- `Products.Domain`: Core business model and repository contract. Keep this layer free of EF Core or ASP.NET types.
- `Products.Infrastructure`: EF Core implementation details. `ProductsDbContext` and `EfCoreProductRepository` are the adapters that implement the domain repository contract.

The dependency direction is intentional:
- API depends on Application
- Application depends on Domain
- Infrastructure depends on Application/Domain and implements the repository abstraction

## Code conventions

- Keep the domain model in `Products.Domain` and avoid leaking EF Core or HTTP concerns into it.
- Prefer request/response DTO records in `Products.Application` for API input/output contracts.
- Put validation in the application service layer rather than in the API endpoint layer.
- Use `CancellationToken` parameters in service and repository methods where async work is involved.
- Use `IProductRepository` from the domain layer as the persistence boundary; concrete EF Core implementations belong in Infrastructure.
- The default runtime configuration uses EF Core InMemory for local development. The infrastructure registration reads `Database:Provider` and `ConnectionStrings:Products` from configuration.
- Keep endpoint changes minimal and focused; most behavior should be implemented in the service layer and repository abstraction.

## Testing conventions

- Unit tests live in `Products.UnitTests` and target the application/service layer.
- Integration tests live in `Products.IntegrationTests` and exercise the full API host through `WebApplicationFactory`.
- Integration tests use Testcontainers when Docker is available; they should fail gracefully when Docker is unavailable rather than breaking local validation.

## Container and CI notes

- The Docker image is built from `Dockerfile` and publishes the API into `/app/publish`.
- GitHub Actions are defined in `.github/workflows/ci.yml` and include test execution, Trivy scanning, OCI export, and tag creation from the project `Version` value.
