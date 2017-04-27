using Cpi.Application.DataModels;
using Cpi.Application.DataModels.Interface;
using Cpi.Application.DataModels.LookUp;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cpi.Compass.Application.BusinessRules
{
    public class CpiRequiredOnInvoiceStatusAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is ISoftDeleteDm && ((ISoftDeleteDm)validationContext.ObjectInstance).Deleted)
            {
                return ValidationResult.Success;
            }

            InvoiceDm model = (InvoiceDm)validationContext.ObjectInstance;

            if (model.StatusId.HasValue && (model.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold))
            {
                if (value == null)
                {
                    return new ValidationResult("Required when for the Status entered.");
                }
                else
                {
                    if (value is List<CallCommodityDm>)
                    {
                        List<CallCommodityDm> callCommodities = (List<CallCommodityDm>)value;
                        if (callCommodities.Where(a => a.Quantity > 0).Count() == 0)
                        {
                            return new ValidationResult("Required for the Status entered.");
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
