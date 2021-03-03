namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
}
