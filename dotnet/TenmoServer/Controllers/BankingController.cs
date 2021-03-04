using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [ApiController]
    [Authorize]
    public class BankingController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly ITransferDAO transferDAO;

        public BankingController(IAccountDAO accountDAO, ITransferDAO transferDAO)
        {
            this.accountDAO = accountDAO;
            this.transferDAO = transferDAO;
        }

        private int GetUserIdFromToken()
        {
            string userIdStr = User.FindFirst("sub")?.Value;

            return Convert.ToInt32(userIdStr);
        }

        [HttpGet("account")]
        public ActionResult<Account> GetLoggedInAccount()
        {
            int userId = GetUserIdFromToken();

            Account account = accountDAO.GetAccountByUserId(userId);

            return Ok(account);
        }

        [HttpPost("transfers/send")]
        public ActionResult<API_Transfer> CreateTransfer(API_Transfer apiTransfer)
        {
            int currentUserId = GetUserIdFromToken();

            if (apiTransfer.FromUserId != currentUserId)
            {
                return Forbid();
            }

            Account fromAccount = accountDAO.GetAccountByUserId(apiTransfer.FromUserId);
            Account toAccount = accountDAO.GetAccountByUserId(apiTransfer.ToUserId);

            if (apiTransfer.FromUserId == apiTransfer.ToUserId)
            {
                return BadRequest();
            }

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

            if (apiTransfer.Amount <= 0)
            {
                return BadRequest();
            }

            Transfer transfer = new Transfer(apiTransfer)
            {
                FromAccountId = fromAccount.AccountId,
                ToAccountId = toAccount.AccountId
            };

            Transfer newTransfer = transferDAO.SendMoney(transfer);

            API_Transfer returnTransfer = new API_Transfer(newTransfer)
            {
                FromUserId = apiTransfer.FromUserId,
                ToUserId = apiTransfer.ToUserId
            };

            return Created($"/transfers/{returnTransfer.TransferId}", returnTransfer);
        }
    }
}
