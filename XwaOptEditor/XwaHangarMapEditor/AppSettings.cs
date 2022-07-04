using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XwaHangarMapEditor.Workspace;

namespace XwaHangarMapEditor
{
    public static class AppSettings
    {
        public const string XwaExeFileName = "XWingAlliance.exe";

        public static string WorkingDirectory { get; set; }

        public static ExeObjectTable Objects { get; private set; }

        public static string[] SpaceCraft { get; private set; }

        public static string[] Equipment { get; private set; }

        public static void SetData()
        {
            if (Objects == null)
            {
                if (Directory.Exists(WorkingDirectory))
                {
                    Objects = ExeObjectTable.FromFile(WorkingDirectory + XwaExeFileName);
                }
            }

            if (SpaceCraft == null)
            {
                if (Directory.Exists(WorkingDirectory))
                {
                    SpaceCraft = File.ReadAllLines(WorkingDirectory + @"FlightModels\Spacecraft0.lst", Encoding.ASCII);

                    for (int i = 0; i < SpaceCraft.Length; i++)
                    {
                        SpaceCraft[i] = Path.GetFileNameWithoutExtension(SpaceCraft[i]);
                    }
                }
            }

            if (Equipment == null)
            {
                if (Directory.Exists(WorkingDirectory))
                {
                    Equipment = File.ReadAllLines(WorkingDirectory + @"FlightModels\Equipment0.lst", Encoding.ASCII);

                    for (int i = 0; i < Equipment.Length; i++)
                    {
                        Equipment[i] = Path.GetFileNameWithoutExtension(Equipment[i]);
                    }
                }
            }
        }
    }
}
