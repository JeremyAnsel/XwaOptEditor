using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Mvvm
{
    public class Messenger
    {
        private static Messenger instance;

        private Dictionary<MessengerKey, object> actions;

        public Messenger()
        {
            this.actions = new Dictionary<MessengerKey, object>();
        }

        public static Messenger Instance
        {
            get
            {
                if (Messenger.instance == null)
                {
                    Messenger.instance = new Messenger();
                }

                return Messenger.instance;
            }
        }

        public void Register<T>(object receiver, Action<T> handler)
        {
            var messengerKey = new MessengerKey(receiver, typeof(T));
            this.actions.Add(messengerKey, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reviewed")]
        public void Unregister<T>(object receiver)
        {
            this.Unregister<T>(receiver, null);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "handler", Justification = "Reviewed")]
        public void Unregister<T>(object receiver, Action<T> handler)
        {
            var messengerKey = this.actions
                .Keys
                .Where(t => t.Receiver == receiver && t.MessageType == typeof(T))
                .FirstOrDefault();

            this.actions.Remove(messengerKey);
        }

        public void Unregister(object receiver)
        {
            foreach (var action in this.actions
                .Keys
                .Where(t => t.Receiver == receiver)
                .ToList())
            {
                this.actions.Remove(action);
            }
        }

        public T Notify<T>(T obj)
        {
            foreach (var action in this.actions.Where(t => t.Key.MessageType == typeof(T)))
            {
                ((Action<T>)action.Value)(obj);
            }

            return obj;
        }
    }
}
