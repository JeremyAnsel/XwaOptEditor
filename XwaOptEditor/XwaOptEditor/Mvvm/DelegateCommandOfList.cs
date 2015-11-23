using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XwaOptEditor.Mvvm
{
    public class DelegateCommandOfList<T> : ICommand
    {
        private readonly Action<IList<T>> executeAction;

        private readonly Func<IList<T>, bool> canExecuteFunc;

        public DelegateCommandOfList(Action<IList<T>> executeAction)
        {
            this.executeAction = executeAction;
        }

        public DelegateCommandOfList(Action<IList<T>> executeAction, Func<IList<T>, bool> canExecuteFunc)
        {
            this.executeAction = executeAction;
            this.canExecuteFunc = canExecuteFunc;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecuteFunc != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (this.canExecuteFunc != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecuteFunc == null ? true : this.canExecuteFunc(parameter == null ? null : ((IList)parameter).Cast<T>().ToList());
        }

        public void Execute(object parameter)
        {
            this.executeAction(parameter == null ? null : ((IList)parameter).Cast<T>().ToList());
        }
    }
}
