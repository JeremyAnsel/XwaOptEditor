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

        public new OptCache Cache
        {
            get { return base.Cache; }
            set { base.Cache = value; ; }
        }

        public new string FileName
        {
            get { return base.FileName; }
            set { this.SetValue(FileNameProperty, value); }
        }

        public new OptFile File
        {
            get { return base.File; }
            set { base.File = value; }
        }

        public new Mesh Mesh
        {
            get { return base.Mesh; }
            set { base.Mesh = value; }
        }

        public new MeshLod Lod
        {
            get { return base.Lod; }
            set { base.Lod = value; }
        }

        public new float? Distance
        {
            get { return base.Distance; }
            set { base.Distance = value; }
        }

        public new int Version
        {
            get { return base.Version; }
            set { base.Version = value; }
        }

        public new bool IsSolid
        {
            get { return base.IsSolid; }
            set { base.IsSolid = value; }
        }

        public new bool IsWireframe
        {
            get { return base.IsWireframe; }
            set { base.IsWireframe = value; }
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
