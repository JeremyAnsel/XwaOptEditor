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
    class TexturesViewModel : ObservableObject
    {
        private OptModel optModel;

        public TexturesViewModel()
        {
            this.SaveAsCommand = new DelegateCommandOf<Texture>(this.ExecuteSaveAsCommand);
            this.SaveAsColorCommand = new DelegateCommandOf<Texture>(this.ExecuteSaveAsColorCommand);
            this.SaveAsAlphaCommand = new DelegateCommandOf<Texture>(this.ExecuteSaveAsAlphaCommand);
            this.ReplaceMapCommand = new DelegateCommandOf<Texture>(this.ExecuteReplaceMapCommand);
            this.ReplaceAlphaMapCommand = new DelegateCommandOf<Texture>(this.ExecuteReplaceAlphaMapCommand);

            this.GenerateAllMipmapsCommand = new DelegateCommand(this.ExecuteGenerateAllMipmapsCommand);
            this.ConvertAllTo8BitsCommand = new DelegateCommand(this.ExecuteConvertAllTo8BitsCommand);
            this.ConvertAllTo32BitsCommand = new DelegateCommand(this.ExecuteConvertAllTo32BitsCommand);
            this.CompactCommand = new DelegateCommand(this.ExecuteCompactCommand);
            this.GenerateNamesCommand = new DelegateCommand(this.ExecuteGenerateNamesCommand);
        }

        public OptModel OptModel
        {
            get
            {
                return this.optModel;
            }

            set
            {
                if (this.optModel != value)
                {
                    this.optModel = value;
                    this.RaisePropertyChangedEvent("OptModel");
                }
            }
        }

        public ICommand SaveAsCommand { get; private set; }

        public ICommand SaveAsColorCommand { get; private set; }

        public ICommand SaveAsAlphaCommand { get; private set; }

        public ICommand ReplaceMapCommand { get; private set; }

        public ICommand ReplaceAlphaMapCommand { get; private set; }

        public ICommand GenerateAllMipmapsCommand { get; private set; }

        public ICommand ConvertAllTo8BitsCommand { get; private set; }

        public ICommand ConvertAllTo32BitsCommand { get; private set; }

        public ICommand CompactCommand { get; private set; }

        public ICommand GenerateNamesCommand { get; private set; }

        private void ExecuteSaveAsCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            BusyIndicatorService.Run(() =>
            {
                string fileName = FileDialogService.GetSaveTextureFileName(texture.Name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    texture.Save(fileName);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(texture.Name, ex));
                }
            });
        }

        private void ExecuteSaveAsColorCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            BusyIndicatorService.Run(() =>
            {
                string name = texture.Name + "_color";
                string fileName = FileDialogService.GetSaveTextureFileName(name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    texture.SaveColorMap(fileName);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(name, ex));
                }
            });
        }

        private void ExecuteSaveAsAlphaCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            if (!texture.HasAlpha)
            {
                return;
            }

            BusyIndicatorService.Run(() =>
            {
                string name = texture.Name + "_alpha";
                string fileName = FileDialogService.GetSaveTextureFileName(name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    texture.SaveAlphaMap(fileName);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(name, ex));
                }
            });
        }

        private void ExecuteReplaceMapCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenTextureFileName(texture.Name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    var newTexture = Texture.FromFile(fileName);
                    newTexture.Name = texture.Name;

                    int bpp = newTexture.BitsPerPixel;

                    newTexture.GenerateMipmaps();

                    if (bpp == 8)
                    {
                        newTexture.Convert32To8();
                    }

                    this.OptModel.File.Textures[newTexture.Name] = newTexture;

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(texture.Name, ex));
                }
            });
        }

        private void ExecuteReplaceAlphaMapCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                string name = texture.Name + "_alpha";
                string fileName = FileDialogService.GetOpenTextureFileName(name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    this.OptModel.File.Textures[texture.Name].SetAlphaMap(fileName);

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(name, ex));
                }
            });
        }

        private void ExecuteGenerateAllMipmapsCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    BusyIndicatorService.Notify("Generating all mipmaps...");

                    this.OptModel.File.GenerateTexturesMipmaps();

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Generate all mipmaps.", ex));
                }
            });
        }

        private void ExecuteConvertAllTo8BitsCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                bool doConvertion = this.OptModel.File.CanTexturesBeConvertedWithoutLoss();

                if (!doConvertion)
                {
                    var result = Messenger.Instance.Notify(new MessageBoxMessage(
                        "All textures can not be converted without loss. Do you want to continue?",
                        "Convert textures to 8 bits",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning));

                    doConvertion = result.Result == MessageBoxResult.Yes;
                }

                if (!doConvertion)
                {
                    return;
                }

                try
                {
                    BusyIndicatorService.Notify("Converting textures to 8 bits...");

                    this.OptModel.File.ConvertTextures32To8();

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Convert textures to 8 bits.", ex));
                }
            });
        }

        private void ExecuteConvertAllTo32BitsCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    BusyIndicatorService.Notify("Converting textures to 32 bits...");

                    this.OptModel.File.ConvertTextures8To32();

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Convert textures to 32 bits.", ex));
                }
            });
        }

        private void ExecuteCompactCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    BusyIndicatorService.Notify("Compacting textures...");

                    this.OptModel.File.CompactTextures();

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Compact textures.", ex));
                }
            });
        }

        private void ExecuteGenerateNamesCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    BusyIndicatorService.Notify("Generating textures names...");

                    this.OptModel.File.GenerateTexturesNames();

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Generate textures names.", ex));
                }
            });
        }
    }
}
