﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataTransferObjects
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }
}