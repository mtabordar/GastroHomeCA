namespace GastroHomeCA.Web.AcceptanceTests.Pages;

public abstract class BasePage
{
    protected static string BaseUrl => AspireSetup.App.GetEndpoint(Services.WebFrontend).ToString().TrimEnd('/');

    public abstract string PagePath { get; }

    protected IPage Page { get; set; } = null!;

    public BasePage(IPage page)
    {
        Page = page;
    }

    public Task GotoAsync() => Page.GotoAsync(PagePath);
}
