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
    }
}
