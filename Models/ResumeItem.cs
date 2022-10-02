using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public class ResumeItem
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required field")]
        [StringLength(50, ErrorMessage = "Resume length can't be more than 50.")]
        public string Summary { get; set; } = "";
    }
}
