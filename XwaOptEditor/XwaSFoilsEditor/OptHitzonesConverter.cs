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

namespace XwaSFoilsEditor
{
    class OptHitzonesConverter : IMultiValueConverter
    {
        public static OptHitzonesConverter Default = new OptHitzonesConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt
            // values[2]: show or hide
            // values[3]: selected mesh model
            // values[4]: sfoils meshes
            // values[5]: show sfoils opened

            if (values.Take(6).Any(t => t == System.Windows.DependencyProperty.UnsetValue))
            {
                return null;
            }

            if (values[0] == null)
            {
                return null;
            }

            var model = (ModelVisual3D)values[0];
            model.Children.Clear();

            if (values[1] == null || values[4] == null || (bool)values[2] == false)
            {
                return null;
            }

            var opt = (OptFile)values[1];
            var selected = (MeshModel)values[3];
            var sfoils = (IList<MeshModel>)values[4];
            double showSFoilsOpened = (double)values[5];

            List<Tuple<Mesh, Vector, Vector>> hitzones = opt.Meshes
                .Select(t => new Tuple<Mesh, Vector, Vector>(t, t.Descriptor.Center, t.Descriptor.Span.Abs()))
                .ToList();

            var visuals = new List<ModelVisual3D>();

            foreach (var hitzone in hitzones
                .Select(t => new
                {
                    Mesh = t.Item1,
                    Position = new Point3D(-t.Item2.Y - t.Item3.Y / 2, -t.Item2.X - t.Item3.X / 2, t.Item2.Z - t.Item3.Z / 2),
                    Size = new Size3D(t.Item3.Y, t.Item3.X, t.Item3.Z)
                }))
            {
                bool isVisible = sfoils[opt.Meshes.IndexOf(hitzone.Mesh)].IsVisible;

                if (!isVisible)
                {
                    visuals.Add(new BoundingBoxWireFrameVisual3D());
                    continue;
                }

                visuals.Add(new BoundingBoxWireFrameVisual3D()
                {
                    BoundingBox = new Rect3D(hitzone.Position, hitzone.Size),
                    Color = selected != null && opt.Meshes.IndexOf(hitzone.Mesh) == selected.MeshIndex ? Colors.Orange : Colors.YellowGreen
                });
            }

            visuals.ForEach(t => model.Children.Add(t));

            if (showSFoilsOpened != 0)
            {
                foreach (var sfoil in sfoils)
                {
                    if (sfoil.MeshIndex >= opt.Meshes.Count)
                    {
                        continue;
                    }

                    if (sfoil.Angle == 0)
                    {
                        continue;
                    }

                    double angle = sfoil.Angle * 360.0 / 255 * sfoil.Look.LengthFactor();
                    angle *= showSFoilsOpened;
                    var transform = new RotateTransform3D(new AxisAngleRotation3D(sfoil.Look.ToVector3D(), angle), sfoil.Pivot.ToPoint3D());
                    transform.Freeze();
                    model.Children[sfoil.MeshIndex].Transform = transform;
                }
            }

            int bridgeIndex = opt.Meshes.IndexOf(opt.Meshes.FirstOrDefault(t => t.Descriptor.MeshType == MeshType.Bridge));

            if (bridgeIndex == -1)
            {
                model.Transform = Transform3D.Identity;
            }
            else
            {
                var transform = model.Children[bridgeIndex].Transform;
                model.Transform = transform;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
