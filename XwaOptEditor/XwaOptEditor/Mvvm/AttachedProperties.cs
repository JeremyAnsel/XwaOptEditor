using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace XwaOptEditor.Mvvm
{
    public static class AttachedProperties
    {
        public static CommandBindingCollection GetRegisterCommandBindings(DependencyObject obj)
        {
            return (CommandBindingCollection)obj.GetValue(RegisterCommandBindingsProperty);
        }

        public static void SetRegisterCommandBindings(DependencyObject obj, CommandBindingCollection value)
        {
            obj.SetValue(RegisterCommandBindingsProperty, value);
        }

        // Using a DependencyProperty as the backing store for RegisterCommandBindings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegisterCommandBindingsProperty =
            DependencyProperty.RegisterAttached("RegisterCommandBindings", typeof(CommandBindingCollection), typeof(AttachedProperties), new PropertyMetadata(new PropertyChangedCallback(OnRegisterCommandBindingsChanged)));

        private static void OnRegisterCommandBindingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as UIElement;

            if (element != null)
            {
                var bindings = e.NewValue as CommandBindingCollection;

                if (bindings != null)
                {
                    element.CommandBindings.Clear();
                    element.CommandBindings.AddRange(bindings);
                }
            }
        }
    }
}
