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
            if (values.Length < 3 ||
                values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue ||
                values[2] == DependencyProperty.UnsetValue)
            {
                return null;

            }

            OptFile optFile = values[2] as OptFile;

            if (optFile == null)
            {
                return null;
            }

            IList<string> textureNames = (IList<string>)values[0];
            int modelVersion = (int)values[1];

            if (modelVersion < 0 || modelVersion >= textureNames.Count)
            {
                return null;
            }

            string textureName = textureNames[modelVersion];

            Texture texture = optFile.Textures.ContainsKey(textureName) ? optFile.Textures[textureName] : null;

            return TextureHelpers.BuildOptTexture(texture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
