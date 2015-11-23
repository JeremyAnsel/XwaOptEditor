using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XwaOptEditor.Converters
{
    class StringFormatConverter : BaseConverter, IValueConverter
    {
        public StringFormatConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = parameter as string;

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(culture, format, value);
            }
            else
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
