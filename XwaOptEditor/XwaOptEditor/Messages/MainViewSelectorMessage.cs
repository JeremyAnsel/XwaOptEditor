using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Messages
{
    class MainViewSelectorMessage
    {
        public MainViewSelectorMessage(string view)
        {
            this.View = view;
        }

        public string View { get; set; }
    }
}
