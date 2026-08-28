namespace GastroHomeCA.Infrastructure.Product;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GastroHomeCA.Domain.Entities;

/// <summary>
/// EF Core configuration for Product entity.
/// </summary>
public static class ProductConfig
{
    public static void Configure(EntityTypeBuilder<Product> builder)
    {
        // Define table name
        builder.ToTable("products");
        
        // Configure navigation properties and relationships
        // Add indexes if needed
        
        // Map additional properties to columns with specific configurations
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.Barcode)
            .HasMaxLength(20);
            
        builder.Property(e => e.CurrentPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
    }
}