using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.WpfOpt;

namespace XwaOptEditor.Controls
{
    class CustomHelixViewport3D : GlobalHelixViewport3D
    {
        public CustomHelixViewport3D()
        {
            this.CameraChanged += CustomHelixViewport3D_CameraChanged;
            this.Loaded += CustomHelixViewport3D_Loaded;
        }

        private void CustomHelixViewport3D_CameraChanged(object sender, RoutedEventArgs e)
        {
            this.Camera.NearPlaneDistance = 10;
            this.Camera.FarPlaneDistance = 4000000;
        }

        private void CustomHelixViewport3D_Loaded(object sender, RoutedEventArgs e)
        {
            this.ResetZoom();
        }

        public void ResetZoom()
        {
            var children = this.Children.ToList();

            this.Children.Clear();

            var opt = children.Where(t => t is CustomOptVisual3D).FirstOrDefault();

            if (opt != null)
            {
                this.Children.Add(opt);
            }

            this.ZoomExtents();

            this.Children.Clear();

            foreach (var child in children)
            {
                this.Children.Add(child);
            }
        }
    }
}
