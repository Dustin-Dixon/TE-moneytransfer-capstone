using System;
using System.Data.SqlClient;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {
        private readonly string connectionString;

        public AccountSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Account GetAccountByUserId(int userId)
        {
            Account output = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT account_id, user_id, balance " +
                                   "FROM accounts " +
                                   "WHERE user_id = @userId;";

                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        output = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return output;
        }

        public bool UpdateAccount(Account account)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "UPDATE accounts " +
                               "SET " +
                               "balance = @balance " +
                               "WHERE account_id = @accountId;";

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@balance", account.Balance);
                command.Parameters.AddWithValue("@account_id", account.AccountId);

                int rowsUpdated = command.ExecuteNonQuery();

                return (rowsUpdated == 1);
            }
        }

        private Account GetAccountFromReader(SqlDataReader reader)
        {
            return new Account()
            {
                AccountId = Convert.ToInt32(reader["account_id"]),
                Balance = Convert.ToDecimal(reader["balance"])
            };
        }
    }
}
