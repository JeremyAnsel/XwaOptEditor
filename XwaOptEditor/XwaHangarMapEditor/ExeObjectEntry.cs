using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeremyAnsel.Xwa.Statistics
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

            if (buffer.Length != ExeObjectEntry.Length)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The length of buffer must be {0}", ExeObjectEntry.Length), "buffer");
            }

            ExeObjectEntry entry = new ExeObjectEntry();

            entry.DataIndex1 = BitConverter.ToInt16(buffer, 20);
            entry.DataIndex2 = BitConverter.ToInt16(buffer, 22);

            return entry;
        }
    }
}
