using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpInvoiceStatusDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            Sold = 1,
            Cancelled = 2
        }
    }

    public class LookUpInvoiceStatusMap : BaseMap<LookUpInvoiceStatusDm>
    {
        public LookUpInvoiceStatusMap()
        {
            ToTable("LookUp.InvoiceStatus");
        }
    }
}
