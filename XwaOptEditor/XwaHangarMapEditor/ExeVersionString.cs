using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeremyAnsel.Xwa.Statistics
{
    public static class ExeVersionString
    {
        private const int BaseOffset = 0x200E19;

        public const string XwaExeVersion = @"X-Wing Alliance\V2.0";

        public static bool IsMatch(string path)
        {
            string version;

            using (BinaryReader file = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read), Encoding.ASCII))
            {
                file.BaseStream.Seek(ExeVersionString.BaseOffset, SeekOrigin.Begin);
                version = new string(file.ReadChars(ExeVersionString.XwaExeVersion.Length));
            }

            return string.Equals(version, ExeVersionString.XwaExeVersion, StringComparison.Ordinal);
        }

        public static void Match(string path)
        {
            if (!ExeVersionString.IsMatch(path))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} was not found in {1}", ExeVersionString.XwaExeVersion, path), "path");
            }
        }
    }
}
