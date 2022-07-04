using JeremyAnsel.Xwa.HooksConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    internal static class StringConverter
    {
        public static int ToInt32(string text)
        {
            return XwaHooksConfig.ToInt32(text);
        }

        public static ushort ToUInt16(string text)
        {
            int value = StringConverter.ToInt32(text);
            return Convert.ToUInt16(value);
        }

        public static ushort ToInt16OrUInt16(string text)
        {
            int value = StringConverter.ToInt32(text);

            if (value < 0)
            {
                return (ushort)Convert.ToInt16(value);
            }
            else
            {
                return Convert.ToUInt16(value);
            }
        }
    }
}
