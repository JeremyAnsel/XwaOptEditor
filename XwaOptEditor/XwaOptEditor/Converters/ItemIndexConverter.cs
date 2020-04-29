using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XwaOptEditor.Converters
{
    class ItemIndexConverter : FrameworkContentElement, IValueConverter
    {
        public ItemIndexConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = DataContext as IList;

            if (list == null)
            {
                return string.Empty;
            }

            int index = list.IndexOf(value);

            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
