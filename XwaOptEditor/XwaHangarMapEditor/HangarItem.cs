using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    class HangarItem
    {
        public ushort ModelIndex { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public int PositionZ { get; set; }

        public string PositionZString
        {
            get
            {
                if (this.IsOnFloor)
                {
                    return "Floor";
                }

                return this.PositionZ.ToString();
            }
        }

        public ushort HeadingXY { get; set; }

        public ushort HeadingZ { get; set; }

        public bool IsOnFloor
        {
            get
            {
                return this.PositionZ == 0x7FFFFFFF;
            }
        }

        public override string ToString()
        {
            return this.ModelIndex + ", (" + this.PositionX + "," + this.PositionY + "," + this.PositionZString + "), (" + this.HeadingXY + "," + this.HeadingZ + ")";
        }
    }
}
