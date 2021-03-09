using System;
using System.ComponentModel.DataAnnotations;

namespace TenmoServer.Models
{
    public class API_Transfer
    {
        public int TransferId { get; set; }

        [Required(ErrorMessage = "The FromUser field is required")]
        public UserInfo FromUser { get; set; }

        [Required(ErrorMessage = "The ToUser field is required")]
        public UserInfo ToUser { get; set; }

        // A valid amount consists of at least one number followed by an optional decimal point and two numbers
        [RegularExpression(@"^\d+(\.\d{2})?$")]
        // The sql column is limited to 13 digits with two of them used as decimal places
        [Range(0.01, 99999999999.99)]
        public decimal Amount { get; set; }

        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
    }
}
