using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySuperDuperPetProject.TransferDatabaseContext;

namespace MySuperDuperPetProject.Middle
{
    public class OldTransitionsRemover(
        ILogger<OldTransitionsRemover> logger,
        IServiceProvider serviceProvider,
        IOptions<OldTransitionsRemover.CleanupOptions> options)
        : BackgroundService
    {
        private readonly TimeSpan cleanupInterval = options.Value.CleanupInterval;
        private readonly TimeSpan retentionPeriod = options.Value.RetentionPeriod;

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