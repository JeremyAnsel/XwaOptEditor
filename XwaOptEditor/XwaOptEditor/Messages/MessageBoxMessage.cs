using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XwaOptEditor.Messages
{
    class MessageBoxMessage
    {
        public MessageBoxMessage(string text)
            : this(text, null, MessageBoxButton.OK)
        {
        }

        public MessageBoxMessage(string text, string caption)
            : this(text, caption, MessageBoxButton.OK)
        {
        }

        public MessageBoxMessage(string text, string caption, MessageBoxButton button)
            : this(text, caption, button, MessageBoxImage.None)
        {
        }

        public MessageBoxMessage(string text, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            this.Text = text;
            this.Caption = caption;
            this.Button = button;
            this.Icon = icon;
            this.Result = MessageBoxResult.None;
        }

        public MessageBoxMessage(Exception ex)
            :this(ex.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error)
        {
        }

        public MessageBoxMessage(string text, Exception ex)
            : this(string.Concat(text, "\n\n", ex.ToString()), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error)
        {
        }

        public MessageBoxMessage(string text, string caption, Exception ex)
            : this(string.Concat(text, "\n\n", ex.ToString()), caption, MessageBoxButton.OK, MessageBoxImage.Error)
        {
        }

        public string Text { get; set; }

        public string Caption { get; set; }

        public MessageBoxButton Button { get; set; }

        public MessageBoxImage Icon { get; set; }

        public MessageBoxResult Result { get; set; }
    }
}
