using Cpi.Application.DataModels.Interface;
using System.ComponentModel.DataAnnotations;

namespace Cpi.Compass.Application.BusinessRules
{
    public class CpiRequiredAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is ISoftDeleteDm && ((ISoftDeleteDm)validationContext.ObjectInstance).Deleted)
            {
                return ValidationResult.Success;
            }

            if (base.IsValid(value, validationContext) != ValidationResult.Success)
            {
                return new ValidationResult(string.Format("{0} is required.", validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
