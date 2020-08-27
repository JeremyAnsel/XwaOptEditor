using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace XwaOptEditor.Converters
{
    class OptCenterConverter : BaseConverter, IMultiValueConverter
    {
        public OptCenterConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt
            // values[2]: show or hide

            if (values.Take(3).Any(t => t == System.Windows.DependencyProperty.UnsetValue))
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

            Vector span = opt.SpanSize;
            double width = Math.Max(span.Y * 0.7, 50);
            double height = Math.Max(span.X * 0.7, 50);
            double depth = Math.Max(span.Z * 0.7, 50);

            Vector min = opt.MinSize;
            Vector max = opt.MaxSize;
            double centerX = -(min.Y + max.Y) * 0.5;
            double centerY = -(min.X + max.X) * 0.5;
            double centerZ = (min.Z + max.Z) * 0.5;

            var points0 = new Point3DCollection();
            points0.Add(new Point3D(-width, 0, 0));
            points0.Add(new Point3D(width, 0, 0));
            points0.Add(new Point3D(0, -height, 0));
            points0.Add(new Point3D(0, height, 0));
            points0.Add(new Point3D(0, 0, -depth));
            points0.Add(new Point3D(0, 0, depth));

            var visual0 = new LinesVisual3D
            {
                Color = Colors.Orange,
                Points = points0
            };

            model.Children.Add(visual0);

            var points1 = new Point3DCollection();
            points1.Add(new Point3D(centerX - width, centerY, centerZ));
            points1.Add(new Point3D(centerX + width, centerY, centerZ));
            points1.Add(new Point3D(centerX, centerY - height, centerZ));
            points1.Add(new Point3D(centerX, centerY + height, centerZ));
            points1.Add(new Point3D(centerX, centerY, centerZ - depth));
            points1.Add(new Point3D(centerX, centerY, centerZ + depth));

            var visual1 = new LinesVisual3D
            {
                Color = Colors.Red,
                Points = points1
            };

            model.Children.Add(visual1);

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
