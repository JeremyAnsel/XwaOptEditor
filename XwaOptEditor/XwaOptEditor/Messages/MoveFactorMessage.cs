using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Messages
{
    class MoveFactorMessage
    {
        public bool Changed { get; set; }

        public float MoveX { get; set; }

        public float MoveY { get; set; }

        public float MoveZ { get; set; }
    }
}
