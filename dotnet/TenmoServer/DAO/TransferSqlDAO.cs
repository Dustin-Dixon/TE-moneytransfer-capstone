using System;
using System.Collections.Generic;
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

                string query = "DECLARE @typeId INT = " +
                               "(" +
                               "    SELECT transfer_type_id " +
                               "    FROM transfer_types " +
                               "    WHERE transfer_type_desc = @transferType" +
                               ");" +
                               "DECLARE @statusId INT = " +
                               "(" +
                               "    SELECT transfer_status_id " +
                               "    FROM transfer_statuses " +
                               "    WHERE transfer_status_desc = @transferStatus" +
                               ");" +
                               "INSERT INTO transfers " +
                               "(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                               "VALUES (@typeId, @statusId, @accountFrom, @accountTo, @amount);" +
                               "SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@transferType", transfer.TransferType);
                command.Parameters.AddWithValue("@transferStatus", transfer.TransferStatus);
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

        public List<Transfer> GetTransfers(int accountId)
        {
            List<Transfer> transfers = new List<Transfer>();

            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT transfer_id, tt.transfer_type_desc, ts.transfer_status_desc, " +
                                   "account_from, account_to, amount " +
                                   "FROM transfers " +
                                   "JOIN transfer_types AS tt ON tt.transfer_type_id = transfers.transfer_type_id " +
                                   "JOIN transfer_statuses AS ts ON ts.transfer_status_id = transfers.transfer_status_id " +
                                   "WHERE account_from = @accountId OR account_to = @accountId";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@accountId", accountId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        transfers.Add(GetTransferFromReader(reader));
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return transfers;
        }

        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferType = Convert.ToString(reader["transfer_type_desc"]),
                TransferStatus = Convert.ToString(reader["transfer_status_desc"]),
                FromAccountId = Convert.ToInt32(reader["account_from"]),
                ToAccountId = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

            return t;
        }
    }
}
