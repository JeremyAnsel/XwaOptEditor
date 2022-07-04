using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor.Workspace
{
    public static class ExeVersionString
    {
        private const int BaseOffset = 0x200E19;

        public const string XwaExeVersion = @"X-Wing Alliance\V2.0";

        public static bool IsMatch(string path)
        {
            string version;

            using (BinaryReader file = new(new FileStream(path, FileMode.Open, FileAccess.Read), Encoding.ASCII))
            {
                file.BaseStream.Seek(BaseOffset, SeekOrigin.Begin);
                version = new string(file.ReadChars(XwaExeVersion.Length));
            }

            return string.Equals(version, XwaExeVersion, StringComparison.Ordinal);
        }

        public static void Match(string path)
        {
            if (!IsMatch(path))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} was not found in {1}", XwaExeVersion, path), "path");
            }
        }
    }
}
