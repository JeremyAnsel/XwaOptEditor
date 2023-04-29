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
    public class GlobalHelixViewport3D : HelixViewport3D
    {
        public static readonly DependencyProperty HeadLightIntensityProperty =
            DependencyProperty.Register("HeadLightIntensity", typeof(double), typeof(GlobalHelixViewport3D), new PropertyMetadata(1.0, HeadLightIntensityChanged));

        public double HeadLightIntensity
        {
            get { return (double)GetValue(HeadLightIntensityProperty); }
            set { SetValue(HeadLightIntensityProperty, value); }
        }

        private static void HeadLightIntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewport = (GlobalHelixViewport3D)d;

            if (viewport.Lights.Children.Count == 0)
            {
                return;
            }

            if (viewport.Lights.Children[0] is not DirectionalLight light)
            {
                return;
            }

            light.Color = Colors.White.ChangeIntensity(viewport.HeadLightIntensity);
        }
    }
}
