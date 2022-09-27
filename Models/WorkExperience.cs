namespace BlazeDocX.Models
{
    public class WorkExperience
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyCountry { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } = DateTime.MinValue;
        public bool IsCurrent { get; set; }
    }
}
