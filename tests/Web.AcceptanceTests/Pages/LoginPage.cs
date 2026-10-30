namespace GastroHomeCA.Web.AcceptanceTests.Pages;

public class LoginPage(IPage page) : BasePage(page)
{
    public override string PagePath => $"{BaseUrl}/login";

    public Task SetEmail(string email)
        => Page.FillAsync("#email", email);

    public Task SetPassword(string password)
        => Page.FillAsync("#password", password);

    public Task ClickLogin()
        => Page.Locator("button[type='submit']").ClickAsync();

    public Task<string?> LogoutButtonText()
        => Page.Locator("a:has-text('Log out')").TextContentAsync();

    public async Task AssertErrorVisible()
    {
        var errorElement = await Page.QuerySelectorAsync("#login-error");
        errorElement.ShouldNotBeNull();
    }
}
