using System.ComponentModel.DataAnnotations;
using TenmoServer.Models;

public class UserInfo
{
    [Required(ErrorMessage = "The UserId field is required")]
    public int? UserId { get; set; }
    public string Username { get; set; }

    public UserInfo() { }

    public UserInfo(User fromUser)
    {
        this.UserId = fromUser.UserId;
        this.Username = fromUser.Username;
    }
}