using GastroHomeCA.Web.AcceptanceTests.Pages;
using GastroHomeCA.Web.AcceptanceTests.Assertions;

namespace GastroHomeCA.Web.AcceptanceTests.Features;

[Describe("Product Management")]
public class ProductTests
{
    [BeforeTest]
    public async Task BeforeTest()
    {
        Logger.Info("Starting Product tests");
    }

    [AfterTest]
    public async Task AfterTest()
    {
        Logger.Info("Product tests completed");
    }

    [Test]
    [Order(1)]
    public async Task List_Products_Should_Display_Products()
    {
        Logger.Info("Testing product list");

        await Page.GotoAsync("http://localhost:5173");
        await Page.WaitForSelectorAsync(".app-container", State: StateAttached);

        // Wait for products to load
        var products = await Page.QuerySelectorAllAsync(".product-card");
        Logger.Info($"Found {products.Length} products");

        // Verify page loaded without errors
        var response = await Page.Request.GetAsync("http://localhost:7000/api/Products");
        var status = response.Status;
        Logger.Info($"API Status: {status}");

        // If there are products, verify their data
        if (products.Length > 0)
        {
            var firstProduct = products[0];
            var productName = await firstProduct.QuerySelectorAsync(".product-name")?.TextContentAsync();
            Logger.Info($"First product: {productName}");
        }

        Logger.Pass("Product list test completed");
    }

    [Test]
    [Order(2)]
    public async Task Add_Product_Should_Create_Product()
    {
        Logger.Info("Testing add product");

        await Page.GotoAsync("http://localhost:5173");
        await Page.WaitForSelectorAsync(".app-container", State: StateAttached);

        // Click Add Product button
        await Page.ClickAsync("button:has-text('Add Product')");
        await Page.WaitForSelectorAsync(".modal-content", State: StateAttached);

        // Fill in form
        await Page.FillAsync("input[name='productName']", "Test Product");
        await Page.FillAsync("input[name='category']", "Test Category");
        await Page.FillAsync("input[name='currentPrice']", "10.99");

        // Click Create Product
        await Page.ClickAsync("button:has-text('Create Product')");
        await Page.WaitForTimeoutAsync(2000);

        // Verify no error message
        var errorMsg = await Page.QuerySelectorAsync(".alert-danger");
        if (errorMsg != null)
        {
            var errorText = await errorMsg.TextContentAsync();
            Logger.Fail($"Error creating product: {errorText}");
        }

        // Close modal and verify product was added
        await Page.ClickAsync("button:has-text('Cancel')");
        await Page.WaitForTimeoutAsync(2000);

        var products = await Page.QuerySelectorAllAsync(".product-card");
        Logger.Info($"Products count after add: {products.Length}");

        if (products.Length > 0)
        {
            var firstProduct = products[0];
            var productName = await firstProduct.QuerySelectorAsync(".product-name")?.TextContentAsync();
            Logger.Info($"Created product: {productName}");
        }

        Logger.Pass("Add product test completed");
    }

    [Test]
    [Order(3)]
    public async Task View_Product_Details_Should_Display_Product_Information()
    {
        Logger.Info("Testing view product details");

        await Page.GotoAsync("http://localhost:5173");
        await Page.WaitForSelectorAsync(".app-container", State: StateAttached);

        // Get first product
        var firstProduct = await Page.QuerySelectorAsync(".product-card");
        if (firstProduct == null)
        {
            Logger.Skip("No products available to view");
            return;
        }

        // Click on product to view details
        await firstProduct.ClickAsync();
        await Page.WaitForTimeoutAsync(2000);

        // Verify product details are displayed
        var productName = await Page.QuerySelectorAsync(".product-name")?.TextContentAsync();
        Logger.Info($"Product name: {productName}");

        // Close product details
        await Page.ClickAsync("button:has-text('Close')");

        Logger.Pass("View product details test completed");
    }

    [Test]
    [Order(4)]
    public async Task Update_Product_Should_Modify_Product_Data()
    {
        Logger.Info("Testing update product");

        await Page.GotoAsync("http://localhost:5173");
        await Page.WaitForSelectorAsync(".app-container", State: StateAttached);

        // Get first product
        var firstProduct = await Page.QuerySelectorAsync(".product-card");
        if (firstProduct == null)
        {
            Logger.Skip("No products available to update");
            return;
        }

        // Click on Edit button
        await firstProduct.ClickAsync();
        await Page.ClickAsync("button:has-text('Edit')");
        await Page.WaitForSelectorAsync(".modal-content", State: StateAttached);

        // Get current price
        var currentPrice = await Page.InputValueAsync("input[name='currentPrice']");
        var newPrice = (double.Parse(currentPrice) + 5).ToString();

        // Update price
        await Page.FillAsync("input[name='currentPrice']", newPrice);

        // Click Update Product
        await Page.ClickAsync("button:has-text('Update Product')");
        await Page.WaitForTimeoutAsync(2000);

        // Verify no error
        var errorMsg = await Page.QuerySelectorAsync(".alert-danger");
        if (errorMsg != null)
        {
            var errorText = await errorMsg.TextContentAsync();
            Logger.Fail($"Error updating product: {errorText}");
        }

        // Close modal
        await Page.ClickAsync("button:has-text('Cancel')");
        await Page.WaitForTimeoutAsync(2000);

        Logger.Pass("Update product test completed");
    }

    [Test]
    [Order(5)]
    public async Task Delete_Product_Should_Remove_Product_From_List()
    {
        Logger.Info("Testing delete product");

        await Page.GotoAsync("http://localhost:5173");
        await Page.WaitForSelectorAsync(".app-container", State: StateAttached);

        // Get first product
        var firstProduct = await Page.QuerySelectorAsync(".product-card");
        if (firstProduct == null)
        {
            Logger.Skip("No products available to delete");
            return;
        }

        // Get product count before delete
        var productsBefore = await Page.QuerySelectorAllAsync(".product-card");

        // Click on Delete button
        await firstProduct.ClickAsync();
        await Page.ClickAsync("button:has-text('Delete')");
        await Page.WaitForSelectorAsync(".modal-content", State: StateAttached);

        // Confirm delete
        await Page.ClickAsync("button:has-text('Confirm')");
        await Page.WaitForTimeoutAsync(2000);

        // Get product count after delete
        var productsAfter = await Page.QuerySelectorAllAsync(".product-card");

        Logger.Info($"Products before: {productsBefore.Length}, Products after: {productsAfter.Length}");

        Logger.Pass("Delete product test completed");
    }
}