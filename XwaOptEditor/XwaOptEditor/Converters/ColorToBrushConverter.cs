using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace XwaOptEditor.Converters
{
    class ColorToBrushConverter : BaseConverter, IValueConverter
    {
        public ColorToBrushConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color? color;

            if (value is Color?)
            {
                color = (Color?)value;
            }
            else if (value is Color)
            {
                color = (Color)value;
            }
            else
            {
                color = null;
            }

            if (!color.HasValue)
            {
                return null;
            }

            return new SolidColorBrush(color.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
