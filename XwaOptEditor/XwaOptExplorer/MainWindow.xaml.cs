using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
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
                .Where(t => t.EndsWith(".opt", StringComparison.OrdinalIgnoreCase))
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

        private static readonly Dictionary<string, string> _tools = new()
        {
            {"Structure...", "OptStructure"},
            {"Textures...", "OptTextures"},
            {"Editor...", "XwaOptEditor"},
            {"Profiles...", "XwaOptProfilesViewer"},
            {"SFoils...", "XwaSFoilsEditor"},
        };

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var element = (ButtonBase)sender;

            if (element.Tag is not string fileName || element.Content is not string content)
            {
                return;
            }

            if (!_tools.TryGetValue(content, out string toolName))
            {
                return;
            }

            string toolPath = GetToolDirectory(toolName);

            if (toolPath is null)
            {
                return;
            }

            Process.Start(toolPath, $"\"{fileName}\"");
        }

        private static string GetToolDirectory(string toolName)
        {
            if (File.Exists(toolName + ".exe"))
            {
                return toolName + ".exe";
            }

            string[] directories = Environment.CurrentDirectory.Split(Path.DirectorySeparatorChar);
            directories[directories.Length - 4] = toolName;

            string directory = string.Join(Path.DirectorySeparatorChar.ToString(), directories);
            string toolPath = directory + Path.DirectorySeparatorChar + toolName + ".exe";

            if (File.Exists(toolPath))
            {
                return toolPath;
            }

            return null;
        }
    }
}
