using System;
using System.Windows;
using System.Windows.Data;
using JeremyAnsel.Xwa.Opt;

namespace OptTextures
{
    public class PalletizedTextureConverter : BaseConverter, IMultiValueConverter
    {
        public PalletizedTextureConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 3 ||
                values[0] == DependencyProperty.UnsetValue ||
                values[1] == null)
                return null;

            string pal = (string)values[1];
            int mipmapLevel = (int)(double)values[2];

            if (pal == "Default")
            {
                return TextureUtils.BuildOptTexture(values[0] as Texture, TextureUtils.DefaultPalette, mipmapLevel - 1);
            }
            else
            {
                return TextureUtils.BuildOptTexture(values[0] as Texture, int.Parse(pal), mipmapLevel - 1);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
