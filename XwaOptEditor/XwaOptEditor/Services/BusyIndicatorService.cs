using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XwaOptEditor.Messages;
using XwaOptEditor.Mvvm;

namespace XwaOptEditor.Services
{
    static class BusyIndicatorService
    {
        public static void Notify(string busyContent)
        {
            Messenger.Instance.Notify(new BusyIndicatorMessage(busyContent));
        }

        public static void Run(Action action)
        {
            BusyIndicatorService.Run(disp => action());
        }

        public static void Run(Action<Action<Action>> action)
        {
            Task.Factory.StartNew(() =>
            {
                Messenger.Instance.Notify(new BusyIndicatorMessage(true));

                try
                {
                    action(callback => Application.Current.Dispatcher.Invoke(callback));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(ex));
                }

                Messenger.Instance.Notify(new BusyIndicatorMessage(false));
            });
        }
    }
}
