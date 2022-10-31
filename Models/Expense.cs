using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazeDocX.Models
{
    public enum ExpenseCategory
    {
        Entertaiment, 
        Education, 
        Purchase, 
        Personal, 
        Health, 
        Investment, 
        Kids, 
        Invoices, 
        Taxes, 
        Services, 
        Food, 
        Travel, 
        Home, 
        Electricity, 
        Communication,
        Other
    }

    public class Expense : AccountModel
    {
        [Required(ErrorMessage = "Required field")]
        public ExpenseCategory Category { get; set; }
    }
}
