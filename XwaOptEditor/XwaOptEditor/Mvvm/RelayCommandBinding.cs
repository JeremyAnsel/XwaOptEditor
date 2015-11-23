using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XwaOptEditor.Mvvm
{
    public class RelayCommandBinding : CommandBinding
    {
        public RelayCommandBinding(ICommand source, ICommand command)
            : base(
            source,
            (sender, e) => command.Execute(null),
            (sender, e) => e.CanExecute = command.CanExecute(null))
        {
        }

        public RelayCommandBinding(ICommand source, ICommand command, Func<object,object> parameterSelector)
            : base(
            source,
            (sender, e) => command.Execute(parameterSelector(sender)),
            (sender, e) => e.CanExecute = command.CanExecute(parameterSelector(sender)))
        {
        }
    };
}
