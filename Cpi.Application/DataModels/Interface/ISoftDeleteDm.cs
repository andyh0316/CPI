using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels.Interface
{
    public interface ISoftDeleteDm
    {
        bool Deleted { get; set; }
    }
}
