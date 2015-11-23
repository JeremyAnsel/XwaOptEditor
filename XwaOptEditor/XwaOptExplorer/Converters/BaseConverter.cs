using System;
using System.Windows.Markup;

namespace XwaOptExplorer.Converters
{
    abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
