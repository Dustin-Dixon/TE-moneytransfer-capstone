using System;
using System.Data.SqlClient;
using System.Transactions;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Transfer SendMoney(Transfer transfer)
        {
            Transfer newTransfer = null;

            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    if (!AddToBalance(transfer.ToAccountId, transfer.Amount))
                    {
                        throw new Exception("Failed to update balance of destination account.");
                    }
                    if (!AddToBalance(transfer.FromAccountId, -transfer.Amount))
                    {
                        throw new Exception("Failed to update balance of source account.");
                    }

                    newTransfer = CreateTransfer(transfer);
                    if (newTransfer == null)
                    {
                        throw new Exception("Failed to create record of transfer.");
                    }

                    transaction.Complete();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return newTransfer;
        }

        private bool AddToBalance(int accountId, decimal amount)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "UPDATE accounts " +
                               "SET balance = balance + @amount " +
                               "WHERE account_id = @accountId;";

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@accountId", accountId);

                int rowsUpdated = command.ExecuteNonQuery();
                return (rowsUpdated == 1);
            }
        }

        private Transfer CreateTransfer(Transfer transfer)
        public Transfer CreateTransfer(Transfer transfer)
        {
            Transfer newTransfer = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO transfers " +
                               "(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                               "VALUES (2, 2, @accountFrom, @accountTo, @amount);" +
                               "SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@accountFrom", transfer.FromAccountId);
                command.Parameters.AddWithValue("@accountTo", transfer.ToAccountId);
                command.Parameters.AddWithValue("@amount", transfer.Amount);

                int newId = Convert.ToInt32(command.ExecuteScalar());
                if (newId != 0)
                {
                    newTransfer = transfer;
                    newTransfer.TransferId = newId;
                }
            }

            return newTransfer;
        }
    }
}
