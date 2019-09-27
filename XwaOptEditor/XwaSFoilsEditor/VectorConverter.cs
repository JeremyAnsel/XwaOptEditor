using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XwaSFoilsEditor
{
    class VectorConverter : IValueConverter
    {
        public static VectorConverter Default = new VectorConverter();

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
