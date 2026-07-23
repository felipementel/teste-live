using Microsoft.EntityFrameworkCore;
using Products.Domain;

namespace Products.Infrastructure;

public sealed class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).HasMaxLength(200).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(1000).IsRequired();
            entity.Property(product => product.Price).HasPrecision(12, 2);
            entity.Property(product => product.Stock).IsRequired();
            entity.Property(product => product.CreatedAtUtc).IsRequired();
            entity.Property(product => product.UpdatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name).HasMaxLength(200).IsRequired();
        });
    }
}
