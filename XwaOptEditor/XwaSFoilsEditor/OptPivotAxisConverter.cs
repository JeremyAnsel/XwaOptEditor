using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace XwaSFoilsEditor
{
    class OptPivotAxisConverter : IMultiValueConverter
    {
        public static OptPivotAxisConverter Default = new OptPivotAxisConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt
            // values[2]: show or hide
            // values[3]: selected mesh model

            if (values.Take(4).Any(t => t == System.Windows.DependencyProperty.UnsetValue))
            {
                return null;
            }

            if (values[0] == null)
            {
                return null;
            }

            var model = (ModelVisual3D)values[0];
            model.Children.Clear();

            if (values[1] == null || (bool)values[2] == false)
            {
                return null;
            }

            var opt = (OptFile)values[1];

            if (values[3] == null)
            {
                return null;
            }

            var selected = (MeshModel)values[3];

            var pivot = selected.Pivot.ToPoint3D();
            var direction = selected.Look.Normalize().Scale(opt.Size).ToVector3D();

            var visual = new ArrowVisual3D
            {
                Material = Materials.Red,
                Point1 = pivot + direction,
                Point2 = pivot - direction,
                Diameter = opt.Size * 0.005
            };

            model.Children.Add(visual);

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
