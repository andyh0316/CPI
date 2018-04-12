using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataTransferObjects
{
    public class UserSalaryDto
    {
        public string UserFullname { get; set; }
        public string UserNickname { get; set; }
        public string Occupation { get; set; }
        public decimal? Salary { get; set; }

        public int? AmountSold { get; set; }
        public decimal? SoldBonus { get; set; }

        public List<UserSalaryDeliveryBonusDto> DeliveryBonuses { get; set; }

        public decimal? TotalPay { get; set; }
    }

    public class UserSalaryDeliveryBonusDto
    {
        public string DeliveryDistance { get; set; }
        public int AmountDelivered { get; set; }
        public decimal Bonus { get; set; }
    }
}
