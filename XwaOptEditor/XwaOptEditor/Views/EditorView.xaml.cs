using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.WpfOpt;
using XwaOptEditor.Models;
using XwaOptEditor.ViewModels;

namespace XwaOptEditor.Views
{
    /// <summary>
    /// Logique d'interaction pour EditorView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();
        }

        private EditorViewModel ViewModel
        {
            get { return (EditorViewModel)this.DataContext; }
        }

        public OptModel OptModel
        {
            get { return (OptModel)this.GetValue(EditorView.OptModelProperty); }
            set { this.SetValue(EditorView.OptModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OptModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptModelProperty =
            DependencyProperty.Register("OptModel", typeof(OptModel), typeof(EditorView), new PropertyMetadata(new PropertyChangedCallback(EditorView.OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var view = (EditorView)sender;

            switch (e.Property.Name)
            {
                case "OptModel":
                    view.ViewModel.OptModel = (OptModel)e.NewValue;
                    break;
            }
        }

        private void HelixViewport3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = (HelixViewport3D)sender;
            var mousePosition = e.GetPosition(viewport);

            var hit = viewport.Viewport
                .FindHits(mousePosition)
                .Select(h => new
                {
                    Result = h,
                    Opt = viewport.Children
                        .Where(t => t is OptVisual3D)
                        .Cast<OptVisual3D>()
                        .Where(t => t.Children.Contains(h.Visual))
                        .FirstOrDefault()
                })
                .FirstOrDefault(t => t.Opt != null && !(t.Result.Visual is LinesVisual3D));

            if (hit == null)
            {
                return;
            }

            MeshLodFace face;

            if (!hit.Opt.ModelToMeshLodFace.TryGetValue(hit.Result.Model, out face))
            {
                return;
            }

            var menu = this.Resources["ModelMenu"] as ContextMenu;
            menu.Tag = new Tuple<MeshLodFace, Point3D>(face, hit.Result.Position);
            menu.PlacementTarget = viewport;
            menu.IsOpen = true;
        }
    }
}
