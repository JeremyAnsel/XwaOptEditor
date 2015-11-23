using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using JeremyAnsel.Xwa.Opt;
using XwaOptEditor.Helpers;

namespace XwaOptEditor.Converters
{
    class TextureConverter : BaseConverter, IValueConverter
    {
        public TextureConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int type = parameter is string ? int.Parse((string)parameter, CultureInfo.InvariantCulture) : 0;

            switch (type)
            {
                case 0:
                    return TextureHelpers.BuildOptTexture(value as Texture);

                case 1:
                    return TextureHelpers.BuildOptTextureAlpha(value as Texture);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
