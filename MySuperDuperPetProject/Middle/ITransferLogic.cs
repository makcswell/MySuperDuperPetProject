using MySuperDuperPetProject.Models;

namespace MySuperDuperPetProject.Middle
{
    public interface ITransferLogic
    {
        Task<IEnumerable<TransferStatisticResponseModel>?> GetMostPopularTransfer(string username,int count, CancellationToken token = default);//добавил username для проверки
        Task<IEnumerable<TransferResponseModel>?> GetTransfers(string username,DateTimeOffset from, DateTimeOffset to, CancellationToken token = default);//добавил username для проверки
        Task<bool> PostTransfer(string username ,string from, string to, CancellationToken token = default);//добавил username для проверки
    }
}