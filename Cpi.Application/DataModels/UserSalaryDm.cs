using Cpi.Application.DataModels.Base;
using Newtonsoft.Json;
using System;

namespace Cpi.Application.DataModels
{
    public class UserSalaryDm : BaseDm
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public virtual UserDm User { get; set; }

        public DateTime? StartDate { get; set; }
        public decimal? Salary { get; set; }
    }

    public class UserSalaryMap : BaseMap<UserSalaryDm>
    {
        public UserSalaryMap()
        {
            ToTable("UserSalary");
            HasRequired(a => a.User).WithMany().HasForeignKey(a => a.UserId).WillCascadeOnDelete(false);
        }
    }
}
