using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace JeremyAnsel.Xwa.WpfOpt
{
    public class GlobalLights : LightSetup
    {
        public static readonly DependencyProperty LightsIntensityProperty =
            DependencyProperty.Register("LightsIntensity", typeof(double), typeof(GlobalLights), new PropertyMetadata(1.0, IntensityChanged));

        public static readonly DependencyProperty AmbientIntensityProperty =
            DependencyProperty.Register("AmbientIntensity", typeof(double), typeof(GlobalLights), new PropertyMetadata(0.0, IntensityChanged));

        public double LightsIntensity
        {
            get { return (double)GetValue(LightsIntensityProperty); }
            set { SetValue(LightsIntensityProperty, value); }
        }

        public double AmbientIntensity
        {
            get { return (double)GetValue(AmbientIntensityProperty); }
            set { SetValue(AmbientIntensityProperty, value); }
        }

        private static void IntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GlobalLights)d).OnSetupChanged();
        }

        protected override void AddLights(Model3DGroup lightGroup)
        {
            // key light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(180, 180, 180).ChangeIntensity(this.LightsIntensity), new Vector3D(-1, -1, -1)));

            // fill light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(120, 120, 120).ChangeIntensity(this.LightsIntensity), new Vector3D(1, -1, -0.1)));

            // rim/back light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(60, 60, 60).ChangeIntensity(this.LightsIntensity), new Vector3D(0.1, 1, -1)));

            // and a little bit from below
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(50, 50, 50).ChangeIntensity(this.LightsIntensity), new Vector3D(0.1, 0.1, 1)));

            lightGroup.Children.Add(new AmbientLight(Color.FromRgb(30, 30, 30).ChangeIntensity(this.AmbientIntensity)));
        }
    }
}
