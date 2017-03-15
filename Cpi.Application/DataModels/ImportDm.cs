using Cpi.Application.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels
{
    public class ImportDm : BaseDm
    {
        //[CobroMaxLength(200)]
        //[DisplayName("File Name")]
        public string FileName { get; set; }

        public byte[] FileData { get; set; }
    }

    public class ImportMap : BaseMap<ImportDm>
    {
        public ImportMap()
        {
            ToTable("Import");
        }
    }
}
