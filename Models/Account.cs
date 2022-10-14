using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public enum Currency
    {
        USD,CAD, GBP, MXN, EUR,JPY, AUD, CHF, DKK, NOK, SEK
    }
    public class Account
    {
        [Required(ErrorMessage = "Required field")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Required field")]
        public decimal Initial { get; set; }
        [Required(ErrorMessage = "Required field")]
        public Currency Currency { get; set; }

    }
}
