using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor.Workspace
{
    public sealed class ExeObjectEntry
    {
        public const int Length = 24;

        public short DataIndex1 { get; set; }

        public short DataIndex2 { get; set; }

        public static ExeObjectEntry FromByteArray(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length != Length)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The length of buffer must be {0}", Length), "buffer");
            }

            ExeObjectEntry entry = new()
            {
                DataIndex1 = BitConverter.ToInt16(buffer, 20),
                DataIndex2 = BitConverter.ToInt16(buffer, 22)
            };

            return entry;
        }
    }
}
