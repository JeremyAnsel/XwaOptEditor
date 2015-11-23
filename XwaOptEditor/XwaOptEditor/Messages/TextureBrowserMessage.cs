using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JeremyAnsel.Xwa.Opt;

namespace XwaOptEditor.Messages
{
    class TextureBrowserMessage
    {
        public TextureBrowserMessage(OptFile optFile)
        {
            this.OptFile = optFile;
        }

        public OptFile OptFile { get; set; }

        public string TextureName { get; set; }
    }
}
