using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySuperDuperPetProject.Models;
using MySuperDuperPetProject.TransferDatabaseContext;
using System.Text;

namespace MySuperDuperPetProject.Middle
{

    public class TransferLogic(ILogger<TransferLogic> logger, TransferDbContext db) : ITransferLogic
    {
        private static string GetTransferHash(string from, string to)
        {
            StringBuilder sb = new();
            sb.Append(from);
            sb.Append(to);
            return sb.ToString();
        }
       
        public async Task<bool> PostTransfer(string username, string from, string to, CancellationToken token = default)//добавил username для взятия из таблицы
        {
            await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(token);
            try
            {
                User? userEntity = await db.Users.FirstOrDefaultAsync(u => u.Name == username, token);
                if (userEntity == null)
                {
                    logger.LogWarning("User not found: {username}",username);

                    throw new UnauthorizedAccessException("User not authorized.");
                }
                Transfers trans = new()
                {
                    PageFrom = from,
                    PageTo = to,
                    TransferUTC = DateTimeOffset.UtcNow,
                    User = userEntity
                };
                await db.Transfers.AddAsync(trans, token);

                string transferHash = GetTransferHash(from, to);

                TransfersStatistic? stat = await db.TransfersStatistics.FindAsync(transferHash, token);
                if (stat == null)
                {
                    stat = new()
                    {
                        HashId = transferHash,
                        Count = 1,
                        From = from,
                        To = to,
                    };
                    await db.TransfersStatistics.AddAsync(stat, token);
                }
                else
                {
                    stat.Count++;
                }

                await db.SaveChangesAsync(token);
                await transaction.CommitAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(token);
                logger.LogError(ex, "Error on posting transfer!");
            }
            return false;
        }
        public async Task<IEnumerable<TransferResponseModel>?> GetTransfers(string username, DateTimeOffset from, DateTimeOffset to, CancellationToken token = default)//добавил username для взятия из таблицы
        {
            User? userfromdb = await db.Users.FirstOrDefaultAsync(u => u.Name == username, token);

            if (userfromdb == null)//Условие для проверки по username
            {
                logger.LogWarning("Пользователь не найден!");
                return null;
            }
            try
            {
                return await db.Transfers.Where(t => t.TransferUTC >= from && t.TransferUTC <= to).AsNoTracking().Select(t => new TransferResponseModel(t.PageFrom, t.PageTo)).ToListAsync(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on getting transfers by user and period!");
                return null;
            }
        }

        public async Task<IEnumerable<TransferStatisticResponseModel>?> GetMostPopularTransfer(int count, CancellationToken token = default)
        {
            try
            {
                return await db.TransfersStatistics.OrderByDescending(ts => ts.Count).Take(count).AsNoTracking().Select(ts => new TransferStatisticResponseModel(ts.Count, ts.From, ts.To)).ToListAsync(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on getting most popular transfers!");
            }
            return null;
        }

    }
}
