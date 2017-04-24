using Cpi.Application.DataModels.Interface;
using Cpi.Compass.Application.BusinessRules;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
namespace Cpi.Application.DataModels.Base
{
    public class BaseDm : ISoftDeleteDm
    {
        public int Id { get; set; }

        public int? CreatedById { get; set; }
        [JsonIgnore]
        public virtual UserDm CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        [NotMapped]
        public string CreatedByUsername {
            get {
                return (CreatedBy != null) ? CreatedBy.Nickname : null;
            }
        }

        public int? ModifiedById { get; set; }
        [JsonIgnore]
        public virtual UserDm ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        //public int? ImportId { get; set; }
        //public virtual ImportDm Import { get; set; }

        public bool Deleted { get; set; }
    }

    public class BaseMap<T> : EntityTypeConfiguration<T> where T : BaseDm
    {
        public BaseMap()
        {
            HasKey(m => m.Id);
            Map(m => m.Requires("Deleted").HasValue(false)).Ignore(m => m.Deleted);
            HasOptional(m => m.CreatedBy).WithMany().HasForeignKey(m => m.CreatedById).WillCascadeOnDelete(false);
            HasOptional(m => m.ModifiedBy).WithMany().HasForeignKey(m => m.ModifiedById).WillCascadeOnDelete(false);
        }
    }
}
