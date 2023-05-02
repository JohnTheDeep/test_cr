using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Address { get; set; }
        [NotMapped]
        public decimal? Balance { get; set; }
        public Wallet() { }
        public Wallet(string address)
        {
            Address = address;
        }
    }
}
