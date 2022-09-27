using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public class WorkReference
    {
        public int Id { get; set; }
        public string PersonName { get; set; } = string.Empty;
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

    }
}
