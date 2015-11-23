using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XwaOptEditor.Mvvm
{
    public class DelegateCommandOf<T> : ICommand
    {
        private readonly Action<T> executeAction;

        private readonly Func<T, bool> canExecuteFunc;

        public DelegateCommandOf(Action<T> executeAction)
        {
            this.executeAction = executeAction;
        }

        public DelegateCommandOf(Action<T> executeAction, Func<T, bool> canExecuteFunc)
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
            return this.canExecuteFunc == null ? true : this.canExecuteFunc((T)parameter);
        }

        public void Execute(object parameter)
        {
            this.executeAction((T)parameter);
        }
    }
}
