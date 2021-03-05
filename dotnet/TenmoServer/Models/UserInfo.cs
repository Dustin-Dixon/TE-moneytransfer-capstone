using TenmoServer.Models;

public class UserInfo
{
    public int UserId { get; set; }
    public string Username { get; set; }

    public UserInfo() { }

    public UserInfo(User fromUser)
    {
        this.UserId = fromUser.UserId;
        this.Username = fromUser.Username;
    }
}