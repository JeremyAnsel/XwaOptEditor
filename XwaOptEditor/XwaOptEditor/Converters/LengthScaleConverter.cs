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
    class LengthScaleConverter : BaseConverter, IValueConverter
    {
        public LengthScaleConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (((float)value) * OptFile.ScaleFactor).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return float.Parse((string)value) / OptFile.ScaleFactor;
            }
            catch
            {
                return null;
            }
        }
    }
}
