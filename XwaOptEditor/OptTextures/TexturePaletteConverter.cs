using JeremyAnsel.Xwa.Opt;
using System;
using System.Windows.Data;

namespace OptTextures
{
    public class TexturePaletteConverter : BaseConverter, IValueConverter
    {
        public TexturePaletteConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TextureUtils.BuildOptPalette(value as Texture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
