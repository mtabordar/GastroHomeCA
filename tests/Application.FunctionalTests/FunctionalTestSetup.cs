using Microsoft.Extensions.DependencyInjection;

namespace GastroHomeCA.Application.FunctionalTests;

public static class FunctionalTestSetup
{
    internal static IServiceScopeFactory ScopeFactory { get; private set; } = null!;
    internal static DatabaseResetter? DbResetter { get; private set; }

    private static WebApiFactory? _factory;
    private static DistributedApplication? _app;

    static FunctionalTestSetup()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var cancellationToken = cts.Token;

        var builder = DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TestAppHost>(
                args: [],
                configureBuilder: (options, _) =>
                {
                    options.DisableDashboard = true;
                })
            .GetAwaiter().GetResult();

        builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

        _app = builder
            .BuildAsync(cancellationToken)
            .GetAwaiter().GetResult();

        _app
            .StartAsync(cancellationToken)
            .GetAwaiter().GetResult();

        _app.ResourceNotifications.WaitForResourceHealthyAsync(
            Services.Database, cancellationToken)
            .GetAwaiter().GetResult();

        var connectionString = _app.GetConnectionStringAsync(Services.Database)
            .GetAwaiter().GetResult()!;

        _factory = new WebApiFactory(connectionString);
        ScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        DbResetter = DatabaseResetter.CreateAsync(connectionString).GetAwaiter().GetResult();
    }

    public static async Task CleanupAsync()
    {
        if (DbResetter is not null) await DbResetter.DisposeAsync();
        if (_app is not null) await _app.DisposeAsync();
        if (_factory is not null) await _factory.DisposeAsync();
    }
}
