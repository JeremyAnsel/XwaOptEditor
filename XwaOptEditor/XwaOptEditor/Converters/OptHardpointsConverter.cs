using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;

namespace XwaOptEditor.Converters
{
    class OptHardpointsConverter : BaseConverter, IMultiValueConverter
    {
        public static Dictionary<HardpointType, Color> HardpointsColors = new Dictionary<HardpointType, Color>()
        {
            {HardpointType.None, Colors.LightGray},
            {HardpointType.RebelLaser, Color.FromRgb(255,0,0)},
            {HardpointType.TurboRebelLaser, Color.FromRgb(255,64,64)},
            {HardpointType.EmpireLaser, Color.FromRgb(0,255,0)},
            {HardpointType.TurboEmpireLaser, Color.FromRgb(64,255,64)},
            {HardpointType.IonCannon, Color.FromRgb(0,0,255)},
            {HardpointType.TurboIonCannon, Color.FromRgb(64,64,255)},
            {HardpointType.Torpedo, Colors.Brown},
            {HardpointType.Missile, Colors.Brown},
            {HardpointType.SuperRebelLaser, Color.FromRgb(255,128,128)},
            {HardpointType.SuperEmpireLaser, Color.FromRgb(128,255,128)},
            {HardpointType.SuperIonCannon, Color.FromRgb(128,128,255)},
            {HardpointType.SuperTorpedo, Colors.Brown},
            {HardpointType.SuperMissile, Colors.Brown},
            {HardpointType.DumbBomb, Colors.Brown},
            {HardpointType.FiredBomb, Colors.Brown},
            {HardpointType.MagPulse, Colors.Brown},
            {HardpointType.TurboMagPulse, Colors.Brown},
            {HardpointType.SuperMagPulse, Colors.Brown},
            {HardpointType.Gunner, Colors.Yellow},
            {HardpointType.CockpitSparks, Colors.LightGray},
            {HardpointType.DockingPoint, Colors.LightGray},
            {HardpointType.Towing, Colors.LightGray},
            {HardpointType.AccStart, Colors.LightGray},
            {HardpointType.AccEnd, Colors.LightGray},
            {HardpointType.InsideHangar, Colors.LightGray},
            {HardpointType.OutsideHangar, Colors.LightGray},
            {HardpointType.DockFromBig, Colors.LightGray},
            {HardpointType.DockFromSmall, Colors.LightGray},
            {HardpointType.DockToBig, Colors.LightGray},
            {HardpointType.DockToSmall, Colors.LightGray},
            {HardpointType.Cockpit, Colors.LightGray},
            {HardpointType.EngineGlow, Colors.LightGray},
            {HardpointType.Custom1, Colors.LightGray},
            {HardpointType.Custom2, Colors.LightGray},
            {HardpointType.Custom3, Colors.LightGray},
            {HardpointType.Custom4, Colors.LightGray},
            {HardpointType.Custom5, Colors.LightGray},
            {HardpointType.Custom6, Colors.LightGray},
            {HardpointType.JammingPoint, Colors.LightGray},
        };

        public OptHardpointsConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt or mesh
            // values[2]: show or hide
            // values[3]: show or hide text
            // values[4]: selected hardpoint
            // values[5]: checked meshes

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

            if (values[1] == null || (bool)values[2] == false)
            {
                return null;
            }

            bool showText = (bool)values[3];

            var selected = values[4] as Hardpoint;

            var checkedMeshes = (IList<Mesh>)values[5];
            IEnumerable<Hardpoint> hardpoints;

            if (values[1] is OptFile)
            {
                hardpoints = ((OptFile)values[1]).Meshes.Where(t => checkedMeshes.Contains(t)).SelectMany(t => t.Hardpoints);
            }
            else if (values[1] is Mesh)
            {
                hardpoints = ((Mesh)values[1]).Hardpoints;
            }
            else
            {
                return null;
            }

            var visuals = new List<ModelVisual3D>();

            var points = hardpoints
                .Where(t => t != selected)
                .Select(t => new
                {
                    HardpointType = t.HardpointType,
                    Position = new Point3D(-t.Position.Y, -t.Position.X, t.Position.Z)
                })
                .GroupBy(t => t.HardpointType)
                .ToList();

            foreach (var point in points)
            {
                Color color = HardpointsColors.ContainsKey(point.Key) ? HardpointsColors[point.Key] : HardpointsColors[0];

                visuals.Add(new PointsVisual3D()
                {
                    Points = new Point3DCollection(point.Select(t => t.Position).ToList()),
                    Color = color,
                    Size = 5
                });

                if (showText)
                {
                    visuals.Add(new BillboardTextGroupVisual3D()
                    {
                        Items = point.Select(t => new BillboardTextItem
                        {
                            Text = t.HardpointType.ToString(),
                            Position = t.Position,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            WorldDepthOffset = 100
                        })
                        .ToList(),
                        Foreground = new SolidColorBrush(color)
                    });
                }
            }

            if (selected != null && hardpoints.Contains(selected))
            {
                var t = selected;
                var position = new Point3D(-t.Position.Y, -t.Position.X, t.Position.Z);

                Color color = HardpointsColors.ContainsKey(t.HardpointType) ? HardpointsColors[t.HardpointType] : HardpointsColors[0];

                visuals.Add(new PointsVisual3D()
                {
                    Points = new Point3DCollection(new List<Point3D>() { position }),
                    Color = color,
                    Size = 5
                });

                if (!showText)
                {
                    visuals.Add(new BillboardTextGroupVisual3D()
                    {
                        Items = new List<BillboardTextItem>()
                        {
                            new BillboardTextItem
                            {
                                Text = "    ",
                                Position = position,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        },
                        BorderBrush = Brushes.White
                    });
                }

                if (showText)
                {
                    visuals.Add(new BillboardTextGroupVisual3D()
                    {
                        Items = new List<BillboardTextItem>()
                        {
                            new BillboardTextItem
                            {
                                Text = t.HardpointType.ToString(),
                                Position = position,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                WorldDepthOffset = 100
                            }
                        },
                        Foreground = new SolidColorBrush(color),
                        BorderBrush = Brushes.White
                    });
                }
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
