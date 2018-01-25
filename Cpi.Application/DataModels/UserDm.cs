using Cobro.Compass.Application.BusinessRules;
using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using System;
using System.Collections.Generic;

namespace Cpi.Application.DataModels
{
    public class UserDm : BaseDm
    {
        [CpiStringLength(25, MinimumLength = 5, ErrorMessage = "Username must be between {2} and {1} characters long.")]
        public string Username { get; set; }

        [CpiRequired]
        [CpiMaxLength(100)]
        public string Fullname { get; set; }

        [CpiRequired]
        [CpiMaxLength(30)]
        public string Nickname { get; set; }

        [CpiMaxLength(500)]
        public string Password { get; set; }
        [CpiMaxLength(500)]
        public string PasswordSalt { get; set; }
        [CpiMaxLength(20)]
        public string TempPassword { get; set; }

        [CpiRequired]
        public int? UserRoleId { get; set; }
        public virtual LookUpUserRoleDm UserRole { get; set; }

        public int? UserOccupationId { get; set; }
        public virtual LookUpUserOccupationDm UserOccupation { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [CpiRequired]
        public decimal? Salary { get; set; }

        [CpiRequired]
        public DateTime? StartDate { get; set; }

        [CpiRequired]
        public int? VacationDaysTaken { get; set; }
    }

    public class UserMap : BaseMap<UserDm>
    {
        public UserMap()
        {
            ToTable("User");

            HasRequired(a => a.UserRole).WithMany().HasForeignKey(a => a.UserRoleId).WillCascadeOnDelete(false);
            HasOptional(a => a.UserOccupation).WithMany().HasForeignKey(a => a.UserOccupationId).WillCascadeOnDelete(false);
        }
    }
}
