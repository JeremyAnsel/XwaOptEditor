using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JeremyAnsel.Xwa.Opt;
using XwaOptEditor.Messages;
using XwaOptEditor.Models;
using XwaOptEditor.Mvvm;
using XwaOptEditor.Services;

namespace XwaOptEditor.ViewModels
{
    class MainWindowViewModel : ObservableObject
    {
        private bool isImportExportScaleEnabled;

        public MainWindowViewModel()
        {
            this.isImportExportScaleEnabled = true;

            this.OptModel = new OptModel();

            this.Busy = new BusyIndicatorViewModel();

            this.HelpCommand = new DelegateCommand(this.ExecuteHelpCommand);
            this.NewCommand = new DelegateCommand(this.ExecuteNewCommand);
            this.OpenCommand = new DelegateCommand(this.ExecuteOpenCommand);
            this.SaveCommand = new DelegateCommand(this.ExecuteSaveCommand);
            this.SaveAsCommand = new DelegateCommand(this.ExecuteSaveAsCommand);

            this.ConvertAllTexturesTo32BppCommand = new DelegateCommand(this.ExecuteConvertAllTexturesTo32BppCommand);
            this.ConvertAllTexturesTo8BppCommand = new DelegateCommand(this.ExecuteConvertAllTexturesTo8BppCommand);

            this.CheckOptCommand = new DelegateCommand(this.ExecuteCheckOptCommand);
            this.CheckFlatTexturesCommand = new DelegateCommand(this.ExecuteCheckFlatTexturesCommand);

            this.ImportOptCommand = new DelegateCommand(this.ExecuteImportOptCommand);
            this.ImportObjCommand = new DelegateCommand(this.ExecuteImportObjCommand);
            this.ExportObjCommand = new DelegateCommand(this.ExecuteExportObjCommand);
            this.ImportRhinoCommand = new DelegateCommand(this.ExecuteImportRhinoCommand);
            this.ExportRhinoCommand = new DelegateCommand(this.ExecuteExportRhinoCommand);
            this.ImportAn8Command = new DelegateCommand(this.ExecuteImportAn8Command);
            this.ExportAn8Command = new DelegateCommand(this.ExecuteExportAn8Command);

            this.ScaleCommand = new DelegateCommand(this.ExecuteScaleCommand);
            this.ChangeAxesCommand = new DelegateCommand(this.ExecuteChangeAxesCommand);

            this.CommandBindings = new CommandBindingCollection();
            this.CommandBindings.Add(ApplicationCommands.Help, this.HelpCommand);
            this.CommandBindings.Add(ApplicationCommands.New, this.NewCommand);
            this.CommandBindings.Add(ApplicationCommands.Open, this.OpenCommand);
            this.CommandBindings.Add(ApplicationCommands.Save, this.SaveCommand);
            this.CommandBindings.Add(ApplicationCommands.SaveAs, this.SaveAsCommand);
        }

        public bool IsImportExportScaleEnabled
        {
            get
            {
                return this.isImportExportScaleEnabled;
            }

            set
            {
                if (value != this.isImportExportScaleEnabled)
                {
                    this.isImportExportScaleEnabled = value;
                    this.RaisePropertyChangedEvent("IsImportExportScaleEnabled");
                }
            }
        }

        public OptModel OptModel { get; private set; }

        public BusyIndicatorViewModel Busy { get; private set; }

        public ICommand HelpCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }

        public ICommand ConvertAllTexturesTo32BppCommand { get; private set; }
        public ICommand ConvertAllTexturesTo8BppCommand { get; private set; }

        public ICommand CheckOptCommand { get; private set; }
        public ICommand CheckFlatTexturesCommand { get; private set; }

        public ICommand ImportOptCommand { get; private set; }
        public ICommand ImportObjCommand { get; private set; }
        public ICommand ExportObjCommand { get; private set; }
        public ICommand ImportRhinoCommand { get; private set; }
        public ICommand ExportRhinoCommand { get; private set; }
        public ICommand ImportAn8Command { get; private set; }
        public ICommand ExportAn8Command { get; private set; }

