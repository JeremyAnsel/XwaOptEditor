﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;
using XwaOptEditor.Extensions;
using XwaOptEditor.Messages;
using XwaOptEditor.Models;
using XwaOptEditor.Mvvm;
using XwaOptEditor.Services;

namespace XwaOptEditor.ViewModels
{
    class EditorViewModel : ObservableObject
    {
        private object clipboardObject;

        private OptModel optModel;

        private int modelVersion = 0;

        private bool modelShowSolid = true;

        private bool modelShowWireframe = false;

        private float modelDistance = 0.001f;

        private bool showHitzones = true;

        private bool showHardpoints = true;

        private bool showHardpointsText = false;

        private bool showEngineGlows = true;

        private bool showNormals = false;

        private bool showCenter = true;

        public EditorViewModel()
        {
            this.CurrentMeshes = new SelectableCollection<Mesh>();
            this.CurrentLods = new SelectableCollection<MeshLod>();
            this.CurrentFaceGroups = new SelectableCollection<FaceGroup>();
            this.CurrentTextureNames = new StringCollection();

            this.CurrentMeshes.SelectedItemChanged += (sender, e) =>
            {
                if (this.CurrentMeshes.SelectedItem == null)
                {
                    this.CurrentLods.LoadItems(null);
                }
                else
                {
                    this.CurrentLods.LoadItems(this.CurrentMeshes.SelectedItem.Lods);
                }
            };

            this.CurrentLods.SelectedItemChanged += (sender, e) =>
            {
                if (this.CurrentLods.SelectedItem == null)
                {
                    this.CurrentFaceGroups.LoadItems(null);
                }
                else
                {
                    this.CurrentFaceGroups.LoadItems(this.CurrentLods.SelectedItem.FaceGroups);
                }
            };

            this.CurrentFaceGroups.SelectedItemChanged += (sender, e) =>
            {
                if (this.CurrentFaceGroups.SelectedItem == null)
                {
                    this.CurrentTextureNames.LoadItems(null);
                }
                else
                {
                    this.CurrentTextureNames.LoadItems(this.CurrentFaceGroups.SelectedItem.Textures);
                }
            };

            this.OptModel = new OptModel();

            this.NewMeshCommand = new DelegateCommand(this.ExecuteNewMeshCommand);
            this.DeleteMeshesCommand = new DelegateCommandOfList<Mesh>(this.ExecuteDeleteMeshesCommand);
            this.UpMeshesCommand = new DelegateCommandOfList<Mesh>(this.ExecuteUpMeshesCommand);
            this.DownMeshesCommand = new DelegateCommandOfList<Mesh>(this.ExecuteDownMeshesCommand);
            this.SplitMeshesCommand = new DelegateCommandOfList<Mesh>(this.ExecuteSplitMeshesCommand);
            this.MergeMeshesCommand = new DelegateCommandOfList<Mesh>(this.ExecuteMergeMeshesCommand);
            this.MoveMeshesCommand = new DelegateCommandOfList<Mesh>(this.ExecuteMoveMeshesCommand);
            this.RotateMeshCommand = new DelegateCommandOf<Mesh>(this.ExecuteRotateMeshCommand);
            this.DuplicateMeshesCommand = new DelegateCommandOfList<Mesh>(this.ExecuteDuplicateMeshesCommand);
            this.ComputeHitzonesCommand = new DelegateCommand(this.ExecuteComputeHitzonesCommand);
            this.ExportMeshObjCommand = new DelegateCommandOfList<Mesh>(this.ExecuteExportMeshObjCommand);
            this.ExportMeshRhinoCommand = new DelegateCommandOfList<Mesh>(this.ExecuteExportMeshRhinoCommand);
            this.ExportMeshAn8Command = new DelegateCommandOfList<Mesh>(this.ExecuteExportMeshAn8Command);
            this.ReplaceMeshObjCommand = new DelegateCommandOf<Mesh>(this.ExecuteReplaceMeshObjCommand);
            this.ReplaceMeshRhinoCommand = new DelegateCommandOf<Mesh>(this.ExecuteReplaceMeshRhinoCommand);
            this.ReplaceMeshAn8Command = new DelegateCommandOf<Mesh>(this.ExecuteReplaceMeshAn8Command);

            this.NewLodCommand = new DelegateCommand(this.ExecuteNewLodCommand);
            this.DeleteLodsCommand = new DelegateCommandOfList<MeshLod>(this.ExecuteDeleteLodsCommand);
            this.UpLodsCommand = new DelegateCommandOfList<MeshLod>(this.ExecuteUpLodsCommand);
            this.DownLodsCommand = new DelegateCommandOfList<MeshLod>(this.ExecuteDownLodsCommand);
            this.SortLodsCommand = new DelegateCommand(this.ExecuteSortLodsCommand);
            this.SplitLodsCommand = new DelegateCommandOfList<MeshLod>(this.ExecuteSplitLodsCommand);
            this.MergeLodsCommand = new DelegateCommandOfList<MeshLod>(this.ExecuteMergeLodsCommand);

            this.ComputeHitzoneCommand = new DelegateCommand(this.ExecuteComputeHitzoneCommand);

            this.NewHardpointCommand = new DelegateCommand(this.ExecuteNewHardpointCommand);
            this.DeleteHardpointsCommand = new DelegateCommandOfList<Hardpoint>(this.ExecuteDeleteHardpointsCommand);
            this.CutHardpointsCommand = new DelegateCommandOfList<Hardpoint>(this.ExecuteCutHardpointsCommand);
            this.CopyHardpointsCommand = new DelegateCommandOfList<Hardpoint>(this.ExecuteCopyHardpointsCommand);
            this.PasteHardpointsCommand = new DelegateCommand(this.ExecutePasteHardpointsCommand);

            this.NewEngineGlowCommand = new DelegateCommand(this.ExecuteNewEngineGlowCommand);
            this.DeleteEngineGlowsCommand = new DelegateCommandOfList<EngineGlow>(this.ExecuteDeleteEngineGlowsCommand);
            this.CutEngineGlowsCommand = new DelegateCommandOfList<EngineGlow>(this.ExecuteCutEngineGlowsCommand);
            this.CopyEngineGlowsCommand = new DelegateCommandOfList<EngineGlow>(this.ExecuteCopyEngineGlowsCommand);
            this.PasteEngineGlowsCommand = new DelegateCommand(this.ExecutePasteEngineGlowsCommand);

            this.AddTextureNameCommand = new DelegateCommand(this.ExecuteAddTextureNameCommand);
            this.BrowseTextureNameCommand = new DelegateCommand(this.ExecuteBrowseTextureNameCommand);
            this.DeleteTextureNamesCommand = new DelegateCommandOfList<StringWrapper>(this.ExecuteDeleteTextureNamesCommand);
            this.ClearTextureNamesCommand = new DelegateCommand(this.ExecuteClearTextureNamesCommand);
            this.UpTextureNamesCommand = new DelegateCommandOf<int>(this.ExecuteUpTextureNamesCommand);
            this.DownTextureNamesCommand = new DelegateCommandOf<int>(this.ExecuteDownTextureNamesCommand);

            this.SelectMeshCommand = new DelegateCommandOf<Tuple<MeshLodFace, Point3D>>(this.ExecuteSelectMeshCommand);
            this.AddMeshToSelectionCommand = new DelegateCommandOf<Tuple<MeshLodFace, Point3D>>(this.ExecuteAddMeshToSelectionCommand);
            this.AddHardpointCommand = new DelegateCommandOf<Tuple<MeshLodFace, Point3D>>(this.ExecuteAddHardpointCommand);
            this.AddEngineGlowCommand = new DelegateCommandOf<Tuple<MeshLodFace, Point3D>>(this.ExecuteAddEngineGlowCommand);
            this.CopyPointCommand = new DelegateCommandOf<Tuple<MeshLodFace, Point3D>>(this.ExecuteCopyPointCommand);

            Func<object, object> listBoxSelectedItemsSelector = sender => ((System.Windows.Controls.ListBox)sender).SelectedItems;

            this.HardpointsCutCopyCommandBindings = new CommandBindingCollection();
            this.HardpointsCutCopyCommandBindings.Add(ApplicationCommands.Cut, this.CutHardpointsCommand, listBoxSelectedItemsSelector);
            this.HardpointsCutCopyCommandBindings.Add(ApplicationCommands.Copy, this.CopyHardpointsCommand, listBoxSelectedItemsSelector);
            this.HardpointsPasteCommandBindings = new CommandBindingCollection();
            this.HardpointsPasteCommandBindings.Add(ApplicationCommands.Paste, this.PasteHardpointsCommand);

            this.EngineGlowsCutCopyCommandBindings = new CommandBindingCollection();
            this.EngineGlowsCutCopyCommandBindings.Add(ApplicationCommands.Cut, this.CutEngineGlowsCommand, listBoxSelectedItemsSelector);
            this.EngineGlowsCutCopyCommandBindings.Add(ApplicationCommands.Copy, this.CopyEngineGlowsCommand, listBoxSelectedItemsSelector);
            this.EngineGlowsPasteCommandBindings = new CommandBindingCollection();
            this.EngineGlowsPasteCommandBindings.Add(ApplicationCommands.Paste, this.PasteEngineGlowsCommand);
        }

