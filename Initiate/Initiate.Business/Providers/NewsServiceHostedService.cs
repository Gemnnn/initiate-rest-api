using Initiate.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class NewsServiceHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public NewsServiceHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var newsService = scope.ServiceProvider.GetRequiredService<INewsService>();

            newsService.Initialize();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}