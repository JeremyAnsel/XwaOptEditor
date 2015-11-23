using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Messages
{
    class BusyIndicatorMessage
    {
        public BusyIndicatorMessage(bool isBusy)
        {
            this.IsBusy = isBusy;
        }

        public BusyIndicatorMessage(string busyContent)
        {
            this.IsBusy = true;
            this.BusyContent = busyContent;
        }

        public bool IsBusy { get; set; }

        public string BusyContent { get; set; }
    }
}
