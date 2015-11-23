using System;
using System.Windows;
using System.Windows.Data;
using JeremyAnsel.Xwa.Opt;

namespace OptTextures
{
    public class TextureNameConverter : BaseConverter, IMultiValueConverter
    {
        public TextureNameConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2 ||
                values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
                return null;

            OptFile optFile = values[1] as OptFile;

            if (optFile == null)
            {
                return null;
            }

            Texture texture = optFile.Textures.ContainsKey((string)values[0]) ? optFile.Textures[(string)values[0]] : null;

            return TextureUtils.BuildOptTexture(texture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
