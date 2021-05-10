using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;
using XwaOptEditor.Helpers;

namespace XwaOptEditor.Converters
{
    class OptEngineGlowsConverter : BaseConverter, IMultiValueConverter
    {
        public OptEngineGlowsConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt or mesh
            // values[2]: show or hide
            // values[3]: selected engine glow
            // values[4]: checked meshes

            if (values.Take(5).Any(t => t == System.Windows.DependencyProperty.UnsetValue))
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

            var checkedMeshes = (IList<Mesh>)values[4];
            IEnumerable<EngineGlow> engineGlows;

            if (values[1] is OptFile)
            {
                engineGlows = ((OptFile)values[1]).Meshes.Where(t => checkedMeshes.Contains(t)).SelectMany(t => t.EngineGlows);
            }
            else if (values[1] is Mesh)
            {
                engineGlows = ((Mesh)values[1]).EngineGlows;
            }
            else
            {
                return null;
            }

            var visuals = new List<ModelVisual3D>();

            foreach (var engine in engineGlows)
            {
                var transform = new Transform3DGroup();

                transform.Children.Add(new ScaleTransform3D(
                    Math.Max(engine.Format.X, engine.Format.Y) * engine.Format.Z * .5,
                    engine.Format.X,
                    engine.Format.Y));

                transform.Children.Add(new TranslateTransform3D(
                    -engine.Position.Y,
                    -engine.Position.X,
                    engine.Position.Z));

                visuals.Add(new TruncatedConeVisual3D
                {
                    BaseRadius = .4,
                    Height = .8,
                    Normal = new Vector3D(-engine.Look.Y, -engine.Look.X, engine.Look.Z),
                    Material = new DiffuseMaterial(new SolidColorBrush(ColorHelpers.FromUint(engine.CoreColor))),
                    BaseCap = false,
                    Transform = transform
                });

                visuals.Add(new TruncatedConeVisual3D
                {
                    BaseRadius = .5,
                    Height = 1,
                    Normal = new Vector3D(-engine.Look.Y, -engine.Look.X, engine.Look.Z),
                    Material = new DiffuseMaterial(new SolidColorBrush(ColorHelpers.FromUint(engine.OuterColor))),
                    BaseCap = false,
                    Transform = transform
                });
            }

            var selected = values[3] as EngineGlow;

            if (selected != null && engineGlows.Contains(selected))
            {
                var engine = selected;

                double depth = Math.Max(engine.Format.X, engine.Format.Y) * engine.Format.Z * 2.0;
                double width = engine.Format.X * 2.0;
                double height = engine.Format.Y * 2.0;

                Vector3D position = new Vector3D(-engine.Position.Y, -engine.Position.X, engine.Position.Z);
                Vector3D look = new Vector3D(-engine.Look.Y, -engine.Look.X, engine.Look.Z);
                Vector3D up = new Vector3D(-engine.Up.Y, -engine.Up.X, engine.Up.Z);
                Vector3D right = new Vector3D(-engine.Right.Y, -engine.Right.X, engine.Right.Z);

                var points = new Point3DCollection();

                points.Add(new Point3D(position.X, position.Y, position.Z));
                points.Add(new Point3D(
                    position.X + look.X * depth,
                    position.Y + look.Y * depth,
                    position.Z + look.Z * depth));

                points.Add(new Point3D(
                    position.X - right.X * width,
                    position.Y - right.Y * width,
                    position.Z - right.Z * width));
                points.Add(new Point3D(
                    position.X + right.X * width,
                    position.Y + right.Y * width,
                    position.Z + right.Z * width));

                points.Add(new Point3D(
                    position.X - up.X * height,
                    position.Y - up.Y * height,
                    position.Z - up.Z * height));
                points.Add(new Point3D(
                    position.X + up.X * height,
                    position.Y + up.Y * height,
                    position.Z + up.Z * height));

                visuals.Add(new LinesVisual3D()
                {
                    Color = Colors.White,
                    Points = points
                });
            }

            visuals.ForEach(t => model.Children.Add(t));

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
