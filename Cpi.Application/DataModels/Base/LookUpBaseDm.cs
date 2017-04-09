using Cpi.Compass.Application.BusinessRules;
using System.ComponentModel.DataAnnotations;

namespace Cpi.Application.DataModels.Base
{
    public class LookUpBaseDm : BaseDm
    {
        [CpiRequired]
        [CpiMaxLength(500)]
        public string Name { get; set; }

        [CpiRequired]
        public int? DisplayOrder { get; set; }
    }
}
