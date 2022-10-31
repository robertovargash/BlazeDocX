using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazeDocX.Models
{
    public enum IncomeCategory
    {
        Salary, Sale, Gift, Investment, Repayment, Other
    }

    public class Income : AccountModel
    {
        [Required(ErrorMessage = "Required field")]
        public IncomeCategory Category { get; set; }
    }
}
