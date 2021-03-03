using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;

        public AccountController(IAccountDAO accountDAO)
        {
            this.accountDAO = accountDAO;
        }

        [HttpGet]
        public ActionResult GetLoggedInAccount()
        {
            string userIdStr = User.FindFirst("sub")?.Value;

            if (userIdStr == null)
            {
                return Unauthorized();
            }

            int userId = Convert.ToInt32(userIdStr);

            Account account = accountDAO.GetAccountByUserId(userId);

            return Ok(account);
        }

    }
}
