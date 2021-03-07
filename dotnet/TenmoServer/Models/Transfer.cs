using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "The FromUser field is required")]
        public UserInfo FromUser { get; set; }

        [Required(ErrorMessage = "The ToUser field is required")]
        public UserInfo ToUser { get; set; }
    }
}
