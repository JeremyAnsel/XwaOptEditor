using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.HooksConfig;
using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.OptTransform;
using JeremyAnsel.Xwa.OptTransform.Wpf;
using JeremyAnsel.Xwa.WpfOpt;
using Microsoft.Win32;
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
        private Dictionary<string, string> _defaultDirectory = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public OptFile OptFile { get; set; }

        public string OptFileProfile { get; set; }

        public OptCache OptCache { get; set; }

        public ObservableCollection<MeshModel> Meshes { get; } = new ObservableCollection<MeshModel>();

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

        private void Window_Loaded(object sender, EventArgs e)
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

            string baseFilename = OptTransformHelpers.GetBaseOptFilename(this.OptFile.FileName);
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

        private void LoadOpt(string filename)
        {
            this.DataContext = null;

            string baseFilename = OptTransformHelpers.GetBaseOptFilename(filename);

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

            var selector = new OptProfileSelectorDialog(optFilename);

            if (selector.ShowDialog() == true)
            {
                opt = OptTransformModel.GetTransformedOpt(opt, selector.SelectedVersion, selector.SelectedObjectProfile, selector.SelectedSkinsKeys);
                this.OptFileProfile = $"Version={selector.SelectedVersion} ObjectProfile={selector.SelectedObjectProfile} Skins={string.Join(",", selector.SelectedSkinsKeys)}";
            }
            else
            {
                this.OptFileProfile = "No profile selected";
            }

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

        private void exportOptButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.OptFile == null || string.IsNullOrEmpty(this.OptFile.FileName))
            {
                return;
            }

            try
            {
                var optFile = GetTransformedOptFile();
                string baseFilename = OptTransformHelpers.GetBaseOptFilename(optFile.FileName);

                string saveFilename = GetSaveAsFile(baseFilename + "_sfoils", ".opt");

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
            if (this.OptFile == null || string.IsNullOrEmpty(this.OptFile.FileName))
            {
                return;
            }

            try
            {
                var optFile = GetTransformedOptFile();
                string baseFilename = OptTransformHelpers.GetBaseOptFilename(optFile.FileName);

                string saveFilename = GetSaveAsFile(baseFilename + "_sfoils", ".obj");

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

        private void exportRhinoButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.OptFile == null || string.IsNullOrEmpty(this.OptFile.FileName))
            {
                return;
            }

            try
            {
                var optFile = GetTransformedOptFile();
                string baseFilename = OptTransformHelpers.GetBaseOptFilename(optFile.FileName);

                string saveFilename = GetSaveAsFile(baseFilename + "_sfoils", ".3dm");

                if (string.IsNullOrEmpty(saveFilename))
                {
                    return;
                }

                OptRhinoConverter.Converter.OptToRhino(optFile, saveFilename, true);
                MessageBox.Show(this, "\"" + saveFilename + "\" saved.", this.Title);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void exportAn8Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.OptFile == null || string.IsNullOrEmpty(this.OptFile.FileName))
            {
                return;
            }

            try
            {
                var optFile = GetTransformedOptFile();
                string baseFilename = OptTransformHelpers.GetBaseOptFilename(optFile.FileName);

                string saveFilename = GetSaveAsFile(baseFilename + "_sfoils", ".an8");

                if (string.IsNullOrEmpty(saveFilename))
                {
                    return;
                }

                OptAn8Converter.Converter.OptToAn8(optFile, saveFilename, true);
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

            var optFile = this.OptFile.Clone();

            foreach (MeshModel model in this.Meshes)
            {
                if (model.Angle == 0)
                {
                    continue;
                }

                var mesh = optFile.Meshes[model.MeshIndex];

                mesh.RotationScale.Pivot = model.Pivot;
                mesh.RotationScale.Look = model.Look;
                mesh.RotationScale.Up = model.Up;
                mesh.RotationScale.Right = model.Right;
            }

            double showSFoilsOpened = this.showSFoils.Value;

            int bridgeIndex = -1;
            Transform3D bridgeTransform = Transform3D.Identity;
            Transform3D bridgeTransformRotation = Transform3D.Identity;

            foreach (var sfoil in this.Meshes)
            {
                if (sfoil.MeshIndex >= optFile.Meshes.Count)
                {
                    continue;
                }

                if (sfoil.Angle == 0)
                {
                    continue;
                }

                double angle = sfoil.Angle * 360.0 / 255 * sfoil.Look.LengthFactor();
                angle *= showSFoilsOpened;
                var transform = new RotateTransform3D(new AxisAngleRotation3D(sfoil.Look.ToVector3D(), angle), sfoil.Pivot.ToPoint3D());
                var transformRotation = new RotateTransform3D(new AxisAngleRotation3D(sfoil.Look.ToVector3D(), angle));

                var mesh = optFile.Meshes[sfoil.MeshIndex];
                TransformMesh(mesh, transform, transformRotation);

                if (bridgeIndex == -1)
                {
                    if (mesh.Descriptor.MeshType == MeshType.Bridge)
                    {
                        bridgeIndex = sfoil.MeshIndex;
                        bridgeTransform = transform;
                        bridgeTransformRotation = transformRotation;
                    }
                }
            }

            if (this.showPivotAxis.IsChecked == true)
            {
                var texture = new JeremyAnsel.Xwa.Opt.Texture
                {
                    Name = "Arrow",
                    Width = 8,
                    Height = 8,
                    ImageData = new byte[8 * 8 * 4 * 2 - 4]
                };

                for (int i = 0; i < texture.ImageData.Length; i += 4)
                {
                    texture.ImageData[i + 0] = 0;
                    texture.ImageData[i + 1] = 0;
                    texture.ImageData[i + 2] = 0xff;
                    texture.ImageData[i + 3] = 0xff;
                }

                optFile.Textures.Add(texture.Name, texture);

                if (this.Meshes.Count != optFile.Meshes.Count)
                {
                    throw new InvalidOperationException("S-Foils items count is not equals to the OPT meshes count");
                }

                for (int modelIndex = 0; modelIndex < this.Meshes.Count; modelIndex++)
                {
                    MeshModel model = this.Meshes[modelIndex];

                    var optMesh = new JeremyAnsel.Xwa.Opt.Mesh();
                    //optFile.Meshes.Add(optMesh);
                    optFile.Meshes.Insert(modelIndex * 2 + 1, optMesh);

                    if (model.Angle == 0)
                    {
                        continue;
                    }

                    var pivot = model.Pivot.ToPoint3D();
                    var direction = model.Look.Normalize().Scale(optFile.Size).ToVector3D();

                    var visual = new ArrowVisual3D
                    {
                        Material = Materials.Red,
                        Point1 = pivot + direction,
                        Point2 = pivot - direction,
                        Diameter = optFile.Size * 0.005
                    };

                    MeshGeometry3D geometry = (MeshGeometry3D)visual.Model.Geometry;

                    foreach (Point3D position in geometry.Positions)
                    {
                        optMesh.Vertices.Add(position.ToVector());
                    }

                    foreach (Vector3D normal in geometry.Normals)
                    {
                        optMesh.VertexNormals.Add(normal.ToPoint3D().ToVector());
                    }

                    foreach (Point coords in geometry.TextureCoordinates)
                    {
                        optMesh.TextureCoordinates.Add(new TextureCoordinates((float)coords.X, -(float)coords.Y));
                    }

                    var optLod = new JeremyAnsel.Xwa.Opt.MeshLod();
                    optMesh.Lods.Add(optLod);

                    var optFaceGroup = new JeremyAnsel.Xwa.Opt.FaceGroup();
                    optLod.FaceGroups.Add(optFaceGroup);

                    optFaceGroup.Textures.Add(texture.Name);

                    for (int index = 0; index < geometry.TriangleIndices.Count; index += 3)
                    {
                        int index0 = geometry.TriangleIndices[index];
                        int index1 = geometry.TriangleIndices[index + 1];
                        int index2 = geometry.TriangleIndices[index + 2];
                        Indices indices = new(index0, index1, index2);

                        var face = new JeremyAnsel.Xwa.Opt.Face
                        {
                            VerticesIndex = indices,
                            VertexNormalsIndex = indices,
                            TextureCoordinatesIndex = indices
                        };

                        optFaceGroup.Faces.Add(face);
                    }
                }
            }

            if (bridgeIndex != -1)
            {
                foreach (var mesh in optFile.Meshes)
                {
                    TransformMesh(mesh, bridgeTransform, bridgeTransformRotation);
                }
            }

            return optFile;
        }

        private static void TransformMesh(Mesh mesh, Transform3D transform, Transform3D transformRotation)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i] = mesh.Vertices[i].Tranform(transform);
            }

            for (int i = 0; i < mesh.VertexNormals.Count; i++)
            {
                mesh.VertexNormals[i] = mesh.VertexNormals[i].Tranform(transformRotation);
            }

            mesh.EngineGlows.Clear();
            mesh.Hardpoints.Clear();
            mesh.ComputeHitzone();
        }

        private string GetSaveAsFile(string fileName, string ext)
        {
            fileName = System.IO.Path.GetFullPath(fileName);
            var dialog = new SaveFileDialog();
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
