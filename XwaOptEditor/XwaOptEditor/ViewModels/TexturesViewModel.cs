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
            this.SaveAsIllumCommand = new DelegateCommandOf<Texture>(this.ExecuteSaveAsIllumCommand);
            this.ReplaceMapCommand = new DelegateCommandOf<Texture>(this.ExecuteReplaceMapCommand);
            this.ReplaceAlphaMapCommand = new DelegateCommandOf<Texture>(this.ExecuteReplaceAlphaMapCommand);
            this.ReplaceIllumMapCommand = new DelegateCommandOf<Texture>(this.ExecuteReplaceIllumMapCommand);
            this.RenameTextureCommand = new DelegateCommandOf<Texture>(this.ExecuteRenameTextureCommand);

            this.UpCommand = new DelegateCommandOfList<KeyValuePair<string, Texture>>(this.ExecuteUpCommand);
            this.DownCommand = new DelegateCommandOfList<KeyValuePair<string, Texture>>(this.ExecuteDownCommand);

            this.GenerateSelectedMipmapsCommand = new DelegateCommandOfList<KeyValuePair<string, Texture>>(this.ExecuteGenerateSelectedMipmapsCommand);
            this.ConvertSelectedTo8BitsCommand = new DelegateCommandOfList<KeyValuePair<string, Texture>>(this.ExecuteConvertSelectedTo8BitsCommand);
            this.ConvertSelectedTo32BitsCommand = new DelegateCommandOfList<KeyValuePair<string, Texture>>(this.ExecuteConvertSelectedTo32BitsCommand);

            this.GenerateAllMipmapsCommand = new DelegateCommand(this.ExecuteGenerateAllMipmapsCommand);
            this.ConvertAllTo8BitsCommand = new DelegateCommand(this.ExecuteConvertAllTo8BitsCommand);
            this.ConvertAllTo32BitsCommand = new DelegateCommand(this.ExecuteConvertAllTo32BitsCommand);
            this.CompactCommand = new DelegateCommand(this.ExecuteCompactCommand);
            this.RemoveUnusedCommand = new DelegateCommand(this.ExecuteRemoveUnusedCommand);
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

        public ICommand SaveAsIllumCommand { get; private set; }

        public ICommand ReplaceMapCommand { get; private set; }

        public ICommand ReplaceAlphaMapCommand { get; private set; }

        public ICommand ReplaceIllumMapCommand { get; private set; }

        public ICommand RenameTextureCommand { get; private set; }

        public ICommand UpCommand { get; private set; }

        public ICommand DownCommand { get; private set; }

        public ICommand GenerateSelectedMipmapsCommand { get; private set; }

        public ICommand ConvertSelectedTo8BitsCommand { get; private set; }

        public ICommand ConvertSelectedTo32BitsCommand { get; private set; }

        public ICommand GenerateAllMipmapsCommand { get; private set; }

        public ICommand ConvertAllTo8BitsCommand { get; private set; }

        public ICommand ConvertAllTo32BitsCommand { get; private set; }

        public ICommand CompactCommand { get; private set; }

        public ICommand RemoveUnusedCommand { get; private set; }

        public ICommand GenerateNamesCommand { get; private set; }

        private void ExecuteSaveAsCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetSaveTextureFileName("Save texture", texture.Name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    texture.Save(fileName);

                    dispatcher(() => this.OptModel.UndoStackPush("save " + System.IO.Path.GetFileName(fileName)));
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

            BusyIndicatorService.Run(dispatcher =>
            {
                string name = texture.Name + "_color";
                string fileName = FileDialogService.GetSaveTextureFileName("Save texture", name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    texture.SaveColorMap(fileName);

                    dispatcher(() => this.OptModel.UndoStackPush("save " + System.IO.Path.GetFileName(fileName)));
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

            BusyIndicatorService.Run(dispatcher =>
            {
                string name = texture.Name + "_alpha";
                string fileName = FileDialogService.GetSaveTextureFileName("Save texture", name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    texture.SaveAlphaMap(fileName);

                    dispatcher(() => this.OptModel.UndoStackPush("save " + System.IO.Path.GetFileName(fileName)));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(name, ex));
                }
            });
        }

        private void ExecuteSaveAsIllumCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            if (!texture.IsIlluminated)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                string name = texture.Name + "_illum";
                string fileName = FileDialogService.GetSaveTextureFileName("Save texture", name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    texture.SaveIllumMap(fileName);

                    dispatcher(() => this.OptModel.UndoStackPush("save " + System.IO.Path.GetFileName(fileName)));
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
                string fileName = FileDialogService.GetOpenTextureFileName("Open texture", texture.Name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    var newTexture = Texture.FromFile(fileName);
                    newTexture.Name = texture.Name;

                    int bpp = newTexture.BitsPerPixel;

                    if (bpp == 32)
                    {
                        newTexture.GenerateMipmaps();
                    }

                    this.OptModel.File.Textures[newTexture.Name] = newTexture;

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("replace " + texture.Name));
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
                string fileName = FileDialogService.GetOpenTextureFileName("Open texture", name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    int bpp = texture.BitsPerPixel;
                    int mipmapsCount = texture.MipmapsCount;

                    texture.RemoveMipmaps();
                    texture.SetAlphaMap(fileName);

                    if (mipmapsCount > 1)
                    {
                        texture.GenerateMipmaps();

                        if (bpp == 8)
                        {
                            texture.Convert32To8();
                        }
                    }

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("replace " + texture.Name + " alpha"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(name, ex));
                }
            });
        }

        private void ExecuteReplaceIllumMapCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                string name = texture.Name + "_illum";
                string fileName = FileDialogService.GetOpenTextureFileName("Open texture", name);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    int bpp = texture.BitsPerPixel;
                    int mipmapsCount = texture.MipmapsCount;

                    texture.Convert8To32();
                    texture.RemoveMipmaps();
                    texture.SetIllumMap(fileName);

                    if (mipmapsCount > 1)
                    {
                        texture.GenerateMipmaps();
                    }

                    if (bpp == 8)
                    {
                        texture.Convert32To8();
                    }

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("replace " + texture.Name + " illum"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(name, ex));
                }
            });
        }

        private void ExecuteRenameTextureCommand(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            var opt = this.OptModel.File;
            string oldName = texture.Name;
            string newName = Microsoft.VisualBasic.Interaction.InputBox("New name for " + oldName, "New texture name");

            if (string.IsNullOrEmpty(newName))
            {
                return;
            }

            if (opt.Textures.Keys.Contains(newName, StringComparer.OrdinalIgnoreCase))
            {
                Messenger.Instance.Notify(new MessageBoxMessage(
                    newName + " already exists",
                    "Rename texture",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning));

                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    foreach (Mesh mesh in opt.Meshes)
                    {
                        foreach (var lod in mesh.Lods)
                        {
                            foreach (var faceGroup in lod.FaceGroups)
                            {
                                for (int markings = 0; markings < faceGroup.Textures.Count; markings++)
                                {
                                    string name = faceGroup.Textures[markings];

                                    if (string.Equals(name, oldName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        faceGroup.Textures[markings] = newName;
                                    }
                                }
                            }
                        }
                    }

                    opt.Textures.Remove(oldName);
                    texture.Name = newName;
                    opt.Textures.Add(texture.Name, texture);

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("rename " + oldName));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Rename " + oldName + " to " + newName, "Rename texture", ex));
                }
            });
        }

        private void SwitchTextures(string key1, string key2)
        {
            var dictionary = this.OptModel.File.Textures;

            Texture tex1 = dictionary[key1];
            dictionary[key1] = null;
            Texture tex2 = dictionary[key2];
            dictionary[key2] = null;

            dictionary[key1] = tex2;
            tex2.Name = key1;
            dictionary[key2] = tex1;
            tex1.Name = key2;

            foreach (var faceGroup in this.OptModel.File.Meshes
                .SelectMany(t => t.Lods)
                .SelectMany(t => t.FaceGroups))
            {
                for (int i = 0; i < faceGroup.Textures.Count; i++)
                {
                    string key = faceGroup.Textures[i];

                    if (key == key1)
                    {
                        faceGroup.Textures[i] = key2;
                    }
                    else if (key == key2)
                    {
                        faceGroup.Textures[i] = key1;
                    }
                }
            }
        }

        private void ExecuteUpCommand(IList<KeyValuePair<string, Texture>> textures)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    BusyIndicatorService.Notify("Move up textures...");

                    var selectedItems = textures.ToList();
                    var keys = this.OptModel.File.Textures.Keys.ToList();

                    for (int selectedIndex = 0; selectedIndex < selectedItems.Count; selectedIndex++)
                    {
                        var selectedItem = selectedItems[selectedIndex];

                        int index = keys.IndexOf(selectedItem.Key);

                        if (index < 1)
                        {
                            continue;
                        }

                        string key1 = keys[index - 1];
                        string key2 = keys[index];
                        this.SwitchTextures(key1, key2);
                    }

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("move up textures"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Move up textures.", ex));
                }
            });
        }

        private void ExecuteDownCommand(IList<KeyValuePair<string, Texture>> textures)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    BusyIndicatorService.Notify("Move down textures...");

                    var selectedItems = textures.ToList();
                    var keys = this.OptModel.File.Textures.Keys.ToList();

                    for (int selectedIndex = selectedItems.Count - 1; selectedIndex >= 0; selectedIndex--)
                    {
                        var selectedItem = selectedItems[selectedIndex];

                        int index = keys.IndexOf(selectedItem.Key);

                        if (index >= keys.Count - 1)
                        {
                            continue;
                        }

                        string key1 = keys[index];
                        string key2 = keys[index + 1];
                        this.SwitchTextures(key1, key2);
                    }

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("move down textures"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Move down textures.", ex));
                }
            });
        }

        private void ExecuteGenerateSelectedMipmapsCommand(IList<KeyValuePair<string, Texture>> textures)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    var selectedItems = textures.ToList();

                    BusyIndicatorService.Notify("Generating selected mipmaps...");

                    selectedItems
                        .AsParallel()
                        .ForAll(t => t.Value.GenerateMipmaps());

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("generate mipmaps"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Generate selected mipmaps.", ex));
                }
            });
        }

        private void ExecuteConvertSelectedTo8BitsCommand(IList<KeyValuePair<string, Texture>> textures)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var selectedItems = textures.ToList();

                bool doConvertion = true;

                foreach (var item in selectedItems)
                {
                    if (!item.Value.CanBeConvertedWithoutLoss())
                    {
                        doConvertion = false;
                        break;
                    }
                }

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

                    selectedItems
                        .AsParallel()
                        .ForAll(t => t.Value.Convert32To8());

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("convert to 8 bpp"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Convert textures to 8 bits.", ex));
                }
            });
        }

        private void ExecuteConvertSelectedTo32BitsCommand(IList<KeyValuePair<string, Texture>> textures)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    var selectedItems = textures.ToList();

                    BusyIndicatorService.Notify("Converting textures to 32 bits...");

                    selectedItems
                        .AsParallel()
                        .ForAll(t => t.Value.Convert8To32());

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("convert to 32 bpp"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Convert textures to 32 bits.", ex));
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
                    dispatcher(() => this.OptModel.UndoStackPush("generate mipmaps"));
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
                    dispatcher(() => this.OptModel.UndoStackPush("convert to 8 bpp"));
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
                    dispatcher(() => this.OptModel.UndoStackPush("convert to 32 bpp"));
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
                    dispatcher(() => this.OptModel.UndoStackPush("compact textures"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Compact textures.", ex));
                }
            });
        }

        private void ExecuteRemoveUnusedCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                try
                {
                    BusyIndicatorService.Notify("Removing unused textures...");

                    this.OptModel.File.RemoveUnusedTextures();

                    dispatcher(() => this.OptModel.File = this.OptModel.File);
                    dispatcher(() => this.OptModel.UndoStackPush("remove textures"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Remove unused textures.", ex));
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
                    dispatcher(() => this.OptModel.UndoStackPush("generate textures names"));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage("Generate textures names.", ex));
                }
            });
        }
    }
}
