using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.HooksConfig;
using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace XwaSFoilsEditor
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public OptFile OptFile { get; set; }

        public OptCache OptCache { get; set; }

        public ObservableCollection<MeshModel> Meshes { get; } = new ObservableCollection<MeshModel>();

        private void viewport3D_CameraChanged(object sender, RoutedEventArgs e)
        {
            var viewport = (HelixViewport3D)sender;

            if (viewport.Camera is PerspectiveCamera)
            {
                viewport.Camera.NearPlaneDistance = 10;
                viewport.Camera.FarPlaneDistance = 4000000;
            }
            else
            {
                viewport.Camera.NearPlaneDistance = double.NegativeInfinity;
                viewport.Camera.FarPlaneDistance = double.PositiveInfinity;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.openButton_Click(null, null);
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.AddExtension = true;
            dialog.DefaultExt = ".opt";
            dialog.Filter = "OPT files|*.opt";

            if (dialog.ShowDialog(this) == true)
            {
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

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.OptFile == null || string.IsNullOrEmpty(this.OptFile.FileName))
            {
                return;
            }

            foreach (MeshModel model in this.Meshes)
            {
                if (model.Angle == 0)
                {
                    continue;
                }

                var mesh = this.OptFile.Meshes[model.MeshIndex];

                mesh.RotationScale.Pivot = model.Pivot;
                mesh.RotationScale.Look = model.Look;
                mesh.RotationScale.Up = model.Up;
                mesh.RotationScale.Right = model.Right;
            }

            string baseFilename = GetBaseOptFilename(this.OptFile.FileName);
            var iniFile = new XwaIniFile(baseFilename);
            iniFile.ParseIni();
            iniFile.Read("SFoils", "SFoils", true);

            var iniList = iniFile.RetrieveLinesList("SFoils");
            iniList.Add(";mesh index, angle, closing speed, opening speed");

            foreach (MeshModel model in this.Meshes)
            {
                if (model.Angle == 0)
                {
                    continue;
                }

                iniList.Add(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", model.MeshIndex, model.Angle, model.ClosingSpeed, model.OpeningSpeed));
            }

            try
            {
                this.OptFile.Save(this.OptFile.FileName);
                iniFile.Save();
                MessageBox.Show(this, "\"" + this.OptFile.FileName + "\" saved.", this.Title);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void getSFoilsButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Meshes.Count == 0)
            {
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine(";mesh index, angle, closing speed, opening speed");

            foreach (MeshModel model in this.Meshes)
            {
                if (model.Angle == 0)
                {
                    continue;
                }

                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}\n", model.MeshIndex, model.Angle, model.ClosingSpeed, model.OpeningSpeed);
            }

            sb.AppendLine();

            foreach (MeshModel model in this.Meshes)
            {
                if (model.Angle == 0)
                {
                    continue;
                }

                sb.AppendFormat(CultureInfo.InvariantCulture, "{0} - {1}\n", model.MeshIndex, model.Name);
                sb.AppendLine("Pivot (m): " + model.Pivot.Scale(OptFile.ScaleFactor, OptFile.ScaleFactor, OptFile.ScaleFactor).ToString());
                sb.AppendLine("Pivot: " + model.Pivot.ToString());
                sb.AppendLine("Look: " + model.Look.ToString());
                sb.AppendLine("Up: " + model.Up.ToString());
                sb.AppendLine("Right: " + model.Right.ToString());
                sb.AppendLine("Angle: " + model.Angle.ToString());
                sb.AppendLine("Closing Speed: " + model.ClosingSpeed.ToString());
                sb.AppendLine("Opening Speed: " + model.OpeningSpeed.ToString());
                sb.AppendLine();
            }

            var dialog = new ListDialog(this);
            dialog.text.Text = sb.ToString();
            dialog.ShowDialog();
        }

        private static string GetBaseOptFilename(string filename)
        {
            string baseFilename = System.IO.Path.ChangeExtension(filename, null);

            if (baseFilename.EndsWith("exterior", StringComparison.OrdinalIgnoreCase))
            {
                baseFilename = baseFilename.Substring(0, baseFilename.Length - "exterior".Length);
            }
            else if (baseFilename.EndsWith("cockpit", StringComparison.OrdinalIgnoreCase))
            {
                baseFilename = baseFilename.Substring(0, baseFilename.Length - "cockpit".Length);
            }

            return baseFilename;
        }

        private void LoadOpt(string filename)
        {
            this.DataContext = null;

            string baseFilename = GetBaseOptFilename(filename);

            string optFilename;

            if (System.IO.File.Exists(baseFilename + "Exterior.opt"))
            {
                optFilename = baseFilename + "Exterior.opt";
            }
            else
            {
                optFilename = baseFilename + ".opt";

                if (!System.IO.File.Exists(optFilename))
                {
                    throw new System.IO.FileNotFoundException(null, optFilename);
                }
            }

            var opt = OptFile.FromFile(optFilename);
            var cache = new OptCache(opt);
            var sfoils = SFoil.GetSFoilsList(baseFilename + ".opt");

            this.OptFile = opt;
            this.OptCache = cache;

            this.modelVisual3D.Children.Clear();
            this.hitzonesVisual3D.Children.Clear();
            this.pivotAxisVisual3D.Children.Clear();

            foreach (var mesh in opt.Meshes)
            {
                var visual = new OptVisual3D
                {
                    Cache = cache,
                    Mesh = mesh,
                };

                this.modelVisual3D.Children.Add(visual);
            }

            this.Meshes.Clear();

            for (int meshIndex = 0; meshIndex < opt.Meshes.Count; meshIndex++)
            {
                var mesh = opt.Meshes[meshIndex];
                var sfoil = sfoils.Where(t => t.MeshIndex == meshIndex).FirstOrDefault() ?? new SFoil();

                var model = new MeshModel
                {
                    Name = mesh.Descriptor.MeshType.ToString(),
                    MeshIndex = meshIndex,
                    Pivot = mesh.RotationScale.Pivot,
                    Look = mesh.RotationScale.Look,
                    Up = mesh.RotationScale.Up,
                    Right = mesh.RotationScale.Right,
                    Angle = sfoil.Angle,
                    OpeningSpeed = sfoil.OpeningSpeed,
                    ClosingSpeed = sfoil.ClosingSpeed
                };

                this.Meshes.Add(model);
            }

            this.viewport3D.ZoomExtents();

            this.DataContext = null;
            this.DataContext = this;
        }
    }
}
