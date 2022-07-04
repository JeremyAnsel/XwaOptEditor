using JeremyAnsel.Xwa.HooksConfig;
using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public sealed class OptModel
    {
        public OptFile File { get; set; }

        public OptCache Cache { get; set; }

        public IList<SFoil> SFoils { get; set; }

        public float ClosedSFoilsElevation { get; set; }

        public float ClosedSFoilsElevationInverted { get; set; }

        public static OptModel FromFile(string path, int version, IList<int> objectProfile = null, IList<string> skins = null)
        {
            OptFile opt = OptFile.FromFile(path);

            opt = OptProfileHelper.GetTransformedOpt(
                opt,
                version,
                objectProfile ?? new List<int>(),
                skins ?? new List<string>());

            var model = new OptModel
            {
                File = opt
            };

            model.Cache = new OptCache(model.File);
            model.SFoils = SFoil.GetSFoilsList(path);

            string ship = XwaHooksConfig.GetStringWithoutExtension(path);
            IList<string> lines;

            lines = XwaHooksConfig.GetFileLines(ship + "Size.txt");

            if (lines.Count == 0)
            {
                lines = XwaHooksConfig.GetFileLines(ship + ".ini", "Size");
            }

            model.ClosedSFoilsElevation = XwaHooksConfig.GetFileKeyValueInt(lines, "ClosedSFoilsElevation");

            if (model.ClosedSFoilsElevation == 0)
            {
                if (path.EndsWith("BWing", StringComparison.OrdinalIgnoreCase))
                {
                    model.ClosedSFoilsElevation = 50;
                }
                else
                {
                    model.ClosedSFoilsElevation = model.File.SpanSize.Z / 2;
                }
            }

            model.ClosedSFoilsElevationInverted = XwaHooksConfig.GetFileKeyValueInt(lines, "ClosedSFoilsElevationInverted");

            if (model.ClosedSFoilsElevationInverted == 0)
            {
                model.ClosedSFoilsElevationInverted = model.ClosedSFoilsElevation;
            }

            return model;
        }
    }
}
