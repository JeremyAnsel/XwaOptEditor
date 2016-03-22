using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using XwaOptEditor.Helpers;

namespace XwaOptEditor.Converters
{
    class PalletizedTextureConverter : BaseConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2 ||
                values[0] == DependencyProperty.UnsetValue ||
                values[1] == null)
                return null;

            string pal = (string)values[1];

            if (pal == "Default")
            {
                return TextureHelpers.BuildOptTexture(values[0] as Texture, TextureHelpers.DefaultPalette);
            }
            else
            {
                return TextureHelpers.BuildOptTexture(values[0] as Texture, int.Parse(pal));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
