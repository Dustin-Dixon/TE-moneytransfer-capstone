namespace TenmoServer.Models
{
    public abstract class TransferBase
    {
        public int TransferId { get; set; }
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }

        public TransferBase() { }

        public TransferBase(TransferBase copyFrom)
        {
            TransferId = copyFrom.TransferId;
            Amount = copyFrom.Amount;
            TransferType = copyFrom.TransferType;
            TransferStatus = copyFrom.TransferStatus;
        }
    }

    public class Transfer : TransferBase
    {
        public Transfer() { }

        public Transfer(TransferBase copyFrom) : base(copyFrom) { }

        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
    }

    public class API_Transfer : TransferBase
    {
        public API_Transfer() { }

        public API_Transfer(TransferBase copyFrom) : base(copyFrom) { }

        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
    }
}
