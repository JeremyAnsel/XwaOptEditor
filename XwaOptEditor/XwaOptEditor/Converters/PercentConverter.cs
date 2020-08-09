using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XwaOptEditor.Converters
{
    class PercentConverter : BaseConverter, IValueConverter
    {
        public PercentConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((float)value * 100).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string percent = (string)value;
                int position = percent.IndexOf('%');

                if (position != -1)
                {
                    percent = percent.Substring(0, position);
                }

                return float.Parse(percent) / 100;
            }
            catch
            {
                return null;
            }
        }
    }
}
