using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;
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
    class OptMeshesConverter : IMultiValueConverter
    {
        public static OptMeshesConverter Default = new OptMeshesConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt
            // values[2]: opt cache
            // values[3]: sfoils meshes
            // values[4]: show sfoils opened
            // values[5]: show or hide solid
            // values[6]: show or hide wireframe

            if (values.Take(7).Any(t => t == System.Windows.DependencyProperty.UnsetValue))
            {
                return null;
            }

            if (values[0] == null)
            {
                return null;
            }

            var model = (ModelVisual3D)values[0];
            //model.Children.Clear();

            if (values[1] == null || values[2] == null || values[3] == null)
            {
                return null;
            }

            var opt = (OptFile)values[1];
            var cache = (OptCache)values[2];
            var sfoils = (IList<MeshModel>)values[3];
            double showSFoilsOpened = (double)values[4];
            bool showSolid = (bool)values[5];
            bool showWireframe = (bool)values[6];

            //foreach (var mesh in opt.Meshes)
            //{
            //    var visual = new OptVisual3D
            //    {
            //        Cache = cache,
            //        Mesh = mesh,
            //        IsSolid = showSolid,
            //        IsWireframe = showWireframe
            //    };

            //    model.Children.Add(visual);
            //}

            foreach (var child in model.Children)
            {
                var visual = (OptVisual3D)child;
                visual.IsSolid = showSolid;
                visual.IsWireframe = showWireframe;
                visual.Transform = Transform3D.Identity;
            }

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