        public ICommand NewMeshCommand { get; private set; }

        public ICommand DeleteMeshesCommand { get; private set; }

        public ICommand UpMeshesCommand { get; private set; }

        public ICommand DownMeshesCommand { get; private set; }

        public ICommand SplitMeshesCommand { get; private set; }

        public ICommand MergeMeshesCommand { get; private set; }

        public ICommand MoveMeshesCommand { get; private set; }

        public ICommand RotateMeshCommand { get; private set; }

        public ICommand DuplicateMeshesCommand { get; private set; }

        public ICommand ComputeHitzonesCommand { get; private set; }

        public ICommand ExportMeshObjCommand { get; private set; }

        public ICommand ExportMeshRhinoCommand { get; private set; }

        public ICommand ExportMeshAn8Command { get; private set; }

        public ICommand ReplaceMeshObjCommand { get; private set; }

        public ICommand ReplaceMeshRhinoCommand { get; private set; }

        public ICommand ReplaceMeshAn8Command { get; private set; }

        public ICommand NewLodCommand { get; private set; }

        public ICommand DeleteLodsCommand { get; private set; }

        public ICommand UpLodsCommand { get; private set; }

        public ICommand DownLodsCommand { get; private set; }

        public ICommand SortLodsCommand { get; private set; }

        public ICommand SplitLodsCommand { get; private set; }

