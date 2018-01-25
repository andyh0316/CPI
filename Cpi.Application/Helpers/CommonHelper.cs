using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.Helpers
{
    public static class CommonHelper
    {
        public static List<string> SmartPrefixes = new List<string> { "10", "15", "16", "69", "70", "81", "86", "87", "93", "98", "96" };
        public static List<string> MetFonePrefixes = new List<string> { "88", "97", "71", "60", "66", "67", "68", "90", "31", "91" };
        public static List<string> CellCardPrefixes = new List<string> { "11", "12", "14", "17", "61", "76", "77", "78", "85", "89", "92", "95", "99" };

        public static DateTime GetGlobalFilteredDate()
        {
            return new DateTime(2017, 6, 1);
        }
    }
}
