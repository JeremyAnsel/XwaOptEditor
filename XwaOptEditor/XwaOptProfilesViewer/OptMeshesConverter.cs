using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace XwaOptProfilesViewer
{
    class OptMeshesConverter : IMultiValueConverter
    {
        public static readonly OptMeshesConverter Default = new OptMeshesConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt
            // values[2]: distance
            // values[3]: show or hide solid
            // values[4]: show or hide wireframe
            // values[5]: selected opt object profile
            // values[6]: selected opt skins

            if (values.Take(7).Any(t => t == System.Windows.DependencyProperty.UnsetValue))
            {
                return null;
            }

            if (values[0] == null)
            {
                return null;
            }

            var model = (ModelVisual3D)values[0];
            model.Children.Clear();

            if (values[1] == null)
            {
                return null;
            }

            var optFile = (OptFile)values[1];

            float distance;
            if (values[2] is float)
            {
                distance = (float)values[2];
            }
            else if (values[2] is double)
            {
                distance = (float)(double)values[2];
            }
            else
            {
                distance = 0.001f;
            }

            bool showSolid = (bool)values[3];
            bool showWireframe = (bool)values[4];
            var selectedObjectProfile = (List<int>)values[5];
            var selectedSkins = ((ItemCollection)values[6]).Cast<string>().ToList();

            var opt = OptModel.GetTransformedOpt(optFile, selectedObjectProfile, selectedSkins);
            var cache = new OptCache(opt);

            foreach (var mesh in opt.Meshes)
            {
                if (mesh.Lods.Count == 0)
                {
                    continue;
                }

                var visual = new OptVisual3D
                {
                    Cache = cache,
                    Mesh = mesh,
                    IsSolid = showSolid,
                    IsWireframe = showWireframe,
                    Distance = distance
                };

                model.Children.Add(visual);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
