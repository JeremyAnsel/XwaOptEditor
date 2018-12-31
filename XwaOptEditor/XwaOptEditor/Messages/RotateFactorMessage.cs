using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Messages
{
    class RotateFactorMessage
    {
        public bool Changed { get; set; }

        public float CenterX { get; set; }

        public float CenterY { get; set; }

        public float Angle{ get; set; }
    }
}
