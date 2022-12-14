using DeckPersonalisationApi.Model;

namespace DeckPersonalisationApi.Services;

public class Startup : BackgroundService
{
    private readonly ILogger<Startup> _logger;
    private IServiceProvider _services;
    private AppConfiguration _conf;

    public Startup(ILogger<Startup> logger, IServiceProvider provider, AppConfiguration conf)
    {
        _logger = logger;
        _services = provider;
        _conf = conf;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _services.CreateScope())
        {
            var ctx = 
                scope.ServiceProvider
                    .GetRequiredService<ApplicationContext>();

            await ctx.Database.EnsureCreatedAsync(stoppingToken);
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Running background service at {DateTime.Now:HH:mm:ss}");
            await Task.Run(RemoveExpiredBlobs);
            await Task.Run(WriteBlobDownloads);
            await Task.Run(UpdateStars);
            await Task.Delay(TimeSpan.FromMinutes(_conf.BackgroundServiceFrequencyMinutes), stoppingToken);
        }
    }

    private void RemoveExpiredBlobs()
    {
        using (var scope = _services.CreateScope())
        {
            var blobService = 
                scope.ServiceProvider
                    .GetRequiredService<BlobService>();

            int count = blobService.RemoveExpiredBlobs();
            _logger.LogInformation($"Deleted {count} unconfirmed blobs");
        }
    }

    private void WriteBlobDownloads()
    {
        using (var scope = _services.CreateScope())
        {
            var blobService = scope.ServiceProvider.GetRequiredService<BlobService>();
            var taskService = scope.ServiceProvider.GetRequiredService<TaskService>();
            blobService.WriteDownloadCache(taskService.RolloverRegisteredDownloads());
            
            // Sneaky extra
            taskService.ClearOldTasks();
        }
    }

    private void UpdateStars()
    {
        using (var scope = _services.CreateScope())
        {
            var cssThemeService = scope.ServiceProvider.GetRequiredService<ThemeService>();
            cssThemeService.UpdateStars();
        }
    }
}