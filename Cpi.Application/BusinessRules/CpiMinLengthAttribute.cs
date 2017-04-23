using Cpi.Application.DataModels.Interface;
using System.ComponentModel.DataAnnotations;
namespace Cpi.Compass.Application.BusinessRules
{
    public class CpiMinLengthAttribute : MinLengthAttribute
    {
        public int Length;
        public CpiMinLengthAttribute(int length)
            : base(length)
        {
            Length = length;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is ISoftDeleteDm && ((ISoftDeleteDm)validationContext.ObjectInstance).Deleted)
            {
                return ValidationResult.Success;
            }

            if (base.IsValid(value, validationContext) != ValidationResult.Success)
            {
                return new ValidationResult(string.Format("Must be at least {1} characters.", Length));
            }

            return ValidationResult.Success;
        }
    }
}
