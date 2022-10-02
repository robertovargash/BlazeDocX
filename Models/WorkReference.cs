using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public class WorkReference
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required field")]
        public string PersonName { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        [EmailAddress(ErrorMessage ="Email address not valid")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public string Occupation { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public string CompanyName { get; set; } = null!;
    }
}
