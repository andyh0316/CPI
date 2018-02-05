using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataTransferObjects
{
    public class UserSalaryDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string UserFullname { get; set; }
        public string UserNickname { get; set; }
        public string Occupation { get; set; }
        public decimal? Salary { get; set; }

        public int? ProductSold { get; set; }

        public decimal? SalaryPaid { get; set; }
        public decimal? BonusPaid { get; set; }
    }
}
