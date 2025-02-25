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
        private readonly ILogger<OldTransitionsRemover> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly TimeSpan cleanupInterval;
        private readonly TimeSpan retentionPeriod;

        public OldTransitionsRemover(ILogger<OldTransitionsRemover> logger, IServiceProvider serviceProvider, IOptions<CleanupOptions> options)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.cleanupInterval = options.Value.CleanupInterval;
            this.retentionPeriod = options.Value.RetentionPeriod;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(cleanupInterval, stoppingToken);
                    using IServiceScope scope = serviceProvider.CreateScope();
                    await using TransferDbContext db = scope.ServiceProvider.GetRequiredService<TransferDbContext>();
                    DateTimeOffset thresholdDate = DateTimeOffset.UtcNow - retentionPeriod;
                    IQueryable<Transfers> oldTransfers = db.Transfers.Where(t => t.TransferUTC < thresholdDate);

                    List<Transfers> oldTransfersList = await oldTransfers.ToListAsync(stoppingToken);
                    if (oldTransfersList.Count != 0)
                    {
                        db.Transfers.RemoveRange(oldTransfersList);
                        await db.SaveChangesAsync(stoppingToken);
                        logger.LogInformation("Старые переходы удалены: {Count}", oldTransfersList.Count);
                    }
                    else
                    {
                        logger.LogInformation("Нет старых переходов для удаления.");
                    }



                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при удалении старых переходов");
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
