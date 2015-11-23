using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Messages
{
    class ScaleFactorMessage
    {
        public float SizeX { get; set; }

        public float SizeY { get; set; }

        public float SizeZ { get; set; }

        public bool Changed { get; set; }

        public float ScaleX { get; set; }

        public float ScaleY { get; set; }
        
        public float ScaleZ { get; set; }

        public string ScaleType { get; set; }
    }
}
