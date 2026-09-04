namespace GastroHomeCA.Domain.Entities;

using GastroHomeCA.Domain.Common;

/// <summary>
/// Represents a product in the inventory with tracking for price history.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Category { get; private set; } = string.Empty;

    public string? Barcode { get; private set; }

    public decimal CurrentPrice { get; private set; }

    public DateTime CreatedDate { get; private set; }

    public DateTime LastUpdatedDate { get; private set; }

    /// <summary>
    /// Creates a new product with an initial price.
    /// </summary>
    public void Create(string name, string category, string? barcode = null, decimal price = 0m)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category cannot be empty.", nameof(category));

        Name = name;
        Category = category;
        Barcode = barcode?.ToString();
        CurrentPrice = price;
        CreatedDate = DateTime.UtcNow.Date;
        LastUpdatedDate = DateTime.UtcNow.Date;
    }

    /// <summary>
    /// Updates the product's details.
    /// </summary>
    public void Update(string name, string category, string? barcode = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category cannot be empty.", nameof(category));

        Name = name;
        Category = category;
        Barcode = barcode?.ToString();
        LastUpdatedDate = DateTime.UtcNow.Date;
    }

    /// <summary>
    /// Updates the product's price.
    /// </summary>
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0m) throw new ArgumentException("Price cannot be less than or equal to zero.", nameof(newPrice));

        // The actual history recording is done in the Application Layer after saving
        CurrentPrice = newPrice;
        LastUpdatedDate = DateTime.UtcNow.Date;
    }
}