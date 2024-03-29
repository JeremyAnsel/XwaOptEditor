﻿using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.OptTransform;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace XwaOptProfilesViewer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> _defaultDirectory = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public OptFile OptFile { get; set; }

        private void viewport3D_CameraChanged(object sender, RoutedEventArgs e)
        {
            const double nearDistance = 10;
            const double farDistance = 4000000;

            var viewport = (HelixViewport3D)sender;

            if (viewport.Camera is PerspectiveCamera)
            {
                viewport.Camera.NearPlaneDistance = nearDistance;
                viewport.Camera.FarPlaneDistance = farDistance;
            }
            else
            {
                viewport.Camera.NearPlaneDistance = -farDistance;
                viewport.Camera.FarPlaneDistance = farDistance;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                try
                {
                    this.LoadOpt(args[1]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return;
            }

            this.openButton_Click(null, null);
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Open OPT file";
            dialog.CheckFileExists = true;
            dialog.AddExtension = true;
            dialog.DefaultExt = ".opt";
            dialog.Filter = "OPT files|*.opt";

            if (dialog.Title != null)
            {
                string directory;
                if (this._defaultDirectory.TryGetValue(dialog.Title, out directory))
                {
                    dialog.InitialDirectory = directory;
                }
            }

            if (dialog.ShowDialog(this) == true)
            {
                if (dialog.Title != null)
                {
                    this._defaultDirectory[dialog.Title] = System.IO.Path.GetDirectoryName(dialog.FileName);
                }

                try
                {
                    this.LoadOpt(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = this.OptFile?.FileName;

            if (!System.IO.File.Exists(filename))
            {
                return;
            }

            try
            {
                this.LoadOpt(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOpt(string filename)
        {
            this.DataContext = null;
            this.OptFile = null;

            if (!System.IO.File.Exists(filename))
            {
                return;
            }

            this.OptFile = OptFile.FromFile(filename);
            this.versionSelector.Value = 0;

            this.optProfileSelector.LoadOpt(filename);

            this.DataContext = this;
            this.viewport3D.ZoomExtents();
        }

        private void exportOptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var optFile = GetTransformedOptFile();

                if (optFile == null)
                {
                    return;
                }

                string baseFilename = System.IO.Path.ChangeExtension(optFile.FileName, null);
                string saveFilename = GetSaveAsFile(baseFilename + "_profile", ".opt");

                if (string.IsNullOrEmpty(saveFilename))
                {
                    return;
                }

                optFile.Save(saveFilename);
                MessageBox.Show(this, "\"" + saveFilename + "\" saved.", this.Title);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void exportObjButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var optFile = GetTransformedOptFile();

                if (optFile == null)
                {
                    return;
                }

                string baseFilename = System.IO.Path.ChangeExtension(optFile.FileName, null);
                string saveFilename = GetSaveAsFile(baseFilename + "_profile", ".obj");

                if (string.IsNullOrEmpty(saveFilename))
                {
                    return;
                }

                OptObjConverter.Converter.OptToObj(optFile, saveFilename, true);
                MessageBox.Show(this, "\"" + saveFilename + "\" saved.", this.Title);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private OptFile GetTransformedOptFile()
        {
            if (this.OptFile == null || string.IsNullOrEmpty(this.OptFile.FileName))
            {
                return null;
            }

            int version = this.versionSelector.Value.Value;
            var selectedObjectProfile = this.optProfileSelector.SelectedObjectProfile;
            var selectedSkins = this.optProfileSelector.SelectedSkinsKeys;

            var opt = OptTransformModel.GetTransformedOpt(this.OptFile, version, selectedObjectProfile, selectedSkins);

            return opt;
        }

        private string GetSaveAsFile(string fileName, string ext)
        {
            fileName = System.IO.Path.GetFullPath(fileName);
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Save " + ext.ToUpperInvariant() + " file";
            dialog.AddExtension = true;
            dialog.DefaultExt = ext;
            dialog.Filter = string.Format(CultureInfo.InvariantCulture, "{0} files|*{1}", ext.ToUpperInvariant(), ext);
            //dialog.InitialDirectory = System.IO.Path.GetDirectoryName(fileName);
            dialog.FileName = System.IO.Path.GetFileName(fileName);

            if (dialog.Title != null)
            {
                string directory;
                if (this._defaultDirectory.TryGetValue(dialog.Title, out directory))
                {
                    dialog.InitialDirectory = directory;
                }
            }

            if (dialog.ShowDialog() == true)
            {
                if (dialog.Title != null)
                {
                    this._defaultDirectory[dialog.Title] = System.IO.Path.GetDirectoryName(dialog.FileName);
                }

                return dialog.FileName;
            }

            return null;
        }
    }
}
