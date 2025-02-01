using MySuperDuperPetProject.Models;

namespace MySuperDuperPetProject.Middle
{
    public interface ITransferLogic
    {
        Task<IEnumerable<TransferStatisticResponseModel>?> GetMostPopularTransfer(int count, CancellationToken token = default);
        Task<IEnumerable<TransferResponseModel>?> GetTransfers(DateTimeOffset from, DateTimeOffset to, CancellationToken token = default);
        Task<bool> PostTransfer(int userId, string username, string from, string to, CancellationToken token = default);
    }
}