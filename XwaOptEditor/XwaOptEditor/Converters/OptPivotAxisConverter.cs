using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using XwaOptEditor.Controls;
using XwaOptEditor.Extensions;

namespace XwaOptEditor.Converters
{
    internal class OptPivotAxisConverter : BaseConverter, IMultiValueConverter
    {
        public OptPivotAxisConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt
            // values[2]: mesh
            // values[3]: show or hide
            // values[4]: customOptVisual3D
            // values[5]: step between 0.0 and 1.0

            if (values.Take(6).Any(t => t == DependencyProperty.UnsetValue))
            {
                return null;
            }

            if (values[0] == null)
            {
                return null;
            }

            var model = (ModelVisual3D)values[0];
            model.Children.Clear();

            if (values[1] == null || values[2] == null || values[4] == null)
            {
                return null;
            }

            var opt = (OptFile)values[1];
            var mesh = (Mesh)values[2];
            var customOptVisual3D = (CustomOptVisual3D)values[4];
            double step = (double)values[5];

            foreach (var child in customOptVisual3D.Children)
            {
                child.Transform = Transform3D.Identity;
            }

            if ((bool)values[3] == false)
            {
                return null;
            }

            var pivot = mesh.RotationScale.Pivot.ToPoint3D();
            Vector3D direction;

            if (mesh.RotationScale.Look.Length() == 0)
            {
                var up = mesh.RotationScale.Up.Normalize().ToVector3D();
                var right = mesh.RotationScale.Right.Normalize().ToVector3D();
                direction = Vector3D.CrossProduct(up, right);
                direction = Vector3D.Multiply(direction, opt.Size);
            }
            else
            {
                direction = mesh.RotationScale.Look.Normalize().Scale(opt.Size).ToVector3D();
            }

            var visual = new ArrowVisual3D
            {
                Material = Materials.Red,
                Point1 = pivot + direction,
                Point2 = pivot - direction,
                Diameter = opt.Size * 0.005
            };

            model.Children.Add(visual);

            double angle = 64.0 * step;
            Transform3D transform;
            if (mesh.RotationScale.Look.Length() == 0)
            {
                double a = angle * Math.PI / 180.0;
                double cosA = Math.Cos(a);
                transform = new ScaleTransform3D(new Vector3D(cosA, cosA, cosA), pivot);
            }
            else
            {
                double a = angle * mesh.RotationScale.Look.LengthFactor();
                transform = new RotateTransform3D(new AxisAngleRotation3D(mesh.RotationScale.Look.ToVector3D(), a), pivot);
            }

            transform.Freeze();

            foreach (int index in customOptVisual3D.GetMeshIndices(mesh))
            {
                if (index == -1)
                {
                    continue;
                }

                customOptVisual3D.Children[index].Transform = transform;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
