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

        public bool IsHangarFloorInverted { get; set; }
    }
}
