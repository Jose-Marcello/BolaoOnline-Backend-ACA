using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UnlikeAttribute : ValidationAttribute, IClientModelValidator
    {

        private string DependentProperty { get; }

        public UnlikeAttribute(string dependentProperty)
        {
            if (string.IsNullOrEmpty(dependentProperty))
            {
                throw new ArgumentNullException(nameof(dependentProperty));
            }
            DependentProperty = dependentProperty;
        }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(DependentProperty);
                var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);
                if (value.Equals(otherPropertyValue))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-unlike", ErrorMessage);

            // Added the following code to account for the scenario where the object is deeper in the model's object hierarchy
            var idAttribute = context.Attributes["id"];
            var lastIndex = idAttribute.LastIndexOf('_');
            var prefix = lastIndex > 0 ? idAttribute.Substring(0, lastIndex + 1) : string.Empty;
            MergeAttribute(context.Attributes, "data-val-unlike-property", $"{prefix}{DependentProperty}");
        }

        private void MergeAttribute(IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return;
            }
            attributes.Add(key, value);
        }
    }
}
