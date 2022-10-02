using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public class CV
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required field")]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Required field")]
        public string LastName { get; set; } = "";
        [Required(ErrorMessage = "Required field")]
        [EmailAddress(ErrorMessage = "Email address not valid")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Required field")]
        public string Occupation { get; set; } = "";
        public string FullName => $"{FirstName.Trim()} {LastName.Trim()}".Trim();
        public List<ResumeItem> ResumeSummary { get; set; } = new List<ResumeItem>();
        public List<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<Education> Educations { get; set; } = new List<Education>();
        public List<WorkReference> References { get; set; } = new List<WorkReference>();

    }
}
