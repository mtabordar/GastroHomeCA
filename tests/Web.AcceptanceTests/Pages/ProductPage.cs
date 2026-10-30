namespace GastroHomeCA.Web.AcceptanceTests.Pages;

public class ProductPage(IPage page) : BasePage(page)
{
    public override string PagePath => $"{BaseUrl}/products";

    public async Task NavigateToProducts()
    {
        await Page.GotoAsync("/products");
    }

    public async Task ClickAddProductButton()
    {
        await Page.ClickAsync("button:has-text('Add Product')");
    }

    public async Task FillProductForm(string name, string category, string price)
    {
        await Page.FillAsync("input[name='productName']", name);
        await Page.FillAsync("input[name='category']", category);
        await Page.FillAsync("input[name='currentPrice']", price);
    }

    public async Task ClickCreateProductButton()
    {
        await Page.ClickAsync("button:has-text('Create Product')");
    }

    public async Task ClickCancelProductButton()
    {
        await Page.ClickAsync("button:has-text('Cancel')");
    }

    public async Task VerifyProductCreated()
    {
        await Page.WaitForSelectorAsync(".product-card");
    }

    public async Task<string?> VerifyProductNotCreated()
    {
        var errorMsg = await Page.QuerySelectorAsync(".alert-danger");
        if (errorMsg != null)
        {
            var errorText = await errorMsg.TextContentAsync();
            return errorText;
        }
        return null;
    }
}
