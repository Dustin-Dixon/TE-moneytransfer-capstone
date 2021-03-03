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

        private int GetUserIdFromToken()
        {
            string userIdStr = User.FindFirst("sub")?.Value;

            return Convert.ToInt32(userIdStr);
        }

        [HttpGet]
        public ActionResult<Account> GetLoggedInAccount()
        {
            int userId = GetUserIdFromToken();

            Account account = accountDAO.GetAccountByUserId(userId);

            return Ok(account);
        }

        [HttpPost("transfers")]
        public ActionResult<API_Transfer> CreateTransfer(API_Transfer apiTransfer)
        {
            int currentUserId = GetUserIdFromToken();

            if (apiTransfer.FromUserId != currentUserId)
            {
                return Forbid();
            }

            Account fromAccount = accountDAO.GetAccountByUserId(apiTransfer.FromUserId);
            Account toAccount = accountDAO.GetAccountByUserId(apiTransfer.ToUserId);

            if (fromAccount == null)
            {
                return BadRequest();
            }

            if (toAccount == null)
            {
                return BadRequest();
            }

            if (fromAccount.Balance < apiTransfer.Amount)
            {
                return BadRequest();
            }

            Transfer transfer = new Transfer
            {
                FromAccountId = fromAccount.AccountId,
                ToAccountId = toAccount.AccountId,
                Amount = apiTransfer.Amount,
                TransferType = apiTransfer.TransferType,
                TransferStatus = apiTransfer.TransferStatus
            };

            return Ok();
        }
    }
}
