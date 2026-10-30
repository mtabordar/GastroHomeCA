namespace GastroHomeCA.Web.AcceptanceTests.StepDefinitions;

public sealed class LoginStepDefinitions
{
    private readonly LoginPage _loginPage;

    public LoginStepDefinitions()
    {
        var context = PlaywrightSetup.Browser.NewContextAsync().Result;
        var page = context.NewPageAsync().Result;
        _loginPage = new LoginPage(page);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Succeeds()
    {
        await _loginPage.GotoAsync();
        await _loginPage.SetEmail("administrator@localhost");
        await _loginPage.SetPassword("Administrator1!");
        await _loginPage.ClickLogin();

        var logoutButtonText = await _loginPage.LogoutButtonText();

        logoutButtonText.ShouldNotBeNull();
        logoutButtonText.ShouldBe("Log out");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShowsError()
    {
        await _loginPage.GotoAsync();
        await _loginPage.SetEmail("hacker@localhost");
        await _loginPage.SetPassword("l337hax!");
        await _loginPage.ClickLogin();

        await _loginPage.AssertErrorVisible();
    }
}
