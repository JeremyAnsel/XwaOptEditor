using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using JeremyAnsel.Xwa.Opt;

namespace XwaOptEditor.Converters
{
    class VectorConverter : BaseConverter, IValueConverter
    {
        public VectorConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Vector.Parse((string)value);
            }
            catch
            {
                return null;
            }
        }
    }
}
