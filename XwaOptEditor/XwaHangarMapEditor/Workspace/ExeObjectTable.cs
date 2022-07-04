using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor.Workspace
{
    public sealed class ExeObjectTable : Collection<ExeObjectEntry>
    {
        private const int BaseOffset = 0x1F9E40;

        private const int Length = 557;

        public static ExeObjectTable FromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(null, path);
            }

            ExeVersionString.Match(path);

            ExeObjectTable obj = new();

            using (BinaryReader file = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read), Encoding.ASCII))
            {
                for (int i = 0; i < Length; i++)
                {
                    file.BaseStream.Seek(BaseOffset + i * ExeObjectEntry.Length, SeekOrigin.Begin);

                    ExeObjectEntry entry = ExeObjectEntry.FromByteArray(file.ReadBytes(ExeObjectEntry.Length));

                    obj.Add(entry);
                }
            }

            return obj;
        }
    }
}
