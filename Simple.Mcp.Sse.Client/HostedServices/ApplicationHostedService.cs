using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Simple.Mcp.Http.Client.HostedServices;

public class ApplicationHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ApplicationHostedService> _logger;

    public ApplicationHostedService(
        IServiceProvider serviceProvider,
        ILogger<ApplicationHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            Services.IApplicationService applicationService = scope.ServiceProvider.GetRequiredService<Services.IApplicationService>();

            await applicationService.RunAsync(stoppingToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in ApplicationHostedService");
            throw;
        }
    }
}