using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public class ResumeItem
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required field")]
        public string Summary { get; set; } = "";
    }
}
