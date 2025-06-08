using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Web.Extensions
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        // Set the name of the property to compare
        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        // Validate the date comparison
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;

            var comparisonValue = (DateTime)validationContext.ObjectType.GetProperty(_comparisonProperty)
                                                                        .GetValue(validationContext.ObjectInstance);

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (currentValue < comparisonValue)
            {
                return new ValidationResult(ErrorMessage = "A Data Final tem que ser maior que a Data Inicial");
            }

            return ValidationResult.Success;
        }
    }
}
