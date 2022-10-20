using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazeDocX.Models
{
    public enum IncomeCategory
    {
        Salary, Sale, Gift, Investment, Repayment, Other
    }

    public class Income
    {
        [Required(ErrorMessage = "Required field")]
        public string Category { get; set; } = "";
        [Required(ErrorMessage = "Required field")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Required field")]
        public DateTime? Date { get; set; }
        //[Required(ErrorMessage = "Required field")]
        //public Account Account { get; set; } = null!;
        public string Details { get; set; } = string.Empty;
    }
}
