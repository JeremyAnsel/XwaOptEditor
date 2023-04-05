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
using System.Windows.Media;
using JeremyAnsel.Xwa.WpfOpt;

namespace XwaOptProfilesViewer
{
    class OptEngineGlowsConverter : IMultiValueConverter
    {
        public static readonly OptEngineGlowsConverter Default = new OptEngineGlowsConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt Visual3DCollection
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
            
            var optModelVisual3DCollection = values[1] as Visual3DCollection;

            if (optModelVisual3DCollection.Count == 0 || (bool)values[2] == false)
            {
                return null;
            }

            IEnumerable<EngineGlow> engineGlows;

            if (optModelVisual3DCollection[0] is OptVisual3D optVisual3D && optVisual3D.File is OptFile optFile)
            {
                engineGlows = optFile.Meshes.SelectMany(t => t.EngineGlows);
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

            visuals.ForEach(t => model.Children.Add(t));

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
