using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        public Account GetAccountByUserId(int userId);

        public bool UpdateAccount(Account account);
    }
}
