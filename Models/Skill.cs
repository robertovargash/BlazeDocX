using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public class Skill
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required field")]
        public string Tag { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public string Description { get; set; } = null!;
    }
}