        public ICommand MergeLodsCommand { get; private set; }

        public ICommand ComputeHitzoneCommand { get; private set; }

        public ICommand NewHardpointCommand { get; private set; }

        public ICommand DeleteHardpointsCommand { get; private set; }

        public ICommand CutHardpointsCommand { get; private set; }

        public ICommand CopyHardpointsCommand { get; private set; }

        public ICommand PasteHardpointsCommand { get; private set; }

        public ICommand NewEngineGlowCommand { get; private set; }

        public ICommand DeleteEngineGlowsCommand { get; private set; }

        public ICommand CutEngineGlowsCommand { get; private set; }

        public ICommand CopyEngineGlowsCommand { get; private set; }

        public ICommand PasteEngineGlowsCommand { get; private set; }

        public ICommand AddTextureNameCommand { get; private set; }

        public ICommand BrowseTextureNameCommand { get; private set; }

        public ICommand DeleteTextureNamesCommand { get; private set; }

        public ICommand ClearTextureNamesCommand { get; private set; }

        public ICommand UpTextureNamesCommand { get; private set; }

        public ICommand DownTextureNamesCommand { get; private set; }

        public ICommand SelectMeshCommand { get; private set; }

        public ICommand AddMeshToSelectionCommand { get; private set; }

        public ICommand AddHardpointCommand { get; private set; }

        public ICommand AddEngineGlowCommand { get; private set; }

        public ICommand CopyPointCommand { get; private set; }

        public CommandBindingCollection HardpointsCutCopyCommandBindings { get; private set; }

        public CommandBindingCollection HardpointsPasteCommandBindings { get; private set; }

        public CommandBindingCollection EngineGlowsCutCopyCommandBindings { get; private set; }

        public CommandBindingCollection EngineGlowsPasteCommandBindings { get; private set; }

        public SelectableCollection<Mesh> CurrentMeshes { get; private set; }

        public SelectableCollection<MeshLod> CurrentLods { get; private set; }

        public SelectableCollection<FaceGroup> CurrentFaceGroups { get; private set; }

        public StringCollection CurrentTextureNames { get; private set; }

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
                    if (this.optModel != null)
                    {
                        this.optModel.PropertyChanged -= this.OnOptModelPropertyChanged;
                    }

                    this.optModel = value;

                    if (this.optModel != null)
                    {
                        this.optModel.PropertyChanged += this.OnOptModelPropertyChanged;
                    }

