using Cpi.Application.DataModels.Interface;
using System.ComponentModel.DataAnnotations;

namespace Cobro.Compass.Application.BusinessRules
{
    public class CpiStringLengthAttribute : StringLengthAttribute
    {
        public CpiStringLengthAttribute(int maximumLength)
            : base(maximumLength)
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is ISoftDeleteDm && ((ISoftDeleteDm)validationContext.ObjectInstance).Deleted)
            {
                return ValidationResult.Success;
            }

            if (base.IsValid(value, validationContext) != ValidationResult.Success)
            {
                return new ValidationResult(string.Format("{0} must be between {1} and {2} characters.", validationContext.DisplayName, MinimumLength, MaximumLength));
            }

            return ValidationResult.Success;
        }
    }
}
