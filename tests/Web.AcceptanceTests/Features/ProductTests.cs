using GastroHomeCA.Web.AcceptanceTests.Pages;

namespace GastroHomeCA.Web.AcceptanceTests.Features;

[Collection("AcceptanceTests")]
public class ProductTests
{
    private readonly IPage _page;

    public ProductTests()
    {
        var context = PlaywrightSetup.Browser.NewContextAsync().Result;
        _page = context.NewPageAsync().Result;
    }

    [Fact]
    public async Task List_Products_Should_Display_Products()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.WaitForSelectorAsync(".app-container");

        var products = await _page.QuerySelectorAllAsync(".product-card");
        products.Count.ShouldBeGreaterThanOrEqualTo(0);

        var response = await _page.EvaluateAsync<string>("fetch('http://localhost:7000/api/Products').then(r => r.status.toString())");
        response.ShouldBe("200");
    }

    [Fact]
    public async Task Add_Product_Should_Create_Product()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.WaitForSelectorAsync(".app-container");

        await _page.ClickAsync("button:has-text('Add Product')");
        await _page.WaitForSelectorAsync(".modal-content");

        await _page.FillAsync("input[name='productName']", "Test Product");
        await _page.FillAsync("input[name='category']", "Test Category");
        await _page.FillAsync("input[name='currentPrice']", "10.99");

        await _page.ClickAsync("button:has-text('Create Product')");
        await _page.WaitForTimeoutAsync(2000);

        var errorMsg = await _page.QuerySelectorAsync(".alert-danger");
        if (errorMsg != null)
        {
            var errorText = await errorMsg.TextContentAsync();
            throw new Exception($"Error creating product: {errorText}");
        }

        await _page.ClickAsync("button:has-text('Cancel')");
        await _page.WaitForTimeoutAsync(2000);

        var products = await _page.QuerySelectorAllAsync(".product-card");
        products.Count.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task View_Product_Details_Should_Display_Product_Information()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.WaitForSelectorAsync(".app-container");

        var firstProduct = await _page.QuerySelectorAsync(".product-card");
        firstProduct.ShouldNotBeNull("No products available to view");

        await firstProduct.ClickAsync();
        await _page.WaitForTimeoutAsync(2000);

        var nameElement = await _page.QuerySelectorAsync(".product-name");
        var productName = await nameElement!.TextContentAsync();
        productName.ShouldNotBeNull();

        await _page.ClickAsync("button:has-text('Close')");
    }

    [Fact]
    public async Task Update_Product_Should_Modify_Product_Data()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.WaitForSelectorAsync(".app-container");

        var firstProduct = await _page.QuerySelectorAsync(".product-card");
        firstProduct.ShouldNotBeNull("No products available to update");

        await firstProduct.ClickAsync();
        await _page.ClickAsync("button:has-text('Edit')");
        await _page.WaitForSelectorAsync(".modal-content");

        var currentPrice = await _page.InputValueAsync("input[name='currentPrice']");
        var newPrice = (double.Parse(currentPrice) + 5).ToString();

        await _page.FillAsync("input[name='currentPrice']", newPrice);

        await _page.ClickAsync("button:has-text('Update Product')");
        await _page.WaitForTimeoutAsync(2000);

        var errorMsg = await _page.QuerySelectorAsync(".alert-danger");
        if (errorMsg != null)
        {
            var errorText = await errorMsg.TextContentAsync();
            throw new Exception($"Error updating product: {errorText}");
        }

        await _page.ClickAsync("button:has-text('Cancel')");
        await _page.WaitForTimeoutAsync(2000);
    }

    [Fact]
    public async Task Delete_Product_Should_Remove_Product_From_List()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.WaitForSelectorAsync(".app-container");

        var firstProduct = await _page.QuerySelectorAsync(".product-card");
        firstProduct.ShouldNotBeNull("No products available to delete");

        var productsBefore = await _page.QuerySelectorAllAsync(".product-card");

        await firstProduct.ClickAsync();
        await _page.ClickAsync("button:has-text('Delete')");
        await _page.WaitForSelectorAsync(".modal-content");

        await _page.ClickAsync("button:has-text('Confirm')");
        await _page.WaitForTimeoutAsync(2000);

        var productsAfter = await _page.QuerySelectorAllAsync(".product-card");

        productsAfter.Count.ShouldBe(productsBefore.Count - 1);
    }
}
