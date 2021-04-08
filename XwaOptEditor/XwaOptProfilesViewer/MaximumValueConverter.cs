using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XwaOptProfilesViewer
{
    class MaximumValueConverter : IValueConverter
    {
        public static readonly MaximumValueConverter Default = new MaximumValueConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Max((int)value - 1, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
