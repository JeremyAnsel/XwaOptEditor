using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XwaOptEditor.Converters
{
    class OptHardpointTypeConverter : BaseConverter, IValueConverter
    {
        public OptHardpointTypeConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            HardpointType hp = (HardpointType)value;

            if ((int)hp >= 1 && (int)hp <= 18)
            {
                return string.Concat(280 + (int)hp - 1, " ", hp);
            }

            return hp.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = (string)value;
            int index = text.IndexOf(' ');

            return (HardpointType)Enum.Parse(typeof(HardpointType), index == -1 ? text : text.Substring(index + 1));
        }
    }
}
