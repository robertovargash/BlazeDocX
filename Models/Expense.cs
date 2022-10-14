using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazeDocX.Models
{
    public enum ExpenseCategory
    {
        Entertaiment,Education, Purchase, Personal, Health, Investment, Kids, Invoices,Taxes,Services,Food,Travel,Home,Electricity, Communication
    }
    public class Expense
    {
        [Required(ErrorMessage = "Required field")]
        public ExpenseCategory Category { get; set; }
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
