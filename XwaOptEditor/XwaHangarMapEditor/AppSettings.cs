using JeremyAnsel.Xwa.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public static class AppSettings
    {
        public const string XwaExeFileName = "XWingAlliance.exe";

        public static string WorkingDirectory { get; set; }

        public static ExeObjectStatistics Objects { get; private set; }

        public static string[] SpaceCraft { get; private set; }

        public static string[] Equipment { get; private set; }

        public static void SetData()
        {
            if (AppSettings.Objects == null)
            {
                if (Directory.Exists(AppSettings.WorkingDirectory))
                {
                    AppSettings.Objects = ExeObjectStatistics.FromFile(AppSettings.WorkingDirectory + AppSettings.XwaExeFileName);
                }
            }

            if (AppSettings.SpaceCraft == null)
            {
                if (Directory.Exists(AppSettings.WorkingDirectory))
                {
                    AppSettings.SpaceCraft = File.ReadAllLines(AppSettings.WorkingDirectory + @"FLIGHTMODELS\SPACECRAFT0.LST", Encoding.ASCII);

                    for (int i = 0; i < AppSettings.SpaceCraft.Length; i++)
                    {
                        AppSettings.SpaceCraft[i] = Path.GetFileNameWithoutExtension(AppSettings.SpaceCraft[i]);
                    }
                }
            }

            if (AppSettings.Equipment == null)
            {
                if (Directory.Exists(AppSettings.WorkingDirectory))
                {
                    AppSettings.Equipment = File.ReadAllLines(AppSettings.WorkingDirectory + @"FLIGHTMODELS\EQUIPMENT0.LST", Encoding.ASCII);

                    for (int i = 0; i < AppSettings.Equipment.Length; i++)
                    {
                        AppSettings.Equipment[i] = Path.GetFileNameWithoutExtension(AppSettings.Equipment[i]);
                    }
                }
            }
        }
    }
}
