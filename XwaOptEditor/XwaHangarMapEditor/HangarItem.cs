using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public sealed class HangarItem
    {
        public ushort ModelIndex { get; set; }

        public int Markings { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public int PositionZ { get; set; }

        public string PositionZString
        {
            get
            {
                if (IsOnFloor)
                {
                    return "Floor";
                }

                return PositionZ.ToString();
            }
        }

        public ushort HeadingXY { get; set; }

        public ushort HeadingZ { get; set; }

        public string ObjectProfile { get; set; }

        public bool IsOnFloor
        {
            get
            {
                return PositionZ == 0x7FFFFFFF;
            }
        }

        public override string ToString()
        {
            return ModelIndex + ", " + Markings + ", (" + PositionX + "," + PositionY + "," + PositionZString + "), (" + HeadingXY + "," + HeadingZ + "), " + ObjectProfile;
        }
    }
}
