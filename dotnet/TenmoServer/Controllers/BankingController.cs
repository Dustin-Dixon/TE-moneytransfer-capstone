using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        private readonly IUserDAO userDAO;

        public BankingController(IAccountDAO accountDAO, ITransferDAO transferDAO, IUserDAO userDAO)
        {
            this.accountDAO = accountDAO;
            this.transferDAO = transferDAO;
            this.userDAO = userDAO;
        }

        private int GetUserIdFromToken()
        {
            string userIdStr = User.FindFirst("sub")?.Value;

            return Convert.ToInt32(userIdStr);
        }

        // Get account info of current logged in user.
        [HttpGet("account")]
        public ActionResult<Account> GetLoggedInAccount()
        {
            int userId = GetUserIdFromToken();

            // User is already authenticated so they must have an account.
            // This cannot return null.
            Account account = accountDAO.GetAccountByUserId(userId);

            return Ok(account);
        }

        [HttpPost("transfers/send")]
        public ActionResult<API_Transfer> CreateTransfer(API_Transfer apiTransfer)
        {
            // Ensure that the logged in user is the source account of the money
            int currentUserId = GetUserIdFromToken();
            if (apiTransfer.FromUser.UserId != currentUserId)
            {
                // Cannot return a "forbid" message unless we use a bare 'StatusResult' which would bypass
                // asp.net authentication handling functionality
                return Forbid();
            }

            GetAccountsFromTransfer(apiTransfer, out Account fromAccount, out Account toAccount);

            ActionResult validationResult = ValidateAccounts(fromAccount, toAccount);
            if (validationResult != null)
            {
                return validationResult;
            }

            // Cannot send more money than the source account has
            if (fromAccount.Balance < apiTransfer.Amount)
            {
                // TODO: Better return message
                return BadRequest("You cannot send more money than you currently have");
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
            API_Transfer returnTransfer = ConvertTransferToApiTransfer(newTransfer);

            return Created($"/transfers/{returnTransfer.TransferId}", returnTransfer);
        }

        [HttpPost("transfers/request")]
        public ActionResult<API_Transfer> RequestTransfer(API_Transfer apiTransfer)
        {
            // Ensure that the logged in user is the destination account of the money
            int currentUserId = GetUserIdFromToken();
            if (apiTransfer.ToUser.UserId != currentUserId)
            {
                // TODO: Better return message
                return Forbid();
            }

            GetAccountsFromTransfer(apiTransfer, out Account fromAccount, out Account toAccount);

            ActionResult validationResult = ValidateAccounts(fromAccount, toAccount);
            if (validationResult != null)
            {
                return validationResult;
            }

            // Translate API transfer to dao transfer
            Transfer transfer = new Transfer(apiTransfer)
            {
                FromAccountId = fromAccount.AccountId,
                ToAccountId = toAccount.AccountId,
                TransferType = "Request",
                TransferStatus = "Pending"
            };

            Transfer newTransfer = transferDAO.CreateTransfer(transfer);

            // Translate the created transfer back into API representation
            API_Transfer returnTransfer = ConvertTransferToApiTransfer(newTransfer);

            return Created($"/transfers/{returnTransfer.TransferId}", returnTransfer);
        }

        private void GetAccountsFromTransfer(API_Transfer apiTransfer, out Account fromAccount, out Account toAccount)
        {
            // Get accounts from the account IDs in the transfer
            fromAccount = accountDAO.GetAccountByUserId(apiTransfer.FromUser.UserId.Value);
            toAccount = accountDAO.GetAccountByUserId(apiTransfer.ToUser.UserId.Value);
        }

        private ActionResult ValidateAccounts(Account fromAccount, Account toAccount)
        {
            // Check that the account IDs actually correspond to accounts.
            if (fromAccount == null)
            {
                return NotFound("The specified UserId does not correspond to an account");
            }

            if (toAccount == null)
            {
                return NotFound("The specified UserId does not correspond to an account");
            }

            // Ensure that the account isn't sending money to itself
            if (fromAccount.AccountId == toAccount.AccountId)
            {
                return BadRequest("Cannot create transfer where source and destination account are the same");
            }

            return null;
        }

        [HttpGet("transfers")]
        public ActionResult<List<API_Transfer>> ViewTransfers()
        {
            int userId = GetUserIdFromToken();

            Account account = accountDAO.GetAccountByUserId(userId);
            List<Transfer> transfers = transferDAO.GetTransfersByAccountId(account.AccountId);

            //convert List to API_Transfers
            List<API_Transfer> apiTransfers = new List<API_Transfer>();
            foreach (Transfer transfer in transfers)
            {
                apiTransfers.Add(ConvertTransferToApiTransfer(transfer));
            }

            return Ok(apiTransfers);
        }

        [HttpGet("transfers/pending")]
        public ActionResult<List<API_Transfer>> PendingTransfers()
        {
            int userId = GetUserIdFromToken();

            Account account = accountDAO.GetAccountByUserId(userId);
            List<Transfer> transfers = transferDAO.GetTransfersByAccountId(account.AccountId);

            //convert List to API_Transfers
            List<API_Transfer> apiTransfers = new List<API_Transfer>();
            foreach (Transfer transfer in transfers)
            {
                // TODO: do filtering on dao side
                if (transfer.TransferStatus == "Pending" && account.AccountId == transfer.FromAccountId)
                {
                    apiTransfers.Add(ConvertTransferToApiTransfer(transfer));
                }
            }

            return Ok(apiTransfers);
        }

        [HttpGet("transfers/{id}")]
        public ActionResult<API_Transfer> GetTransferById(int id)
        {
            Transfer transfer = transferDAO.GetTransferById(id);

            if (transfer == null)
            {
                return NotFound("Could not find transfer with the provided id");
            }

            API_Transfer apiTransfer = ConvertTransferToApiTransfer(transfer);

            int userId = GetUserIdFromToken();
            if (apiTransfer.FromUser.UserId != userId && apiTransfer.ToUser.UserId != userId)
            {
                return Forbid();
            }

            return Ok(apiTransfer);
        }

        private API_Transfer ConvertTransferToApiTransfer(Transfer transfer)
        {
            User fromUser = userDAO.GetUserByAccountId(transfer.FromAccountId);
            User toUser = userDAO.GetUserByAccountId(transfer.ToAccountId);

            API_Transfer apiTransfer = new API_Transfer(transfer)
            {
                FromUser = new UserInfo(fromUser),
                ToUser = new UserInfo(toUser)
            };

            return apiTransfer;
        }
    }
}
