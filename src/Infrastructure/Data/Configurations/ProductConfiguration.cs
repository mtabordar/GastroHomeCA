namespace GastroHomeCA.Infrastructure.Data.Configurations;

using GastroHomeCA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// EF Core configuration for Product entity.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Define table name
        builder.ToTable("products");
        
        // Map properties
        builder.Property(e => e.Id)
            .HasColumnName("id");
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.Barcode)
            .HasMaxLength(50);
        
        builder.Property(e => e.CurrentPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(e => e.LastUpdatedDate)
            .HasColumnName("last_updated_date");
    }
}