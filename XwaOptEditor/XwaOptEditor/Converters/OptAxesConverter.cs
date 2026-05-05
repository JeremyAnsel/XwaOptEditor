using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using XwaOptEditor.Extensions;

namespace XwaOptEditor.Converters
{
    internal class OptAxesConverter : BaseConverter, IMultiValueConverter
    {
        public OptAxesConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt
            // values[2]: mesh
            // values[3]: show or hide

            if (values.Take(4).Any(t => t == DependencyProperty.UnsetValue))
            {
                return null;
            }

            if (values[0] == null)
            {
                return null;
            }

            var model = (ModelVisual3D)values[0];
            model.Children.Clear();

            if (values[1] == null || values[2] == null)
            {
                return null;
            }

            var opt = (OptFile)values[1];
            var mesh = (Mesh)values[2];

            if ((bool)values[3] == false)
            {
                return null;
            }

            var pivot = mesh.RotationScale.Pivot.ToPoint3D();
            float length = opt.Size * 2;
            double diameter = opt.Size * 0.005;

            Vector3D look;

            if (mesh.RotationScale.Look.Length() == 0)
            {
                var upVector = mesh.RotationScale.Up.Normalize().ToVector3D();
                var rightVector = mesh.RotationScale.Right.Normalize().ToVector3D();
                look = Vector3D.CrossProduct(upVector, rightVector);
                look = Vector3D.Multiply(look, length);
            }
            else
            {
                look = mesh.RotationScale.Look.Normalize().Scale(length).ToVector3D();
            }

            var lookVisual = new ArrowVisual3D
            {
                Material = Materials.Red,
                Point1 = pivot,
                Point2 = pivot + look,
                Diameter = diameter
            };

            model.Children.Add(lookVisual);

            Vector3D up = mesh.RotationScale.Up.Normalize().Scale(length).ToVector3D();

            var upVisual = new ArrowVisual3D
            {
                Material = Materials.Green,
                Point1 = pivot,
                Point2 = pivot + up,
                Diameter = diameter
            };

            model.Children.Add(upVisual);

            Vector3D right = mesh.RotationScale.Right.Normalize().Scale(length).ToVector3D();

            var rightVisual = new ArrowVisual3D
            {
                Material = Materials.Blue,
                Point1 = pivot,
                Point2 = pivot + right,
                Diameter = diameter
            };

            model.Children.Add(rightVisual);

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
