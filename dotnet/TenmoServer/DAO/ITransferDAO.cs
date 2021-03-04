using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer CreateTransfer(Transfer transfer);
    }
}
