using TenmoClient.Data;

namespace TenmoClient
{
    public class UserService : ILoginHandler
    {
        private API_User user = new API_User();

        public void Login(API_User u)
        {
            user = u;
        }

        public void Logout()
        {
            user = new API_User();
        }

        public int GetUserId()
        {
            return user.UserId;
        }

        public bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(user.Token);
        }

        public string GetToken()
        {
            return user?.Token ?? string.Empty;
        }
    }
}
