using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    static class StringExtensions
    {
        private static readonly string[] separators = new[] { "\r\n", "\r", "\n" };

        public static string[] SplitLines(this string str, bool removeEmptyEntries = true)
        {
            return str.Split(separators, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }
    }
}
