namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
    }
}
