using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using XwaOptEditor.Helpers;

namespace XwaOptEditor.Converters
{
    class ColorConverter : BaseConverter, IValueConverter
    {
        public ColorConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ColorHelpers.FromUint((uint)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ColorHelpers.ToUint((Color)value);
        }
    }
}
