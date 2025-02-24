using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySuperDuperPetProject.TransferDatabaseContext;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace MySuperDuperPetProject.Middle
{




    public class OldTransitionsRemover : BackgroundService
    {
        private readonly ILogger<OldTransitionsRemover> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _cleanupInterval;
        private readonly TimeSpan _retentionPeriod;

        public OldTransitionsRemover(ILogger<OldTransitionsRemover> logger, IServiceProvider serviceProvider, IOptions<CleanupOptions> options)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._cleanupInterval = options.Value.CleanupInterval;
            this._retentionPeriod = options.Value.RetentionPeriod;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                    using var scope = _serviceProvider.CreateScope();
                    await using var db = scope.ServiceProvider.GetRequiredService<TransferDbContext>();
                    var thresholdDate = DateTimeOffset.UtcNow - _retentionPeriod;
                    var oldTransfers = db.Transfers.Where(t => t.TransferUTC < thresholdDate);
                    db.Transfers.RemoveRange(oldTransfers);
                    await db.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Старые переходы удалены: {Count}", oldTransfers.Count());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при удалении старых переходов");
                }
            }
        }
        public class CleanupOptions
        {
            public TimeSpan CleanupInterval { get; set; }
            public TimeSpan RetentionPeriod { get; set; }
        }
    }
}
