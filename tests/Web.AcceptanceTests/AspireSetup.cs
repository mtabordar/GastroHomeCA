using Aspire.Hosting;

namespace GastroHomeCA.Web.AcceptanceTests;

[CollectionDefinition("AcceptanceTests")]
public class AcceptanceTestsCollectionDefinition
{
}

public class AspireSetup : IDisposable
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    public static IDistributedApplicationTestingBuilder Builder { get; private set; } = null!;
    public static DistributedApplication App { get; private set; } = null!;

    static AspireSetup()
    {
        var cts = new CancellationTokenSource(DefaultTimeout);
        var cancellationToken = cts.Token;

        Builder = DistributedApplicationTestingBuilder
             .CreateAsync<Projects.AppHost>(
                args: [],
                configureBuilder: (options, _) =>
                {
                    options.DisableDashboard = false;
                }).Result;

        Builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

        Builder.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(Builder.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        Builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        App = Builder
            .BuildAsync(cancellationToken)
            .Result;

        App
            .StartAsync(cancellationToken)
            .Wait(cancellationToken);

        Task.WaitAll(
            App.ResourceNotifications.WaitForResourceHealthyAsync(Services.WebApi, cancellationToken),
            App.ResourceNotifications.WaitForResourceHealthyAsync(Services.WebFrontend, cancellationToken));
    }

    public void Dispose()
    {
        App.DisposeAsync().GetAwaiter().GetResult();
    }
}
