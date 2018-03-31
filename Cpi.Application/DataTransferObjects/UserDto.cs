using Cpi.Application.DataModels.LookUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataTransferObjects
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Nickname { get; set; }
        public string UserRole { get; set; }
        public string UserOccupation { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public decimal? Salary { get; set; }
        public int? VacationDaysTaken { get; set; }
        public DateTime? StartDate { get; set; }
        public List<LookUpWeekDayDm> WorkDays { get; set; }
    }
}
