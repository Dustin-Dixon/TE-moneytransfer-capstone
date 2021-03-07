namespace TenmoClient.Data
{
    public class API_Transfer
    {
        public int TransferId { get; set; }
        public UserInfo FromUser { get; set; }
        public UserInfo ToUser { get; set; }
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }

        public string Message { get; set; }
    }
}
