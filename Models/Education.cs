using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public class Education
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required field")]
        public string Institute { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public string InstituteCountry { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public string Degree { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public DateTime? StartDate { get; set; }
        [RequiredIf("IsCurrent", false, ErrorMessage = "EndDate field is required or just select CURRENT")]
        public DateTime? EndDate { get; set; } = DateTime.MinValue;
        public bool IsCurrent { get; set; }
    }
}
