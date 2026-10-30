namespace GastroHomeCA.Application.FunctionalTests.Infrastructure;

public abstract class TestBase : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await TestApp.ResetState();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
