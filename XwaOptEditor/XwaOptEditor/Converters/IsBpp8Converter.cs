using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XwaOptEditor.Converters
{
    class IsBpp8Converter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var texture = value as Texture;

            if (texture == null)
            {
                return false;
            }

            return texture.BitsPerPixel == 8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
