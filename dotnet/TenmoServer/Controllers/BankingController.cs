using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Transactions;
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
            // Ensure that the logged in user is the source account of the money
            int currentUserId = GetUserIdFromToken();
            if (apiTransfer.FromUserId != currentUserId)
            {
                // TODO: Better return message
                return Forbid();
            }

            // Ensure that the account isn't sending money to itself
            if (apiTransfer.FromUserId == apiTransfer.ToUserId)
            {
                // TODO: Better return message
                return BadRequest();
            }

            // Get accounts from the account IDs in the transfer
            Account fromAccount = accountDAO.GetAccountByUserId(apiTransfer.FromUserId);
            Account toAccount = accountDAO.GetAccountByUserId(apiTransfer.ToUserId);

            // Check that the account IDs actually correspond to accounts.
            if (fromAccount == null)
            {
                // TODO: Better return message
                return BadRequest();
            }

            if (toAccount == null)
            {
                // TODO: Better return message
                return BadRequest();
            }

            // Cannot send more money than the source account has
            if (fromAccount.Balance < apiTransfer.Amount)
            {
                // TODO: Better return message
                return BadRequest();
            }

            // TODO: Move validation to model
            if (apiTransfer.Amount <= 0)
            {
                return BadRequest();
            }

            // Translate API transfer to dao transfer
            Transfer transfer = new Transfer(apiTransfer)
            {
                FromAccountId = fromAccount.AccountId,
                ToAccountId = toAccount.AccountId,
                TransferType = "Send",
                TransferStatus = "Approved"
            };

            Transfer newTransfer = null;
            using (TransactionScope transaction = new TransactionScope())
            {
                newTransfer = transferDAO.CreateTransfer(transfer);

                fromAccount.Balance -= transfer.Amount;
                toAccount.Balance += transfer.Amount;

                accountDAO.UpdateAccount(fromAccount);
                accountDAO.UpdateAccount(toAccount);

                transaction.Complete();
            }

            // Translate the created transfer back into API representation
            API_Transfer returnTransfer = new API_Transfer(newTransfer)
            {
                FromUserId = apiTransfer.FromUserId,
                ToUserId = apiTransfer.ToUserId
            };

            return Created($"/transfers/{returnTransfer.TransferId}", returnTransfer);
        }
    }
}
