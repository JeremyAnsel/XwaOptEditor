using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace XwaOptExplorer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ExecuteOpen(null, null);
        }

        private void ExecuteOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Select a folder";
            dialog.DefaultExt = ".opt";
            dialog.CheckFileExists = true;
            dialog.Filter = "OPT files (*.opt)|*.opt";

            string directory;

            if (dialog.ShowDialog(this) == true)
            {
                directory = System.IO.Path.GetDirectoryName(dialog.FileName);
            }
            else
            {
                return;
            }

            this.DataContext = null;

            this.DataContext = System.IO.Directory
                .EnumerateFiles(directory, "*.opt")
                .Select(t => new OptFileItem(t))
                .ToList();
        }

        private void OptVisual_ModelChanged(object sender, EventArgs e)
        {
            if (this.viewport == null)
            {
                return;
            }

            this.viewport.Camera.NearPlaneDistance = 10;
            this.viewport.Camera.FarPlaneDistance = 4000000;

            this.viewport.ZoomExtents();
        }
    }
}
