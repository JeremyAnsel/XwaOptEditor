using JeremyAnsel.Xwa.HooksConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaSFoilsEditor
{
    public sealed class SFoil
    {
        public SFoil()
        {
        }

        public SFoil(int meshIndex, int angle, int closingSpeed, int openingSpeed)
        {
            this.MeshIndex = meshIndex;
            this.Angle = angle;
            this.ClosingSpeed = closingSpeed;
            this.OpeningSpeed = openingSpeed;
        }

        public int MeshIndex { get; set; }

        public int Angle { get; set; }

        public int ClosingSpeed { get; set; }

        public int OpeningSpeed { get; set; }

        public static IList<string> GetSFoilsLines(string ship)
        {
            var lines = XwaHooksConfig.GetFileLines(ship + "SFoils.txt");

            if (lines.Count == 0)
            {
                lines = XwaHooksConfig.GetFileLines(ship + ".ini", "SFoils");
            }

            return lines;
        }

        public static IList<string> GetSFoilsLandingGearsLines(string ship)
        {
            var lines = XwaHooksConfig.GetFileLines(ship + "SFoilsLandingGears.txt");

            if (lines.Count == 0)
            {
                lines = XwaHooksConfig.GetFileLines(ship + ".ini", "SFoilsLandingGears");
            }

            return lines;
        }

        public static IList<SFoil> GetSFoilsList(string optFilename)
        {
            string ship = XwaHooksConfig.GetStringWithoutExtension(optFilename);

            var sfoilsLines = GetSFoilsLines(ship);
            var landingGearsLines = GetSFoilsLandingGearsLines(ship);

            var lines = new List<string>(sfoilsLines.Count + landingGearsLines.Count);
            lines.AddRange(sfoilsLines);
            lines.AddRange(landingGearsLines);

            IList<IList<string>> values = XwaHooksConfig.GetFileListValues(lines);
            var sfoils = new List<SFoil>();

            foreach (IList<string> value in values)
            {
                if (value.Count < 4)
                {
                    continue;
                }

                var sfoil = new SFoil
                {
                    MeshIndex = XwaHooksConfig.ToInt32(value[0]),
                    Angle = XwaHooksConfig.ToInt32(value[1]),
                    ClosingSpeed = XwaHooksConfig.ToInt32(value[2]),
                    OpeningSpeed = XwaHooksConfig.ToInt32(value[3]),
                };

                sfoils.Add(sfoil);
            }

            return sfoils;
        }
    }
}
