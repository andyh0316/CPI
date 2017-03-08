﻿using Cpi.Application.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels
{
    public class CallDm : BaseDm
    {
        [MaxLength(200)]
        public string CustomerName { get; set; }

        [MaxLength(30)]
        public string PhoneNumber { get; set; }
    }

    public class CallMap : BaseMap<CallDm>
    {
        public CallMap()
        {
            ToTable("Call");
        }
    }
}
