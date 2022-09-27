namespace BlazeDocX.Models
{
    public class Education
    {
        public int Id { get; set; }
        public string Institute { get; set; } = string.Empty;
        public string InstituteCountry { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } = DateTime.MinValue;
        public bool IsCurrent { get; set; }
    }
}
