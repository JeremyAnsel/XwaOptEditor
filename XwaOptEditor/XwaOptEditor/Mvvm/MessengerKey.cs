using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Mvvm
{
    public class MessengerKey
    {
        public MessengerKey(object receiver, Type messageType)
        {
            this.Receiver = receiver;
            this.MessageType = messageType;
        }

        public object Receiver { get; private set; }

        public Type MessageType { get; private set; }
    }
}
