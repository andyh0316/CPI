using Cpi.Application.DataModels.Interface;
using System.ComponentModel.DataAnnotations;
namespace Cpi.Compass.Application.BusinessRules
{
    public class CpiGreaterThanZeroAttribute : ValidationAttribute
    {
        public CpiGreaterThanZeroAttribute()
            : base()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is ISoftDeleteDm && ((ISoftDeleteDm)validationContext.ObjectInstance).Deleted)
            {
                return ValidationResult.Success;
            }

            if ((value is decimal? && ((decimal?)value) <= 0) ||
                (value is int? && ((int?)value <= 0)))
            {
                return new ValidationResult(string.Format("{0} must be greater than 0.", validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
