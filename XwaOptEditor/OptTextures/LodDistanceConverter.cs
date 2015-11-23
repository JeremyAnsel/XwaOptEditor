using System;
using System.Globalization;
using System.Windows.Data;

namespace OptTextures
{
    public class LodDistanceConverter : BaseConverter, IValueConverter
    {
        public LodDistanceConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float distance = (float)value;
            distance = JeremyAnsel.Xwa.Opt.OptFile.ScaleFactor / distance;
            return distance.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                float distance = float.Parse((string)value, CultureInfo.InvariantCulture);
                return JeremyAnsel.Xwa.Opt.OptFile.ScaleFactor / distance;
            }
            catch
            {
                return null;
            }
        }
    }
}
