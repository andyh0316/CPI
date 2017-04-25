using Cpi.Application.DataModels.Interface;
using System.ComponentModel.DataAnnotations;
namespace Cpi.Compass.Application.BusinessRules
{
    public class CpiMaxLengthAttribute : MaxLengthAttribute
    {
        public CpiMaxLengthAttribute(int length)
            : base(length)
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
                return new ValidationResult(string.Format("{0} must not exceed {1} characters.", validationContext.DisplayName, Length));
            }

            return ValidationResult.Success;
        }
    }
}
