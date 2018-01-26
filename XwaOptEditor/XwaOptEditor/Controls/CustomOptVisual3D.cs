using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;

namespace XwaOptEditor.Controls
{
    class CustomOptVisual3D : OptVisual3D
    {
        public CustomOptVisual3D()
        {
            this.ModelChanged += CustomOptVisual3D_ModelChanged;
        }

        private HelixViewport3D GetViewport()
        {
            DependencyObject obj = this;

            while (obj != null)
            {
                obj = VisualTreeHelper.GetParent(obj);

                if (obj is HelixViewport3D)
                {
                    break;
                }
            }

            if (obj == null)
            {
                return null;
            }

            return (HelixViewport3D)obj;
        }

        private void CustomOptVisual3D_ModelChanged(object sender, EventArgs e)
        {
            CustomOptVisual3D opt = (CustomOptVisual3D)sender;

            var viewport = opt.GetViewport();

            if (viewport != null)
            {
                viewport.ResetCamera();
                ((CustomHelixViewport3D)viewport).ResetZoom();
            }
        }
    }
}
