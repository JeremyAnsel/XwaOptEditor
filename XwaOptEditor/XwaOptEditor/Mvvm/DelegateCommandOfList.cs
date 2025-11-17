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
            return this.canExecuteFunc == null ? true : this.canExecuteFunc(GetParameterList(parameter));
        }

        public void Execute(object parameter)
        {
            this.executeAction(GetParameterList(parameter));
        }

        private static List<T> GetParameterList(object parameter)
        {
            if (parameter is null)
            {
                return null;
            }

            var list = new List<T>();

            foreach (object item in (IList)parameter)
            {
                if (item is SelectableItem<T> value)
                {
                    list.Add(value.Value);
                }
                else
                {
                    list.Add((T)item);
                }
            }

            return list;
        }
    }
}
