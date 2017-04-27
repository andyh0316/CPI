using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.Helpers
{
    public static class EnumHelper
    {
        public static Dictionary<string, int> GetEnumIntList(Type enumType)
        {
            Array enumValues = Enum.GetValues(enumType);

            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (var value in enumValues)
            {
                dictionary.Add(value.ToString(), (int)value);
            }

            return dictionary;
        }
    }
}
