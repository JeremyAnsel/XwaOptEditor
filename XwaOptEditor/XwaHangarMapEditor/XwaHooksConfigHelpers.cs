using JeremyAnsel.Xwa.HooksConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public static class XwaHooksConfigHelpers
    {
        private static readonly TypeConverter UInt32Converter = TypeDescriptor.GetConverter(typeof(uint));

        public static KeyValuePair<string, string> GetLineSplitKeyValue(string line)
        {
            int pos = line.IndexOf('=');

            if (pos == -1)
            {
                return new KeyValuePair<string, string>();
            }

            string key = line.Substring(0, pos).Trim();
            string value = line.Substring(pos + 1).Trim();
            return new KeyValuePair<string, string>(key, value);
        }

        public static string GetLineKey(string line)
        {
            int pos = line.IndexOf('=');

            if (pos == -1)
            {
                return string.Empty;
            }

            return line.Substring(0, pos).Trim();
        }

        public static string GetLineValue(string line)
        {
            int pos = line.IndexOf('=');

            if (pos == -1)
            {
                return string.Empty;
            }

            return line.Substring(pos + 1).Trim();
        }

        public static uint ToUnsignedInt32(string text)
        {
            var sb = new StringBuilder(text.Length);
            int length = text.Length;
            int index = 0;

            while (index < length && char.IsWhiteSpace(text, index))
            {
                index++;
            }

            if (index == length)
            {
                return 0;
            }

            sb.Append("0x");

            while (index < length)
            {
                char c = text[index];

                bool isDigit = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');

                if (!isDigit)
                {
                    break;
                }

                sb.Append(c);
                index++;
            }

            if (sb.Length == 0)
            {
                return 0;
            }

            uint value = (uint)UInt32Converter.ConvertFromInvariantString(sb.ToString());

            return value;
        }

        public static uint GetFileKeyValueUnsignedInt(IList<string> lines, string key, uint defaultValue = 0)
        {
            string value = XwaHooksConfig.GetFileKeyValue(lines, key);

            if (value.Length == 0)
            {
                return defaultValue;
            }

            return ToUnsignedInt32(value);
        }
    }
}
