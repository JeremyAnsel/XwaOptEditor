using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XwaOptEditor.Converters
{
    class SelectFirstConverter : BaseConverter, IMultiValueConverter
    {
        public SelectFirstConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Take(1).Any(t => t == System.Windows.DependencyProperty.UnsetValue))
            {
                return null;
            }

            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
