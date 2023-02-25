using System;
using System.Collections;
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

namespace XwaOptEditor.Converters
{
    class OptHitzonesConverter : BaseConverter, IMultiValueConverter
    {
        public OptHitzonesConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt or mesh
            // values[2]: show or hide
            // values[3]: selected mesh
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

            IList<Mesh> selected;

            if (values[3] is IList)
            {
                selected = ((IList)values[3]).Cast<Mesh>().ToList();
            }
            else
            {
                selected = new List<Mesh>();
                selected.Add((Mesh)values[3]);
            }

            var checkedMeshes = (IList<Mesh>)values[4];
            List<Tuple<Mesh, Vector, Vector>> hitzones;

            if (values[1] is OptFile)
            {
                hitzones = ((OptFile)values[1]).Meshes
                    .Where(t => checkedMeshes.Contains(t))
                    .Select(t => new Tuple<Mesh, Vector, Vector>(t, t.Descriptor.Center, t.Descriptor.Span.Abs()))
                    .ToList();
            }
            else if (values[1] is Mesh)
            {
                hitzones = new List<Tuple<Mesh, Vector, Vector>>()
                {
                    new Tuple<Mesh, Vector, Vector>((Mesh)values[1], ((Mesh)values[1]).Descriptor.Center, ((Mesh)values[1]).Descriptor.Span.Abs())
                };
            }
            else
            {
                return null;
            }

            var visuals = new List<ModelVisual3D>();

            foreach (var hitzone in hitzones
                .Select(t => new
                {
                    Mesh = t.Item1,
                    Position = new Point3D(-t.Item2.Y - t.Item3.Y / 2, -t.Item2.X - t.Item3.X / 2, t.Item2.Z - t.Item3.Z / 2),
                    Size = new Size3D(t.Item3.Y, t.Item3.X, t.Item3.Z)
                }))
            {
                visuals.Add(new BoundingBoxWireFrameVisual3D()
                {
                    BoundingBox = new Rect3D(hitzone.Position, hitzone.Size),
                    Color = selected.Contains(hitzone.Mesh) ? Colors.Red : Colors.YellowGreen
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
