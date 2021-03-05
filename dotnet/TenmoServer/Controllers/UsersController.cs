using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserDAO userDAO;

        public UsersController(IUserDAO userDAO)
        {
            this.userDAO = userDAO;
        }

        [HttpGet]
        public ActionResult<List<UserInfo>> ListUsers()
        {
            List<UserInfo> output = new List<UserInfo>();

            List<User> users = userDAO.GetUsers();
            foreach (User user in users)
            {
                output.Add(new UserInfo()
                {
                    UserId = user.UserId,
                    Username = user.Username
                });
            }

            return Ok(output);
        }
    }
}
