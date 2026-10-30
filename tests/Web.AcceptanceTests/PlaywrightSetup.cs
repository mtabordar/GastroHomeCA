using System.Diagnostics;

namespace GastroHomeCA.Web.AcceptanceTests;

[CollectionDefinition("AcceptanceTests")]
public class AcceptanceTestsCollection : ICollectionFixture<PlaywrightSetup>, ICollectionFixture<AspireSetup>
{
}

public class PlaywrightSetup : IDisposable
{
    private static bool IsHeadless => Debugger.IsAttached is false;
    private static IPlaywright? _playwright;

    public static IBrowser Browser { get; private set; } = null!;

    static PlaywrightSetup()
    {
        _playwright = Playwright.CreateAsync().Result;

        Browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = IsHeadless ? 0 : 500
        }).Result;
    }

    public void Dispose()
    {
        Browser.CloseAsync().Wait();
        _playwright?.Dispose();
    }
}
