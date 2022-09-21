using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xceed.Document.NET;

namespace BlazeDocX.Models
{
    public class Profile
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = "";
        public string Resume { get; set; } = "";
        public string? Picture { get; set; }
        public string FullName => $"{FirstName.Trim()} {LastName.Trim()}".Trim();
    }
}