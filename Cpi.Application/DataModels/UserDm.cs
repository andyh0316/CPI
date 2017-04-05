using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cpi.Application.DataModels
{
    public class UserDm : BaseDm
    {
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int? UserRoleId { get; set; }
        public virtual LookUpUserRoleDm UserRole { get; set; }

        public int? UserOccupationId { get; set; }
        public virtual LookUpUserOccupationDm UserOccupation { get; set; }
    }

    public class UserMap : BaseMap<UserDm>
    {
        public UserMap()
        {
            ToTable("User");
            HasRequired(a => a.UserRole).WithMany().HasForeignKey(a => a.UserRoleId).WillCascadeOnDelete(false);
            HasRequired(a => a.UserOccupation).WithMany().HasForeignKey(a => a.UserOccupationId).WillCascadeOnDelete(false);
        }
    }
}