                    this.RaisePropertyChangedEvent("OptModel");
                }
            }
        }

        public int ModelVersion
        {
            get
            {
                return this.modelVersion;
            }

            set
            {
                int max = this.OptModel.File == null ? 0 : this.OptModel.File.MaxTextureVersion;

                if (value < 0)
                {
                    value = 0;
                }
                else if (value > max)
                {
                    value = max;
                }

                if (this.modelVersion != value)
                {
                    this.modelVersion = value;
                    this.RaisePropertyChangedEvent("ModelVersion");
                }
            }
        }

        public bool ModelShowSolid
        {
            get
            {
                return this.modelShowSolid;
            }

            set
            {
                if (this.modelShowSolid != value)
                {
                    this.modelShowSolid = value;
                    this.RaisePropertyChangedEvent("ModelShowSolid");
                }
            }
        }

        public bool ModelShowWireframe
        {
            get
            {
                return this.modelShowWireframe;
            }

            set
            {
                if (this.modelShowWireframe != value)
                {
                    this.modelShowWireframe = value;
                    this.RaisePropertyChangedEvent("ModelShowWireframe");
                }
            }
        }

        public float ModelDistance
        {
            get
            {
                return this.modelDistance;
            }

            set
            {
                if (this.modelDistance != value)
                {
                    this.modelDistance = value;
                    this.RaisePropertyChangedEvent("ModelDistance");
                }
            }
        }

        public bool ShowHitzones
        {
            get
            {
                return this.showHitzones;
            }

            set
            {
                if (this.showHitzones != value)
                {
                    this.showHitzones = value;
                    this.RaisePropertyChangedEvent("ShowHitzones");
                }
            }
        }

        public bool ShowHardpoints
        {
            get
            {
                return this.showHardpoints;
            }

            set
            {
                if (this.showHardpoints != value)
                {
                    this.showHardpoints = value;
                    this.RaisePropertyChangedEvent("ShowHardpoints");
                }
            }
        }

        public bool ShowHardpointsText
        {
            get
            {
                return this.showHardpointsText;
            }

            set
            {
                if (this.showHardpointsText != value)
                {
                    this.showHardpointsText = value;
                    this.RaisePropertyChangedEvent("ShowHardpointsText");
                }
            }
        }

        public bool ShowEngineGlows
        {
            get
            {
                return this.showEngineGlows;
            }

            set
            {
                if (this.showEngineGlows != value)
                {
                    this.showEngineGlows = value;
                    this.RaisePropertyChangedEvent("ShowEngineGlows");
                }
            }
        }

        public bool ShowNormals
        {
            get
            {
                return this.showNormals;
            }

            set
            {
                if (this.showNormals != value)
                {
                    this.showNormals = value;
                    this.RaisePropertyChangedEvent("ShowNormals");
                }
            }
        }

        public bool ShowCenter
        {
            get
            {
                return this.showCenter;
            }

            set
            {
                if (this.showCenter != value)
                {
                    this.showCenter = value;
                    this.RaisePropertyChangedEvent("ShowCenter");
                }
            }
        }

        public void UpdateModel(bool preserveView = false)
        {
            if (preserveView)
            {
                var items = this.CurrentMeshes.ToList();

                this.CurrentMeshes.Clear();
                items.ForEach(t => this.CurrentMeshes.Add(t));
            }
            else
            {
                this.OptModel.File = this.OptModel.File;
            }
        }

        private void OnOptModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "File")
            {
                if (this.OptModel.File == null)
                {
                    this.CurrentMeshes.LoadItems(null);
                }
                else
                {
                    this.CurrentMeshes.LoadItems(this.OptModel.File.Meshes);
                }
            }
        }

        private void ExecuteNewMeshCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                dispatcher(() => this.CurrentMeshes.ClearSelection());

                var mesh = new Mesh();
                mesh.Lods.Add(new MeshLod());

                this.OptModel.File.Meshes.Add(mesh);

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("new mesh"));
            });
        }

        private void ExecuteDeleteMeshesCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                dispatcher(() => this.CurrentMeshes.ClearSelection());

                foreach (var mesh in meshes)
                {
                    this.OptModel.File.Meshes.Remove(mesh);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.OptModel.UndoStackPush("delete mesh"));
            });
        }

        private void ExecuteUpMeshesCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var opt = this.OptModel.File;

                dispatcher(() => this.CurrentMeshes.ClearSelection());

                var orderedMeshes = meshes
                    .Select(mesh => new
                    {
                        Mesh = mesh,
                        Index = opt.Meshes.IndexOf(mesh)
                    })
                    .OrderBy(t => t.Index)
                    .Select(t => t.Mesh)
                    .ToList();

                foreach (var mesh in orderedMeshes)
                {
                    int index = opt.Meshes.IndexOf(mesh);

                    if (index == 0)
                    {
                        continue;
                    }

                    opt.Meshes.RemoveAt(index);
                    opt.Meshes.Insert(index - 1, mesh);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(meshes));
                dispatcher(() => this.OptModel.UndoStackPush("move up mesh"));
            });
        }

        private void ExecuteDownMeshesCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var opt = this.OptModel.File;

                dispatcher(() => this.CurrentMeshes.ClearSelection());

                var orderedMeshes = meshes
                    .Select(mesh => new
                    {
                        Mesh = mesh,
                        Index = opt.Meshes.IndexOf(mesh)
                    })
                    .OrderByDescending(t => t.Index)
                    .Select(t => t.Mesh)
                    .ToList();

                foreach (var mesh in orderedMeshes)
                {
                    int index = opt.Meshes.IndexOf(mesh);

                    if (index == opt.Meshes.Count - 1)
                    {
                        continue;
                    }

                    opt.Meshes.RemoveAt(index);
                    opt.Meshes.Insert(index + 1, mesh);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(meshes));
                dispatcher(() => this.OptModel.UndoStackPush("move down mesh"));
            });
        }

        private void ExecuteSplitMeshesCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var opt = this.OptModel.File;

                dispatcher(() => this.CurrentMeshes.ClearSelection());

                foreach (var mesh in meshes)
                {
                    opt.SplitMesh(mesh);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.OptModel.UndoStackPush("split mesh"));
            });
        }

        private void ExecuteMergeMeshesCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                bool doMerge = true;

                if (meshes.Sum(t => t.Vertices.Count - 2) > 510)
                {
                    var result = Messenger.Instance.Notify(new MessageBoxMessage(
                        "The merged mesh may contain more than 512 vertices.\nDo you want to continue?",
                        "Merge Meshes",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning));

                    doMerge = result.Result == MessageBoxResult.Yes;
                }

                if (doMerge)
                {
                    dispatcher(() => this.CurrentMeshes.ClearSelection());

                    var merge = this.OptModel.File.MergeMeshes(meshes);

                    if (merge != null)
                    {
                        merge.MergeLods(merge.Lods
                            .OrderBy(t => t.Distance)
                            .TakeWhile(t => t.Distance == merge.Lods[0].Distance)
                            .ToList());
                    }

                    dispatcher(() => this.UpdateModel());
                    dispatcher(() => this.CurrentMeshes.SetSelection(merge));
                    dispatcher(() => this.OptModel.UndoStackPush("merge meshes"));
                }
            });
        }

        private void ExecuteMoveMeshesCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                float moveX;
                float moveY;
                float moveZ;

                if (meshes.Count == 1)
                {
                    var mesh = meshes[0];

                    var message = new MoveFactorSingleMessage();
                    if (mesh.Descriptor != null)
                    {
                        var center = mesh.Descriptor.Center;
                        message.CenterX = center.X;
                        message.CenterY = center.Y;
                        message.CenterZ = center.Z;
                    }

                    Messenger.Instance.Notify(message);

                    if (!message.Changed)
                    {
                        return;
                    }

                    moveX = message.MoveX;
                    moveY = message.MoveY;
                    moveZ = message.MoveZ;
                }
                else
                {
                    var message = Messenger.Instance.Notify(new MoveFactorMessage());

                    if (!message.Changed)
                    {
                        return;
                    }

                    moveX = message.MoveX;
                    moveY = message.MoveY;
                    moveZ = message.MoveZ;
                }

                BusyIndicatorService.Notify("Moving ...");

                foreach (var mesh in meshes)
                {
                    mesh.Move(moveX, moveY, moveZ);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(meshes));
                dispatcher(() => this.OptModel.UndoStackPush("move meshes"));
            });
        }

        private void ExecuteRotateMeshCommand(Mesh mesh)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var message = new RotateFactorMessage();
                if (mesh.Descriptor != null)
                {
                    var center = mesh.Descriptor.Center;
                    message.CenterX = center.X;
                    message.CenterY = center.Y;
                }

                Messenger.Instance.Notify(message);

                if (!message.Changed)
                {
                    return;
                }

                BusyIndicatorService.Notify("Rotating ...");

                mesh.RotateXY(message.Angle, message.CenterX, message.CenterY);

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("rotate mesh"));
            });
        }

        private void ExecuteDuplicateMeshesCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                BusyIndicatorService.Notify("Duplicating ...");

                foreach (var mesh in meshes)
                {
                    this.OptModel.File.Meshes.Add(mesh.Duplicate());
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(meshes));
                dispatcher(() => this.OptModel.UndoStackPush("duplicate meshes"));
            });
        }

        private void ExecuteComputeHitzonesCommand()
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                this.OptModel.File.ComputeHitzones();

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("compute hitzones"));
            });
        }

        private void ExecuteExportMeshObjCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetSaveObjFileName("Export OBJ file", System.IO.Path.ChangeExtension(this.OptModel.File.FileName, "obj"));

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Exporting ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File.Clone();
                opt.Meshes.Clear();

                foreach (var mesh in meshes)
                {
                    opt.Meshes.Add(mesh.Clone());
                }

                opt.RemoveUnusedTextures();

                bool scale = true;

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

        private void ExecuteExportMeshRhinoCommand(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetSaveRhinoFileName("Export 3DM file", System.IO.Path.ChangeExtension(this.OptModel.File.FileName, "3dm"));

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Exporting ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File.Clone();
                opt.Meshes.Clear();

                foreach (var mesh in meshes)
                {
                    opt.Meshes.Add(mesh.Clone());
                }

                opt.RemoveUnusedTextures();

                bool scale = true;

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

        private void ExecuteExportMeshAn8Command(IList<Mesh> meshes)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetSaveAn8FileName("Export AN8 file", System.IO.Path.ChangeExtension(this.OptModel.File.FileName, "an8"));

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Exporting ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File.Clone();
                opt.Meshes.Clear();

                foreach (var mesh in meshes)
                {
                    opt.Meshes.Add(mesh.Clone());
                }

                opt.RemoveUnusedTextures();

                bool scale = true;

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

        private void ReplaceMesh(OptFile opt, Mesh mesh, OptFile import)
        {
            import.MergeMeshes(import.Meshes.ToList());

            foreach (var texture in import.Textures.Values)
            {
                opt.Textures[texture.Name] = texture;
            }

            mesh.Lods.Clear();
            mesh.Vertices.Clear();
            mesh.TextureCoordinates.Clear();
            mesh.VertexNormals.Clear();

            if (import.Meshes.Count != 0)
            {
                var importMesh = import.Meshes[0];

                if (importMesh.Lods.Count != 0)
                {
                    mesh.Lods.Add(importMesh.Lods[0]);
                }

                for (int i = 0; i < importMesh.Vertices.Count; i++)
                {
                    mesh.Vertices.Add(importMesh.Vertices[i]);
                }

                for (int i = 0; i < importMesh.TextureCoordinates.Count; i++)
                {
                    mesh.TextureCoordinates.Add(importMesh.TextureCoordinates[i]);
                }

                for (int i = 0; i < importMesh.VertexNormals.Count; i++)
                {
                    mesh.VertexNormals.Add(importMesh.VertexNormals[i]);
                }
            }

            mesh.CompactBuffers();
            mesh.ComputeHitzone();
        }

        private void ExecuteReplaceMeshObjCommand(Mesh mesh)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenObjFileName("Replace OBJ file", null);

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Replace ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = true;

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    var import = OptObjConverter.Converter.ObjToOpt(fileName, scale);

                    this.ReplaceMesh(opt, mesh, import);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("replace " + System.IO.Path.GetFileName(fileName)));
            });
        }

        private void ExecuteReplaceMeshRhinoCommand(Mesh mesh)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenRhinoFileName("Replace 3DM file", null);

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Replace ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = true;

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    var import = OptRhinoConverter.Converter.RhinoToOpt(fileName, scale);

                    this.ReplaceMesh(opt, mesh, import);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("replace " + System.IO.Path.GetFileName(fileName)));
            });
        }

        private void ExecuteReplaceMeshAn8Command(Mesh mesh)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenAn8FileName("Replace AN8 file", null);

                if (fileName == null)
                {
                    return;
                }

                BusyIndicatorService.Notify(string.Concat("Replace ", System.IO.Path.GetFileName(fileName), "..."));

                var opt = this.OptModel.File;
                bool scale = true;

                try
                {
                    dispatcher(() => this.OptModel.File = null);

                    var import = OptAn8Converter.Converter.An8ToOpt(fileName, scale);

                    this.ReplaceMesh(opt, mesh, import);
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }

                dispatcher(() => this.OptModel.File = opt);
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("replace " + System.IO.Path.GetFileName(fileName)));
            });
        }

        private void ExecuteNewLodCommand()
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                var lod = new MeshLod();
                mesh.Lods.Add(lod);

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(lod));
                dispatcher(() => this.OptModel.UndoStackPush("new lod"));
            });
        }

        private void ExecuteDeleteLodsCommand(IList<MeshLod> lods)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                dispatcher(() => this.CurrentLods.ClearSelection());

                foreach (var lod in lods)
                {
                    mesh.Lods.Remove(lod);
                }

                mesh.CompactBuffers();

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("delete lod"));
            });
        }

        private void ExecuteUpLodsCommand(IList<MeshLod> lods)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                dispatcher(() => this.CurrentLods.ClearSelection());

                var orderedLods = lods
                    .Select(lod => new
                    {
                        Lod = lod,
                        Index = mesh.Lods.IndexOf(lod)
                    })
                    .OrderBy(t => t.Index)
                    .Select(t => t.Lod)
                    .ToList();

                foreach (var lod in orderedLods)
                {
                    int index = mesh.Lods.IndexOf(lod);

                    if (index == 0)
                    {
                        continue;
                    }

                    mesh.Lods.RemoveAt(index);
                    mesh.Lods.Insert(index - 1, lod);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(lods));
                dispatcher(() => this.OptModel.UndoStackPush("move up lod"));
            });
        }

        private void ExecuteDownLodsCommand(IList<MeshLod> lods)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                dispatcher(() => this.CurrentLods.ClearSelection());

                var orderedLods = lods
                    .Select(lod => new
                    {
                        Lod = lod,
                        Index = mesh.Lods.IndexOf(lod)
                    })
                    .OrderByDescending(t => t.Index)
                    .Select(t => t.Lod)
                    .ToList();

                foreach (var lod in orderedLods)
                {
                    int index = mesh.Lods.IndexOf(lod);

                    if (index == mesh.Lods.Count - 1)
                    {
                        continue;
                    }

                    mesh.Lods.RemoveAt(index);
                    mesh.Lods.Insert(index + 1, lod);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(lods));
                dispatcher(() => this.OptModel.UndoStackPush("move down lod"));
            });
        }

        private void ExecuteSortLodsCommand()
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                dispatcher(() => this.CurrentLods.ClearSelection());

                mesh.SortLods();

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("sort lods"));
            });
        }

        private void ExecuteSplitLodsCommand(IList<MeshLod> lods)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                dispatcher(() => this.CurrentLods.ClearSelection());

                foreach (var lod in lods)
                {
                    mesh.SplitLod(lod);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("split lod"));
            });
        }

        private void ExecuteMergeLodsCommand(IList<MeshLod> lods)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                dispatcher(() => this.CurrentLods.ClearSelection());

                var mergedLod = mesh.MergeLods(lods);

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(mergedLod));
                dispatcher(() => this.OptModel.UndoStackPush("merge lods"));
            });
        }

        private void ExecuteComputeHitzoneCommand()
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                mesh.ComputeHitzone();

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("compute hitzone"));
            });
        }

        private void ExecuteNewHardpointCommand()
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                mesh.Hardpoints.Add(new Hardpoint());

                dispatcher(() => this.UpdateModel(true));
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("new hardpoint"));
            });
        }

        private void ExecuteDeleteHardpointsCommand(IList<Hardpoint> hardpoints)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                foreach (var hardpoint in hardpoints)
                {
                    mesh.Hardpoints.Remove(hardpoint);
                }

                dispatcher(() => this.UpdateModel(true));
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("delete hardpoints"));
            });
        }

        private void ExecuteCutHardpointsCommand(IList<Hardpoint> hardpoints)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            this.clipboardObject = new Tuple<Mesh, IList<Hardpoint>>(this.CurrentMeshes.SelectedItem, hardpoints);
        }

        private void ExecuteCopyHardpointsCommand(IList<Hardpoint> hardpoints)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            this.clipboardObject = new Tuple<Mesh, IList<Hardpoint>>(null, hardpoints);
        }

        private void ExecutePasteHardpointsCommand()
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            var selected = this.clipboardObject as Tuple<Mesh, IList<Hardpoint>>;

            if (selected == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                if (selected.Item1 == null)
                {
                    foreach (var hardpoint in selected.Item2)
                    {
                        mesh.Hardpoints
                            .Add(new Hardpoint
                            {
                                HardpointType = hardpoint.HardpointType,
                                Position = hardpoint.Position
                            });
                    }
                }
                else
                {
                    this.clipboardObject = null;

                    foreach (var hardpoint in selected.Item2)
                    {
                        selected.Item1.Hardpoints.Remove(hardpoint);
                        mesh.Hardpoints.Add(hardpoint);
                    }
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("paste hardpoints"));
            });
        }

        private void ExecuteNewEngineGlowCommand()
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                mesh.EngineGlows.Add(new EngineGlow
                {
                    Format = new JeremyAnsel.Xwa.Opt.Vector(50, 50, 1)
                });

                dispatcher(() => this.UpdateModel(true));
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("new engine glow"));
            });
        }

        private void ExecuteDeleteEngineGlowsCommand(IList<EngineGlow> engineGlows)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                foreach (var engineGlow in engineGlows)
                {
                    mesh.EngineGlows.Remove(engineGlow);
                }

                dispatcher(() => this.UpdateModel(true));
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("delete engine glow"));
            });
        }

        private void ExecuteCutEngineGlowsCommand(IList<EngineGlow> engineGlows)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            this.clipboardObject = new Tuple<Mesh, IList<EngineGlow>>(this.CurrentMeshes.SelectedItem, engineGlows);
        }

        private void ExecuteCopyEngineGlowsCommand(IList<EngineGlow> engineGlows)
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            this.clipboardObject = new Tuple<Mesh, IList<EngineGlow>>(null, engineGlows);
        }

        private void ExecutePasteEngineGlowsCommand()
        {
            if (this.CurrentMeshes.SelectedItem == null)
            {
                return;
            }

            var selected = this.clipboardObject as Tuple<Mesh, IList<EngineGlow>>;

            if (selected == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;

                if (selected.Item1 == null)
                {
                    foreach (var engineGlow in selected.Item2)
                    {
                        mesh.EngineGlows
                            .Add(new EngineGlow
                            {
                                IsDisabled = engineGlow.IsDisabled,
                                Position = engineGlow.Position,
                                CoreColor = engineGlow.CoreColor,
                                OuterColor = engineGlow.OuterColor,
                                Format = engineGlow.Format,
                                Look = engineGlow.Look,
                                Up = engineGlow.Up,
                                Right = engineGlow.Right
                            });
                    }
                }
                else
                {
                    this.clipboardObject = null;

                    foreach (var engineGlow in selected.Item2)
                    {
                        selected.Item1.EngineGlows.Remove(engineGlow);
                        mesh.EngineGlows.Add(engineGlow);
                    }
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("paste engine glows"));
            });
        }

        private void ExecuteAddTextureNameCommand()
        {
            if (this.CurrentFaceGroups.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                string fileName = FileDialogService.GetOpenTextureFileName("Open texture", null);

                if (fileName == null)
                {
                    return;
                }

                try
                {
                    var texture = Texture.FromFile(fileName);

                    int bpp = texture.BitsPerPixel;

                    texture.GenerateMipmaps();

                    if (bpp == 8)
                    {
                        texture.Convert32To8();
                    }

                    var mesh = this.CurrentMeshes.SelectedItem;
                    var lod = this.CurrentLods.SelectedItem;
                    var selectedTextures = this.CurrentFaceGroups.SelectedItem.Textures.ToList();
                    var faceGroups = this.CurrentFaceGroups.SelectedItems.ToList();

                    this.OptModel.File.Textures[texture.Name] = texture;

                    foreach (var faceGroup in faceGroups.Where(t => selectedTextures.SequenceEqual(t.Textures)))
                    {
                        faceGroup.Textures.Add(texture.Name);
                    }

                    dispatcher(() => this.UpdateModel());
                    dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                    dispatcher(() => this.CurrentLods.SetSelection(lod));
                    dispatcher(() => this.CurrentFaceGroups.SetSelection(faceGroups));
                    dispatcher(() => this.OptModel.UndoStackPush("add texture name " + texture.Name));
                }
                catch (Exception ex)
                {
                    Messenger.Instance.Notify(new MessageBoxMessage(fileName, ex));
                }
            });
        }

        private void ExecuteBrowseTextureNameCommand()
        {
            if (this.CurrentFaceGroups.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                Messenger.Instance.Notify(new BusyIndicatorMessage("Browsing texture..."));

                var message = Messenger.Instance.Notify(new TextureBrowserMessage(this.OptModel.File));

                if (string.IsNullOrEmpty(message.TextureName))
                {
                    return;
                }

                var mesh = this.CurrentMeshes.SelectedItem;
                var lod = this.CurrentLods.SelectedItem;
                var selectedTextures = this.CurrentFaceGroups.SelectedItem.Textures.ToList();
                var faceGroups = this.CurrentFaceGroups.SelectedItems.ToList();

                foreach (var faceGroup in faceGroups.Where(t => selectedTextures.SequenceEqual(t.Textures)))
                {
                    faceGroup.Textures.Add(message.TextureName);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(lod));
                dispatcher(() => this.CurrentFaceGroups.SetSelection(faceGroups));
                dispatcher(() => this.OptModel.UndoStackPush("add texture name " + message.TextureName));
            });
        }

        private void ExecuteDeleteTextureNamesCommand(IList<StringWrapper> textureNames)
        {
            if (this.CurrentFaceGroups.SelectedItem == null)
            {
                return;
            }

            if (textureNames == null || textureNames.Count == 0)
            {
                return;
            }

            IList<int> selectedItemsIndexes = this.CurrentTextureNames
                .GetSelectedIndexes(textureNames)
                .Reverse()
                .ToList();

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;
                var lod = this.CurrentLods.SelectedItem;
                var selectedTextures = this.CurrentFaceGroups.SelectedItem.Textures.ToList();
                var faceGroups = this.CurrentFaceGroups.SelectedItems.ToList();

                foreach (var faceGroup in faceGroups.Where(t => selectedTextures.SequenceEqual(t.Textures)))
                {
                    foreach (int textureIndex in selectedItemsIndexes)
                    {
                        faceGroup.Textures.RemoveAt(textureIndex);
                    }
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(lod));
                dispatcher(() => this.CurrentFaceGroups.SetSelection(faceGroups));
                dispatcher(() => this.OptModel.UndoStackPush("delete texture names"));
            });
        }

        private void ExecuteClearTextureNamesCommand()
        {
            if (this.CurrentFaceGroups.SelectedItem == null)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;
                var lod = this.CurrentLods.SelectedItem;
                var selectedTextures = this.CurrentFaceGroups.SelectedItem.Textures.ToList();
                var faceGroups = this.CurrentFaceGroups.SelectedItems.ToList();

                foreach (var faceGroup in faceGroups.Where(t => selectedTextures.SequenceEqual(t.Textures)))
                {
                    faceGroup.Textures.Clear();
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(lod));
                dispatcher(() => this.CurrentFaceGroups.SetSelection(faceGroups));
                dispatcher(() => this.OptModel.UndoStackPush("clear texture names"));
            });
        }

        private void ExecuteUpTextureNamesCommand(int textureIndex)
        {
            if (this.CurrentFaceGroups.SelectedItem == null)
            {
                return;
            }

            if (textureIndex == -1 || textureIndex == 0)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;
                var lod = this.CurrentLods.SelectedItem;
                var selectedTextures = this.CurrentFaceGroups.SelectedItem.Textures.ToList();
                var faceGroups = this.CurrentFaceGroups.SelectedItems.ToList();

                foreach (var faceGroup in faceGroups.Where(t => selectedTextures.SequenceEqual(t.Textures)))
                {
                    string textureName = faceGroup.Textures[textureIndex];
                    faceGroup.Textures.RemoveAt(textureIndex);
                    faceGroup.Textures.Insert(textureIndex - 1, textureName);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(lod));
                dispatcher(() => this.CurrentFaceGroups.SetSelection(faceGroups));
                dispatcher(() => this.ModelVersion = textureIndex - 1);
                dispatcher(() => this.OptModel.UndoStackPush("move up texture name"));
            });
        }

        private void ExecuteDownTextureNamesCommand(int textureIndex)
        {
            if (this.CurrentFaceGroups.SelectedItem == null)
            {
                return;
            }

            if (textureIndex == -1 || textureIndex == this.CurrentFaceGroups.SelectedItem.Textures.Count - 1)
            {
                return;
            }

            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = this.CurrentMeshes.SelectedItem;
                var lod = this.CurrentLods.SelectedItem;
                var selectedTextures = this.CurrentFaceGroups.SelectedItem.Textures.ToList();
                var faceGroups = this.CurrentFaceGroups.SelectedItems.ToList();

                foreach (var faceGroup in faceGroups.Where(t => selectedTextures.SequenceEqual(t.Textures)))
                {
                    string textureName = faceGroup.Textures[textureIndex];
                    faceGroup.Textures.RemoveAt(textureIndex);
                    faceGroup.Textures.Insert(textureIndex + 1, textureName);
                }

                dispatcher(() => this.UpdateModel());
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.CurrentLods.SetSelection(lod));
                dispatcher(() => this.CurrentFaceGroups.SetSelection(faceGroups));
                dispatcher(() => this.ModelVersion = textureIndex + 1);
                dispatcher(() => this.OptModel.UndoStackPush("move down texture name"));
            });
        }

        private void ExecuteSelectMeshCommand(Tuple<MeshLodFace, Point3D> tag)
        {
            this.CurrentMeshes.SetSelection(tag.Item1.Mesh);
            this.CurrentLods.SetSelection(tag.Item1.Lod);
            this.CurrentFaceGroups.SetSelection(tag.Item1.Face);
        }

        private void ExecuteAddMeshToSelectionCommand(Tuple<MeshLodFace, Point3D> tag)
        {
            this.CurrentMeshes.AddToSelection(tag.Item1.Mesh);
        }

        private void ExecuteAddHardpointCommand(Tuple<MeshLodFace, Point3D> tag)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = tag.Item1.Mesh;

                mesh.Hardpoints
                    .Add(new Hardpoint
                    {
                        Position = new JeremyAnsel.Xwa.Opt.Vector((float)-tag.Item2.Y, (float)-tag.Item2.X, (float)tag.Item2.Z)
                    });

                dispatcher(() => this.UpdateModel(true));
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("add hardpoint"));
            });
        }

        private void ExecuteAddEngineGlowCommand(Tuple<MeshLodFace, Point3D> tag)
        {
            BusyIndicatorService.Run(dispatcher =>
            {
                var mesh = tag.Item1.Mesh;

                mesh.EngineGlows
                    .Add(new EngineGlow
                    {
                        Position = new JeremyAnsel.Xwa.Opt.Vector((float)-tag.Item2.Y, (float)-tag.Item2.X, (float)tag.Item2.Z),
                        Format = new JeremyAnsel.Xwa.Opt.Vector(50, 50, 1)
                    });

                dispatcher(() => this.UpdateModel(true));
                dispatcher(() => this.CurrentMeshes.SetSelection(mesh));
                dispatcher(() => this.OptModel.UndoStackPush("add engine glow"));
            });
        }

        private void ExecuteCopyPointCommand(Tuple<MeshLodFace, Point3D> tag)
        {
            float scale = JeremyAnsel.Xwa.Opt.OptFile.ScaleFactor;

            var point = tag.Item2;

            var vector = new JeremyAnsel.Xwa.Opt.Vector(
                (float)-point.Y * scale,
                (float)-point.X * scale,
                (float)point.Z * scale);

            Clipboard.SetText(vector.ToString());
        }
    }
}
