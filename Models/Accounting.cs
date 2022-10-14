namespace BlazeDocX.Models
{
    public class Accounting
    {
        public List<Income> Incomes { get; set; } = new List<Income>();
        public List<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
