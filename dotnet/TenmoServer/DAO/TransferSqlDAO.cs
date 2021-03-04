using System;
using System.Data.SqlClient;
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
