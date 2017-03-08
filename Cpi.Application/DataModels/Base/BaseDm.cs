using Cpi.Application.DataModels.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels.Base
{
    public class BaseDm : ISoftDeleteDm
    {
        public int Id { get; set; }

        public int? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool Deleted { get; set; }
    }

    public class BaseMap<T> : EntityTypeConfiguration<T> where T : BaseDm
    {
        public BaseMap()
        {
            HasKey(m => m.Id);
            Map(m => m.Requires("Deleted").HasValue(false)).Ignore(m => m.Deleted);
        }
    }
}
