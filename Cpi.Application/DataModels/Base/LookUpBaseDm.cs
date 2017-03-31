using Cpi.Application.DataModels.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels.Base
{
    public class LookUpBaseDm : BaseDm
    {
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
