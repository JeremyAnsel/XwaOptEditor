using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    internal static class StringExtensions
    {
        private static readonly string[] separators = new[] { "\r\n", "\r", "\n" };

        public static string[] SplitLines(this string str, bool removeEmptyEntries = true)
        {
            return str.Split(separators, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static IList<string> GetLines(this string str)
        {
            var lines = new List<string>();

            foreach (string strLine in str.SplitLines())
            {
                string line = strLine.Trim();

                if (line.Length == 0)
                {
                    continue;
                }

                if (line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("//"))
                {
                    continue;
                }

                lines.Add(line);
            }

            return lines;
        }
    }
}
