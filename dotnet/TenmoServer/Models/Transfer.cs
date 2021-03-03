namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
    }

    public class API_Transfer
    {
        public int TransferId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
    }
}
