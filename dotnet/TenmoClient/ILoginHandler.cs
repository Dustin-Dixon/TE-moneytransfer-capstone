using TenmoClient.Data;

namespace TenmoClient
{
    public interface ILoginHandler
    {
        void Login(API_User user);

        void Logout();
    }
}
