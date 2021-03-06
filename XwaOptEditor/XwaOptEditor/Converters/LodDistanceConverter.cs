﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using JeremyAnsel.Xwa.Opt;

namespace XwaOptEditor.Converters
{
    class LodDistanceConverter : BaseConverter, IValueConverter
    {
        public LodDistanceConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float v;

            if (value is float)
            {
                v = (float)value;
            }
            else if (value is double)
            {
                v = (float)(double)value;
            }
            else
            {
                return null;
            }

            float data = (1.0f / v) * OptFile.ScaleFactor;
            return data.ToString("R", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float data;

            if (!float.TryParse((string)value, NumberStyles.Float, CultureInfo.InvariantCulture, out data))
            {
                return null;
            }

            data = 1.0f / (data / OptFile.ScaleFactor);

            if (targetType == typeof(float))
            {
                return data;
            }
            else if (targetType == typeof(double))
            {
                return (double)data;
            }
            else
            {
                return null;
            }
        }
    }
}
