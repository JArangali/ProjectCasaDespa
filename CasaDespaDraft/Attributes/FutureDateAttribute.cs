using System;
using System.ComponentModel.DataAnnotations;

namespace CasaDespaDraft.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Date is required.");
            }

            DateTime dateValue;

            if (!DateTime.TryParse(value.ToString(), out dateValue))
            {
                return new ValidationResult("Invalid date format.");
            }

            if (dateValue <= DateTime.UtcNow.Date)
            {
                return new ValidationResult("Date must be a future date.");
            }

            return ValidationResult.Success;
        }
    }
}