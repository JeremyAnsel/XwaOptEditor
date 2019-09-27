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
    class VectorScaleConverter : IValueConverter
    {
        public static VectorScaleConverter Default = new VectorScaleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Vector v = (Vector)value;

            return new Vector(v.X * OptFile.ScaleFactor, v.Y * OptFile.ScaleFactor, v.Z * OptFile.ScaleFactor).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Vector v = Vector.Parse((string)value);
                return new Vector(v.X / OptFile.ScaleFactor, v.Y / OptFile.ScaleFactor, v.Z / OptFile.ScaleFactor);
            }
            catch
            {
                return null;
            }
        }
    }
}
