using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Messages
{
    class OpenFileDialogMessage
    {
        public string Title { get; set; }

        public string DefaultExtension { get; set; }

        public string Filter { get; set; }

        public string FileName { get; set; }
    }
}