        public ICommand ScaleCommand { get; private set; }
        public ICommand ChangeAxesCommand { get; private set; }

        public CommandBindingCollection CommandBindings { get; private set; }

        private void ExecuteHelpCommand()
        {
            Messenger.Instance.Notify(new MainViewSelectorMessage("Help"));
        }

        private void ExecuteNewCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                bool isEmpty = this.OptModel.File == null ||
                    (string.IsNullOrEmpty(this.OptModel.File.FileName) &&
                    this.OptModel.File.Meshes.Count == 0 &&
                    this.OptModel.File.Textures.Count == 0);

                if (!isEmpty)
                {
                    var message = Messenger.Instance.Notify(new MessageBoxMessage("Creating a new opt will erase the existing one.\nDo you want to continue?", "New opt", MessageBoxButton.YesNo, MessageBoxImage.Warning));

                    if (message.Result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                dispatcher(() => this.OptModel.File = null);

                var opt = new OptFile();

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.OptModel.UndoStackPush("new"));
            });
        }

        private void ExecuteOpenCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenOptFileName();

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Opening ", System.IO.Path.GetFileName(fileName), "..."));

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    var opt = OptFile.FromFile(fileName);

                    dispatcher(() => this.OptModel.File = opt);
                    dispatcher(() => this.OptModel.UndoStackPush("open " + System.IO.Path.GetFileNameWithoutExtension(fileName)));

                    if (!this.OptModel.IsPlayable)
                    {
                        Messenger.Instance.Notify(new MainViewSelectorMessage("PlayabilityMessages"));
                        Messenger.Instance.Notify(new MessageBoxMessage(fileName + "\n\n" + "This opt will not be fully playable.", "Check Opt Playability", MessageBoxButton.OK, MessageBoxImage.Warning));
                    }
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }
            });
        }

        private void ExecuteSaveCommand()
        {
            if (this.OptModel.File == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.OptModel.File.FileName))
            {
                this.ExecuteSaveAsCommand();
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var opt = this.OptModel.File;

                BusyIndicatorService.Notify(string.Concat("Saving ", System.IO.Path.GetFileName(this.OptModel.File.FileName), "..."));

                try
                {
                    opt.Save(opt.FileName);

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("save " + System.IO.Path.GetFileNameWithoutExtension(opt.FileName)));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(opt.FileName, ex));
                }
            });
        }

        private void ExecuteSaveAsCommand()
        {
            if (this.OptModel.File == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetSaveOptFileName(this.OptModel.File.FileName);

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Saving ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;

                try
                {
                    opt.Save(fileName);

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("save " + System.IO.Path.GetFileNameWithoutExtension(fileName)));

                    if (!this.OptModel.IsPlayable)
                    {
                        Messenger.Instance.Notify(new MainViewSelectorMessage("PlayabilityMessages"));
                        Messenger.Instance.Notify(new MessageBoxMessage(fileName + "\n\n" + "This opt will not be fully playable.", "Check Opt Playability", MessageBoxButton.OK, MessageBoxImage.Warning));
                    }
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }
            });
        }

        private void ExecuteConvertAllTexturesTo32BppCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenOptFileName();

                if (fileName == null)
                {
                    return;
                }

                string directory = System.IO.Path.GetDirectoryName(fileName);

                BusyIndicatorService.Notify("Converting all textures to 32 bpp...");

                var message = Messenger.Instance.Notify(new MessageBoxMessage(string.Concat("The textures of all OPTs in \"", directory, "\" will be converted to 32 bpp.\nDo you want to continue?"), "Converting textures", MessageBoxButton.YesNo, MessageBoxImage.Warning));

                if (message.Result != MessageBoxResult.Yes)
                {
                    return;
                }

                foreach (string file in System.IO.Directory.GetFiles(directory, "*.opt"))
                {
                    BusyIndicatorService.Notify(string.Concat("Converting ", System.IO.Path.GetFileName(file), " to 32 bpp..."));

                    OptFile opt = null;

                    try
                    {
                        opt = OptFile.FromFile(file);
                    }
                    catch (System.IO.InvalidDataException)
                    {
                        continue;
                    }

                    if (opt.TexturesBitsPerPixel == 32)
                    {
                        continue;
                    }

                    opt.ConvertTextures8To32();
                    opt.Save(opt.FileName);
                }

                BusyIndicatorService.Notify("Converting all textures to 32 bpp completed.");

                Messenger.Instance.Notify(new MessageBoxMessage("Converting all textures to 32 bpp completed.", "Converting textures"));
            });
        }

        private void ExecuteConvertAllTexturesTo8BppCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenOptFileName();

                if (fileName == null)
                {
                    return;
                }

                string directory = System.IO.Path.GetDirectoryName(fileName);

                BusyIndicatorService.Notify("Converting all textures to 8 bpp...");

                var message = Messenger.Instance.Notify(new MessageBoxMessage(string.Concat("The textures of all OPTs in \"", directory, "\" will be converted to 8 bpp.\nDo you want to continue?"), "Converting textures", MessageBoxButton.YesNo, MessageBoxImage.Warning));

                if (message.Result != MessageBoxResult.Yes)
                {
                    return;
                }

                foreach (string file in System.IO.Directory.GetFiles(directory, "*.opt"))
                {
                    BusyIndicatorService.Notify(string.Concat("Converting ", System.IO.Path.GetFileName(file), " to 8 bpp..."));

                    OptFile opt = null;

                    try
                    {
                        opt = OptFile.FromFile(file);
                    }
                    catch (System.IO.InvalidDataException)
                    {
                        continue;
                    }

                    if (opt.TexturesBitsPerPixel == 8)
                    {
                        continue;
                    }

                    opt.ConvertTextures32To8();
                    opt.Save(opt.FileName);
                }

                BusyIndicatorService.Notify("Converting all textures to 8 bpp completed.");

                Messenger.Instance.Notify(new MessageBoxMessage("Converting all textures to 8 bpp completed.", "Converting textures"));
            });
        }

        private void ExecuteCheckOptCommand()
        {
            Messenger.Instance.Notify(new MainViewSelectorMessage("PlayabilityMessages"));

            if (!this.OptModel.IsPlayable)
            {
                Messenger.Instance.Notify(new MessageBoxMessage("This opt will not be fully playable.", "Check Opt Playability", MessageBoxButton.OK, MessageBoxImage.Warning));
            }
        }

        private void ExecuteCheckFlatTexturesCommand()
        {
            var flatTextures = this.OptModel.File.CheckFlatTextures(false);

            if (flatTextures.Count == 0)
            {
                return;
            }

            string text = string.Join("\n", flatTextures);

            var message = Messenger.Instance.Notify(new MessageBoxMessage("This opt contains flat textures.\nDo you want to remove them?\n\n" + text, "Check Flat Textures", MessageBoxButton.YesNo, MessageBoxImage.Warning));

            if (message.Result != MessageBoxResult.Yes)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                BusyIndicatorService.Notify("Removing flat textures ...");

                var opt = this.OptModel.File;

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    opt.CheckFlatTextures(true);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(ex));
                }

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.OptModel.UndoStackPush("remove flat textures"));
            });
        }

        private void ExecuteImportOptCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenOptFileName();

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Importing ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    var import = OptFile.FromFile(fileName);
                    string importName = System.IO.Path.GetFileNameWithoutExtension(fileName) + "_";

                    foreach (var faceGroup in import.Meshes.SelectMany(t => t.Lods).SelectMany(t => t.FaceGroups))
                    {
                        var textures = faceGroup.Textures.ToList();
                        faceGroup.Textures.Clear();

                        foreach (var texture in textures)
                        {
                            faceGroup.Textures.Add(texture.StartsWith(importName, StringComparison.Ordinal) ? texture : (importName + texture));
                        }
                    }

                    foreach (var texture in import.Textures.Values)
                    {
                        texture.Name = texture.Name.StartsWith(importName, StringComparison.Ordinal) ? texture.Name : (importName + texture.Name);
                    }

                    foreach (var texture in import.Textures.Values)
                    {
                        opt.Textures[texture.Name] = texture;
                    }

                    foreach (var mesh in import.Meshes)
                    {
                        opt.Meshes.Add(mesh);
                    }
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.OptModel.UndoStackPush("import " + System.IO.Path.GetFileName(fileName)));
            });
        }

        private void ExecuteImportObjCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenObjFileName();

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Importing ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = this.IsImportExportScaleEnabled;

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    var import = OptObjConverter.Converter.ObjToOpt(fileName, scale);

                    foreach (var texture in import.Textures.Values)
                    {
                        opt.Textures[texture.Name] = texture;
                    }

                    foreach (var mesh in import.Meshes)
                    {
                        opt.Meshes.Add(mesh);
                    }
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.OptModel.UndoStackPush("import " + System.IO.Path.GetFileName(fileName)));
            });
        }

        private void ExecuteExportObjCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetSaveObjFileName(System.IO.Path.ChangeExtension(this.OptModel.File.FileName, "obj"));

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Exporting ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = this.IsImportExportScaleEnabled;

                try
                {
                    OptObjConverter.Converter.OptToObj(opt, fileName, scale, BusyIndicatorService.Notify);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }
            });
        }

        private void ExecuteImportRhinoCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenRhinoFileName();

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Importing ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = this.IsImportExportScaleEnabled;

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    var import = OptRhinoConverter.Converter.RhinoToOpt(fileName, scale);

                    foreach (var texture in import.Textures.Values)
                    {
                        opt.Textures[texture.Name] = texture;
                    }

                    foreach (var mesh in import.Meshes)
                    {
                        opt.Meshes.Add(mesh);
                    }
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.OptModel.UndoStackPush("import " + System.IO.Path.GetFileName(fileName)));
            });
        }

        private void ExecuteExportRhinoCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetSaveRhinoFileName(System.IO.Path.ChangeExtension(this.OptModel.File.FileName, "3dm"));

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Exporting ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = this.IsImportExportScaleEnabled;

                try
                {
                    OptRhinoConverter.Converter.OptToRhino(opt, fileName, scale, BusyIndicatorService.Notify);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }
            });
        }

        private void ExecuteImportAn8Command()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenAn8FileName();

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Importing ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = this.IsImportExportScaleEnabled;

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    var import = OptAn8Converter.Converter.An8ToOpt(fileName, scale);

                    foreach (var texture in import.Textures.Values)
                    {
                        opt.Textures[texture.Name] = texture;
                    }

                    foreach (var mesh in import.Meshes)
                    {
                        opt.Meshes.Add(mesh);
                    }
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.OptModel.UndoStackPush("import " + System.IO.Path.GetFileName(fileName)));
            });
        }

        private void ExecuteExportAn8Command()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetSaveAn8FileName(System.IO.Path.ChangeExtension(this.OptModel.File.FileName, "an8"));

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Exporting ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = this.IsImportExportScaleEnabled;

                try
                {
                    OptAn8Converter.Converter.OptToAn8(opt, fileName, scale, BusyIndicatorService.Notify);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }
            });
        }

        private void ExecuteScaleCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var opt = this.OptModel.File;
                var size = opt.SpanSize.Scale(OptFile.ScaleFactor, OptFile.ScaleFactor, OptFile.ScaleFactor);

                var message = Messenger.Instance.Notify(
                    new ScaleFactorMessage
                    {
                        SizeX = size.X,
                        SizeY = size.Y,
                        SizeZ = size.Z
                    });

                if (!message.Changed)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat(message.ScaleType, "..."));

                opt.Scale(message.ScaleX, message.ScaleY, message.ScaleZ);

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.OptModel.UndoStackPush("scale"));
            });
        }

        private void ExecuteChangeAxesCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var message = Messenger.Instance.Notify(new ChangeAxesMessage());

                if (!message.Changed)
                {
                    return;
                }

                BusyIndicatorService.Notify("Changes axes...");

                var opt = this.OptModel.File;

                opt.ChangeAxes(message.AxisX, message.AxisY, message.AxisZ);

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.OptModel.UndoStackPush("change axes"));
            });
        }
    }
}
