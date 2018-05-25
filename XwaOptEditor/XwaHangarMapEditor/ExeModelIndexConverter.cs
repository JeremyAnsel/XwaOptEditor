using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace XwaHangarMapEditor
{
    public sealed class ExeModelIndexConverter : IValueConverter
    {
        public static readonly ExeModelIndexConverter Default = new ExeModelIndexConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
            {
                return string.Empty;
            }

            int modelIndex = System.Convert.ToInt32(value, CultureInfo.InvariantCulture);

            var obj = AppSettings.Objects?.ElementAtOrDefault(modelIndex);

            if (obj == null)
            {
                return string.Empty;
            }

            if (obj.DataIndex1 == -1)
            {
                return string.Empty;
            }

            if (AppSettings.SpaceCraft != null && AppSettings.Equipment != null)
            {
                switch (obj.DataIndex1)
                {
                    case 0:
                        return AppSettings.SpaceCraft.ElementAtOrDefault(obj.DataIndex2);

                    case 1:
                        return AppSettings.Equipment.ElementAtOrDefault(obj.DataIndex2);
                }
            }

            return obj.DataIndex1 + ", " + obj.DataIndex2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
