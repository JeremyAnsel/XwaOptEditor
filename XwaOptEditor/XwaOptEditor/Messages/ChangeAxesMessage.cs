using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Messages
{
    class ChangeAxesMessage
    {
        public bool Changed { get; set; }

        public int AxisX { get; set; }

        public int AxisY { get; set; }
        
        public int AxisZ { get; set; }
    }
}
