using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XwaOptEditor.Mvvm
{
    public static class CommandBindingCollectionExtensions
    {
        public static void Add(this CommandBindingCollection bindings, ICommand source, ICommand command)
        {
            bindings.Add(new RelayCommandBinding(source, command));
        }

        public static void Add(this CommandBindingCollection bindings, ICommand source, ICommand command, Func<object, object> parameterSelector)
        {
            bindings.Add(new RelayCommandBinding(source, command, parameterSelector));
        }
    }
}
