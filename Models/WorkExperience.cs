using System.ComponentModel.DataAnnotations;

namespace BlazeDocX.Models
{
    public class RequiredIfAttribute : RequiredAttribute
    {
        private String PropertyName { get; set; }
        private Object DesiredValue { get; set; }

        public RequiredIfAttribute(String propertyName, Object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            Object instance = context.ObjectInstance;
            Type type = instance.GetType();
            Object proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (proprtyvalue.ToString() == DesiredValue.ToString())
            {
                ValidationResult result = base.IsValid(value, context);
                return result;
            }
            return ValidationResult.Success;
        }
    }
    public class WorkExperience
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required field")]
        public string CompanyName { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public string CompanyCountry { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public string Occupation { get; set; } = null!;
        [Required(ErrorMessage = "Required field")]
        public DateTime? StartDate { get; set; }
        [RequiredIf("IsCurrent", false, ErrorMessage = "EndDate field is required or just select CURRENT")]
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
    }
}
