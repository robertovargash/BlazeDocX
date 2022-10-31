using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public abstract class AccountModel
    {
        [Required(ErrorMessage = "Required field")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Required field")]
        public DateTime? Date { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}
