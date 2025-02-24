using Microsoft.EntityFrameworkCore;
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
                    using IServiceScope scope = _serviceProvider.CreateScope();
                    await using TransferDbContext db = scope.ServiceProvider.GetRequiredService<TransferDbContext>();
                    DateTimeOffset thresholdDate = DateTimeOffset.UtcNow - _retentionPeriod;
                    IQueryable<Transfers> oldTransfers = db.Transfers.Where(t => t.TransferUTC < thresholdDate);

                    List<Transfers> oldTransfersList = await oldTransfers.ToListAsync(stoppingToken);
                    if (oldTransfersList.Count != 0)
                    {
                        db.Transfers.RemoveRange(oldTransfersList);
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Старые переходы удалены: {Count}", oldTransfersList.Count);
                    }
                    else
                    {
                        _logger.LogInformation("Нет старых переходов для удаления.");
                    }
                   

                   
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
