using System.ComponentModel.DataAnnotations;

namespace Cpi.Application.DataModels.Base
{
    public class LookUpBaseDm : BaseDm
    {
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }

        [Required]
        public int? DisplayOrder { get; set; }
    }
}
