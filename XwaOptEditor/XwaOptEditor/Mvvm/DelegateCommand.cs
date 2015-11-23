using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XwaOptEditor.Mvvm
{
    public class DelegateCommand : ICommand
    {
        private readonly Action executeAction;

        private readonly Func<bool> canExecuteFunc;

        public DelegateCommand(Action executeAction)
        {
            this.executeAction = executeAction;
        }

        public DelegateCommand(Action executeAction, Func<bool> canExecuteFunc)
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
            return this.canExecuteFunc == null ? true : this.canExecuteFunc();
        }

        public void Execute(object parameter)
        {
            this.executeAction();
        }
    }
}
