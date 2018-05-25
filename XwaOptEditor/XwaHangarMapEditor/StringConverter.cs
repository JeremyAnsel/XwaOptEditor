using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    static class StringConverter
    {
        private static readonly TypeConverter Int32Converter = TypeDescriptor.GetConverter(typeof(int));

        public static int ToInt32(string text)
        {
            text = text.Trim();

            bool isNegative = text.StartsWith("-");
            if (isNegative)
            {
                text = text.Substring(1).TrimStart();
            }

            int value = (int)Int32Converter.ConvertFromInvariantString(text);
            if (isNegative)
            {
                value = -value;
            }

            return value;
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
