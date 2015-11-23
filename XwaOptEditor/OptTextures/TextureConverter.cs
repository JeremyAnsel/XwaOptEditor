﻿using System;
using System.Windows.Data;
using JeremyAnsel.Xwa.Opt;

namespace OptTextures
{
    public class TextureConverter : BaseConverter, IValueConverter
    {
        public TextureConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TextureUtils.BuildOptTexture(value as Texture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
