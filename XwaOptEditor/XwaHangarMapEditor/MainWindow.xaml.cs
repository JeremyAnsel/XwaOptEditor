using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.HooksConfig;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XwaHangarMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, string> _defaultDirectory = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private ViewModel ViewModel
        {
            get
            {
                return (ViewModel)this.Resources["viewModel"];
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetWorkingDirectory();

            bool error = false;

            if (System.IO.Directory.Exists(AppSettings.WorkingDirectory))
            {
                try
                {
                    AppSettings.SetData();
                    this.workingDirectoryText.Text = AppSettings.WorkingDirectory;

                    if (this.Resources["viewModel"] is ViewModel viewModel)
                    {
                        viewModel.UpdateAllProperties();
                        this.viewport3D.ResetCamera();
                    }
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    error = true;
                }
            }
            else
            {
                error = true;
            }

            if (error)
            {
                this.Close();
                return;
            }
        }

        private void SetWorkingDirectory()
        {
            var dlg = new FolderBrowserForWPF.Dialog
            {
                Title = "Choose a working directory containing " + AppSettings.XwaExeFileName + " or a child directory"
            };

            if (dlg.ShowDialog() == true)
            {
                string fileName = dlg.FileName;

                if (!System.IO.File.Exists(System.IO.Path.Combine(fileName, "XWingAlliance.exe")))
                {
                    fileName = System.IO.Path.GetDirectoryName(fileName);

                    if (!System.IO.File.Exists(System.IO.Path.Combine(fileName, "XWingAlliance.exe")))
                    {
                        return;
                    }
                }

                AppSettings.WorkingDirectory = fileName + System.IO.Path.DirectorySeparatorChar;
            }
        }

        private void Viewport3D_CameraChanged(object sender, RoutedEventArgs e)
        {
            var viewport = (HelixViewport3D)sender;

            viewport.Camera.NearPlaneDistance = 10;
            viewport.Camera.FarPlaneDistance = 4000000;
        }

        private void ExecuteHelp(object sender, ExecutedRoutedEventArgs e)
        {
            var help = new HelpWindow(this);
            help.ShowDialog();
        }

        private void ExecuteNew(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.HangarBaseName = null;
            this.ViewModel.HangarSkinsText = GlobalConstants.DefaultHangarSkinsText;
            this.ViewModel.HangarObjectsText = GlobalConstants.DefaultHangarObjectsText;
            this.ViewModel.HangarMapText = GlobalConstants.DefaultHangarMapText;
            this.viewport3D.ResetCamera();
        }

        private void ExecuteOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Open OPT file",
                DefaultExt = ".opt",
                CheckFileExists = true,
                Filter = "Hangar OPT files|*.opt",
                InitialDirectory = AppSettings.WorkingDirectory + "FlightModels"
            };

            string fileName;

            if (dialog.ShowDialog(this) == true)
            {
                fileName = dialog.FileName;
            }
            else
            {
                return;
            }

            try
            {
                string hangarBaseName;
                string hangarModel;

                if (fileName.EndsWith("Hangar.opt", StringComparison.OrdinalIgnoreCase))
                {
                    hangarBaseName = fileName.Substring(0, fileName.IndexOf("Hangar.opt", StringComparison.OrdinalIgnoreCase));
                    hangarModel = System.IO.Path.GetFileNameWithoutExtension(fileName);
                }
                else
                {
                    hangarBaseName = System.IO.Path.ChangeExtension(fileName, null);
                    hangarModel = System.IO.Path.GetFileNameWithoutExtension(fileName);

                    if (System.IO.File.Exists(hangarBaseName + "Hangar.opt"))
                    {
                        hangarModel += "Hangar";
                    }
                }

                var iniFile = new XwaIniFile(hangarBaseName);
                iniFile.ParseIni();
                iniFile.Read("Skins", "Skins");
                iniFile.Read("HangarObjects", "HangarObjects");
                iniFile.Read("HangarMap", "HangarMap");
                iniFile.Read("FamHangarMap", "FamHangarMap");

                string hangarSkinsText = string.Join("\n", iniFile.Sections["Skins"].Lines);
                string hangarObjectsText = string.Join("\n", iniFile.Sections["HangarObjects"].Lines);
                string hangarMapText = string.Join("\n",
                    iniFile.Sections["HangarMap"].Lines
                    .Union(iniFile.Sections["FamHangarMap"].Lines));

                this.ViewModel.InvokePropertyChanged = false;

                this.ViewModel.HangarBaseName = null;
                this.ViewModel.HangarModel = null;
                this.ViewModel.HangarSkinsText = null;
                this.ViewModel.HangarObjectsText = null;
                this.ViewModel.HangarMapText = null;

                this.ViewModel.HangarModel = hangarModel;
                this.ViewModel.HangarSkinsText = hangarSkinsText;
                this.ViewModel.HangarObjectsText = hangarObjectsText;
                this.ViewModel.HangarMapText = hangarMapText;
                this.ViewModel.HangarBaseName = hangarBaseName;

                this.ViewModel.InvokePropertyChanged = true;
                this.ViewModel.UpdateAllProperties();

                this.viewport3D.ResetCamera();
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, fileName + "\n" + ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSave(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.ViewModel.HangarBaseName))
            {
                this.ExecuteSaveAs(null, null);
                return;
            }

            try
            {
                this.WriteText(this.ViewModel.HangarBaseName + ".ini");
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save hangar file",
                AddExtension = true,
                DefaultExt = ".ini",
                Filter = "Hangar files|*.ini",
                FileName = System.IO.Path.GetFileName(this.ViewModel.HangarBaseName + ".ini"),
                InitialDirectory = AppSettings.WorkingDirectory + "FlightModels"
            };

            string fileName;

            if (dialog.ShowDialog(this) == true)
            {
                fileName = dialog.FileName;
            }
            else
            {
                return;
            }

            try
            {
                this.WriteText(fileName);
                this.ViewModel.HangarBaseName = System.IO.Path.ChangeExtension(fileName, null);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WriteText(string fileName)
        {
            var iniFile = new XwaIniFile(fileName);
            iniFile.ParseIni();
            iniFile.Read("Skins", "Skins");
            iniFile.Read("HangarObjects", "HangarObjects");
            iniFile.Read("HangarMap", "HangarMap");
            iniFile.Read("FamHangarMap", "FamHangarMap");

            ICollection<string> hangarSkinsIniList = iniFile.RetrieveLinesList("Skins");

            foreach (string line in this.ViewModel.HangarSkinsText.SplitLines(false))
            {
                hangarSkinsIniList.Add(line);
            }

            ICollection<string> hangarObjectsIniList = iniFile.RetrieveLinesList("HangarObjects");

            foreach (string line in this.ViewModel.HangarObjectsText.SplitLines(false))
            {
                hangarObjectsIniList.Add(line);
            }

            ICollection<string> hangarMapIniList;

            if (this.ViewModel.HangarModel.EndsWith("Hangar", StringComparison.OrdinalIgnoreCase))
            {
                hangarMapIniList = iniFile.RetrieveLinesList("HangarMap");
                iniFile.RetrieveLinesList("FamHangarMap");
            }
            else
            {
                iniFile.RetrieveLinesList("HangarMap");
                hangarMapIniList = iniFile.RetrieveLinesList("FamHangarMap");
            }

            foreach (string line in this.ViewModel.HangarMapText.SplitLines(false))
            {
                hangarMapIniList.Add(line);
            }

            iniFile.Save();

            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Saved.", this.Title);
        }

        private void ExportOptButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Export OPT file",
                AddExtension = true,
                DefaultExt = ".opt",
                Filter = "OPT files|*.opt",
                FileName = System.IO.Path.GetFileName(this.ViewModel.HangarBaseName) + "HangarView.opt"
            };

            if (dialog.Title != null)
            {
                if (this._defaultDirectory.TryGetValue(dialog.Title, out string directory))
                {
                    dialog.InitialDirectory = directory;
                }
            }

            string fileName;

            if (dialog.ShowDialog(this) == true)
            {
                if (dialog.Title != null)
                {
                    this._defaultDirectory[dialog.Title] = System.IO.Path.GetDirectoryName(dialog.FileName);
                }

                fileName = dialog.FileName;
            }
            else
            {
                return;
            }

            try
            {
                JeremyAnsel.Xwa.Opt.OptFile optFile = this.ViewModel.BuildOptMap();

                optFile.Save(fileName);

                Xceed.Wpf.Toolkit.MessageBox.Show(this, "Exported.", this.Title);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportObjButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Export OBJ file",
                AddExtension = true,
                DefaultExt = ".obj",
                Filter = "OBJ files|*.obj",
                FileName = System.IO.Path.GetFileName(this.ViewModel.HangarBaseName) + "HangarView.obj"
            };

            if (dialog.Title != null)
            {
                if (this._defaultDirectory.TryGetValue(dialog.Title, out string directory))
                {
                    dialog.InitialDirectory = directory;
                }
            }

            string fileName;

            if (dialog.ShowDialog(this) == true)
            {
                if (dialog.Title != null)
                {
                    this._defaultDirectory[dialog.Title] = System.IO.Path.GetDirectoryName(dialog.FileName);
                }

                fileName = dialog.FileName;
            }
            else
            {
                return;
            }

            try
            {
                JeremyAnsel.Xwa.Opt.OptFile optFile = this.ViewModel.BuildOptMap();

                OptObjConverter.Converter.OptToObj(optFile, fileName, true);

                Xceed.Wpf.Toolkit.MessageBox.Show(this, "Exported.", this.Title);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
