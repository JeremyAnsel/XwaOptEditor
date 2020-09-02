using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Messages
{
    class MoveFactorSingleMessage
    {
        public bool Changed { get; set; }

        public float CenterX { get; set; }

        public float CenterY { get; set; }

        public float CenterZ { get; set; }

        public float MoveX { get; set; }

        public float MoveY { get; set; }

        public float MoveZ { get; set; }
    }
}
