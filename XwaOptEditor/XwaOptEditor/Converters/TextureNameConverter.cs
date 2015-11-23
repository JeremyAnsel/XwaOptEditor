using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using JeremyAnsel.Xwa.Opt;
using XwaOptEditor.Helpers;

namespace XwaOptEditor.Converters
{
    class TextureNameConverter : BaseConverter, IMultiValueConverter
    {
        public TextureNameConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 ||
                values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
            {
                return null;

            }

            OptFile optFile = values[1] as OptFile;

            if (optFile == null)
            {
                return null;
            }

            Texture texture = optFile.Textures.ContainsKey((string)values[0]) ? optFile.Textures[(string)values[0]] : null;

            return TextureHelpers.BuildOptTexture(texture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
