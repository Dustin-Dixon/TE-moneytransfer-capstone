namespace TenmoClient.Data
{
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
