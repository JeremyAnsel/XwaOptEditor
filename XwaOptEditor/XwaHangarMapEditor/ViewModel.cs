using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace XwaHangarMapEditor
{
    public sealed class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool InvokePropertyChanged { get; set; } = true;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (!this.InvokePropertyChanged)
            {
                return;
            }

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void UpdateAllProperties()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private readonly SortedDictionary<string, Dictionary<string, IList<int>>> _optObjectProfiles = new();

        private Dictionary<string, IList<int>> GetOptObjectProfiles(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!_optObjectProfiles.TryGetValue(name, out Dictionary<string, IList<int>> model))
            {
                string fileName = AppSettings.WorkingDirectory + "FlightModels\\" + name + ".opt";

                if (!System.IO.File.Exists(fileName))
                {
                    return null;
                }

                model = OptProfileHelper.GetObjectProfiles(fileName);

                _optObjectProfiles.Add(name, model);
            }

            return model;
        }

        private readonly SortedDictionary<string, IList<string>> _optSkins = new();

        private IList<string> GetOptSkins(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!_optSkins.TryGetValue(name, out IList<string> model))
            {
                string fileName = AppSettings.WorkingDirectory + "FlightModels\\" + name + ".opt";

                if (!System.IO.File.Exists(fileName))
                {
                    return null;
                }

                model = OptProfileHelper.GetSkins(fileName);

                _optSkins.Add(name, model);
            }

            return model;
        }

        private readonly SortedDictionary<string, OptModel> _optModels = new();

        private OptModel GetOptModel(string name, int version = 0, IList<int> objectProfile = null, IList<string> skins = null)
        {
            if (string.IsNullOrEmpty(AppSettings.WorkingDirectory))
            {
                return null;
            }

            string key = name;
            key += "," + version.ToString(CultureInfo.InvariantCulture);
            key += "," + string.Join("_", objectProfile ?? new List<int>());
            key += "," + string.Join("_", skins ?? new List<string> { "Default" });

            key = key.ToUpperInvariant();

            if (!_optModels.TryGetValue(key, out OptModel model))
            {
                string fileName = AppSettings.WorkingDirectory + "FlightModels\\" + name + ".opt";

                if (!System.IO.File.Exists(fileName))
                {
                    return null;
                }

                model = OptModel.FromFile(fileName, version, objectProfile, skins);

                _optModels.Add(key, model);
            }

            return model;
        }

        private bool _showPlayer = true;

        public bool ShowPlayer
        {
            get
            {
                return this._showPlayer;
            }

            set
            {
                if (this._showPlayer == value)
                {
                    return;
                }

                this._showPlayer = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HangarMap3D));
            }
        }

        private bool _showRescueShuttle = true;

        public bool ShowRescueShuttle
        {
            get
            {
                return this._showRescueShuttle;
            }

            set
            {
                if (this._showRescueShuttle == value)
                {
                    return;
                }

                this._showRescueShuttle = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HangarMap3D));
            }
        }

        private bool _showHangarRoofCrane = true;

        public bool ShowHangarRoofCrane
        {
            get
            {
                return this._showHangarRoofCrane;
            }

            set
            {
                if (this._showHangarRoofCrane == value)
                {
                    return;
                }

                this._showHangarRoofCrane = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HangarMap3D));
            }
        }

        public List<ObjectItem> ObjectItems
        {
            get
            {
                var items = Enumerable
                    .Range(0, AppSettings.Objects?.Count ?? 0)
                    .Select(index =>
                    {
                        var item = new ObjectItem
                        {
                            ModelIndex = index,
                            ModelName = (string)ExeModelIndexConverter.Default.Convert(index, null, null, null)
                        };

                        Dictionary<string, IList<int>> objectProfiles = this.GetOptObjectProfiles(item.ModelName);

                        if (objectProfiles is not null)
                        {
                            item.ObjectProfiles = objectProfiles
                                .Select(t => t.Key)
                                .ToList();
                        }

                        IList<string> skins = this.GetOptSkins(item.ModelName);

                        if (skins is not null)
                        {
                            item.Skins = skins.ToList();
                        }

                        return item;
                    })
                    .ToList();

                return items;
            }
        }

        private string hangarBaseName;

        public string HangarBaseName
        {
            get
            {
                return this.hangarBaseName;
            }

            set
            {
                if (this.hangarBaseName == value)
                {
                    return;
                }

                this.hangarBaseName = value;
                this.OnPropertyChanged();
            }
        }

        private string hangarModel = "Hangar";

        public string HangarModel
        {
            get
            {
                return this.hangarModel;
            }

            set
            {
                if (this.hangarModel == value)
                {
                    return;
                }

                this.hangarModel = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HangarModel3D));
                this.OnPropertyChanged(nameof(this.HangarMap3D));
            }
        }

        public bool IsRegularHangar
        {
            get
            {
                return this.HangarModel is null || this.HangarModel.EndsWith("Hangar", StringComparison.OrdinalIgnoreCase);
            }
        }

        private string hangarSkinsText = GlobalConstants.DefaultHangarSkinsText;

        public string HangarSkinsText
        {
            get
            {
                return this.hangarSkinsText;
            }

            set
            {
                if (this.hangarSkinsText == value)
                {
                    return;
                }

                this.hangarSkinsText = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HangarSkins));
                this.OnPropertyChanged(nameof(this.HangarMap3D));
            }
        }

        public HangarSkins HangarSkins
        {
            get
            {
                return HangarSkins.FromText(this.HangarSkinsText);
            }
        }

        private string hangarObjectsText = GlobalConstants.DefaultHangarObjectsText;

        public string HangarObjectsText
        {
            get
            {
                return this.hangarObjectsText;
            }

            set
            {
                if (this.hangarObjectsText == value)
                {
                    return;
                }

                this.hangarObjectsText = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HangarObjects));
                this.OnPropertyChanged(nameof(this.HangarMap3D));
            }
        }

        public HangarObjects HangarObjects
        {
            get
            {
                return HangarObjects.FromText(this.HangarObjectsText);
            }
        }

        private string hangarMapText = GlobalConstants.DefaultHangarMapText;

        public string HangarMapText
        {
            get
            {
                return this.hangarMapText;
            }

            set
            {
                if (this.hangarMapText == value)
                {
                    return;
                }

                this.hangarMapText = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HangarMap));
                this.OnPropertyChanged(nameof(this.HangarMap3D));
            }
        }

        public HangarMap HangarMap
        {
            get
            {
                return HangarMap.FromText(this.HangarMapText);
            }
        }

        public Visual3D HangarModel3D
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.HangarModel))
                {
                    return new OptVisual3D();
                }

                var hangar = this.GetOptModel(this.HangarModel);

                if (hangar == null)
                {
                    return new OptVisual3D();
                }

                var model3D = new OptVisual3D
                {
                    Cache = hangar.Cache,
                    SortingFrequency = .2
                };

                return model3D;
            }
        }

        public IList<Visual3D> HangarMap3D
        {
            get
            {
                HangarSkins hangarSkins = this.HangarSkins;
                HangarObjects hangarObjects = this.HangarObjects;
                HangarMap hangarMap = this.HangarMap;
                bool isRegularHangar = this.IsRegularHangar;

                double hangarFloorZ = 0;
                bool isHangarFloorInverted = false;

                if (!string.IsNullOrWhiteSpace(this.HangarModel))
                {
                    var hangar = this.GetOptModel(this.HangarModel);

                    if (hangar != null)
                    {
                        Hardpoint hangarFloorHardpoint = hangar.File
                            .Meshes
                            .SelectMany(t => t.Hardpoints)
                            .Where(t => t.HardpointType == HardpointType.InsideHangar)
                            .FirstOrDefault();

                        if (hangarFloorHardpoint != null)
                        {
                            hangarFloorZ = hangarFloorHardpoint.Position.Z;
                        }

                        isHangarFloorInverted = hangarObjects.IsHangarFloorInverted;
                    }
                }

                HangarMap map = HangarMap.FromText(this.HangarMapText);

                var collection = new List<Visual3D>(map.Count);

                foreach (HangarItem item in map)
                {
                    Visual3D model = this.CreateModel3D(item, 0, hangarFloorZ, isHangarFloorInverted, hangarSkins, hangarObjects);

                    if (model != null)
                    {
                        collection.Add(model);
                    }
                }

                if (this.ShowPlayer)
                {
                    var player = new HangarItem
                    {
                        ModelIndex = 1,
                        Markings = 0,
                        PositionX = hangarObjects.PlayerOffsetX,
                        PositionY = hangarObjects.PlayerOffsetY,
                        PositionZ = 0x7FFFFFFF,
                        HeadingXY = 0,
                        HeadingZ = 0,
                        ObjectProfile = "Default"
                    };

                    if (isRegularHangar)
                    {
                        player.PositionY -= 0x320;
                    }
                    else
                    {
                        player.PositionY -= 0x1A90;
                    }

                    Visual3D model = this.CreateModel3D(player, hangarObjects.PlayerOffsetZ, hangarFloorZ, hangarObjects.IsPlayerFloorInverted, hangarSkins, hangarObjects);

                    if (model != null)
                    {
                        collection.Add(model);
                    }
                }

                if (isRegularHangar && this.ShowRescueShuttle && hangarObjects.LoadShuttle)
                {
                    var shuttle = new HangarItem
                    {
                        ModelIndex = hangarObjects.ShuttleModelIndex,
                        Markings = hangarObjects.ShuttleMarkings,
                        PositionX = hangarObjects.ShuttlePositionX,
                        PositionY = hangarObjects.ShuttlePositionY,
                        PositionZ = 0x7FFFFFFF,
                        HeadingXY = hangarObjects.ShuttleOrientation,
                        HeadingZ = 0,
                        ObjectProfile = hangarObjects.ShuttleObjectProfile
                    };

                    Visual3D model = this.CreateModel3D(shuttle, hangarObjects.ShuttlePositionZ, hangarFloorZ, hangarObjects.IsShuttleFloorInverted, hangarSkins, hangarObjects);

                    if (model != null)
                    {
                        collection.Add(model);
                    }
                }

                if (isRegularHangar && this.ShowHangarRoofCrane)
                {
                    var roofCrane = new HangarItem
                    {
                        ModelIndex = 316,
                        Markings = 0,
                        PositionX = hangarObjects.HangarRoofCranePositionX,
                        PositionY = hangarObjects.HangarRoofCranePositionY,
                        PositionZ = hangarObjects.HangarRoofCranePositionZ,
                        HeadingXY = 0,
                        HeadingZ = 0,
                        ObjectProfile = "Default"
                    };

                    Visual3D model = this.CreateModel3D(roofCrane, 0, hangarFloorZ, isHangarFloorInverted, hangarSkins, hangarObjects);

                    if (model != null)
                    {
                        collection.Add(model);
                    }
                }

                return collection;
            }
        }

        private Visual3D CreateModel3D(HangarItem item, int itemOffsetZ, double hangarFloorZ, bool isHangarFloorInverted, HangarSkins hangarSkins, HangarObjects hangarObjects)
        {
            string name = (string)ExeModelIndexConverter.Default.Convert(item.ModelIndex, null, null, null);

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (hangarObjects.ObjectsReplace.TryGetValue(name, out string value))
            {
                name = value;
            }

            var objectProfiles = this.GetOptObjectProfiles(name);

            if (objectProfiles is null)
            {
                return null;
            }

            objectProfiles.TryGetValue(item.ObjectProfile, out IList<int> objectProfile);

            IList<string> skin = null;

            if (hangarSkins.TryGetValue(name, out Dictionary<int, IList<string>> skins))
            {
                if (!skins.TryGetValue(item.Markings, out skin))
                {
                    if (skins.ContainsKey(-1))
                    {
                        skin = skins[-1];
                    }
                }
            }

            var model = this.GetOptModel(name, item.Markings, objectProfile, skin);

            if (model is null)
            {
                return null;
            }

            var modelVisual3D = new ModelVisual3D();

            for (int meshIndex = 0; meshIndex < model.File.Meshes.Count; meshIndex++)
            {
                var mesh = model.File.Meshes[meshIndex];
                var sfoil = model.SFoils.Where(t => t.MeshIndex == meshIndex).FirstOrDefault() ?? new SFoil();

                var visual = new OptVisual3D
                {
                    Cache = model.Cache,
                    Mesh = mesh,
                    SortingFrequency = .2,
                    Version = item.Markings
                };

                double angle = sfoil.Angle * 360.0 / 255 * mesh.RotationScale.Look.LengthFactor();
                var transform = new RotateTransform3D(new AxisAngleRotation3D(mesh.RotationScale.Look.ToVector3D(), angle), mesh.RotationScale.Pivot.ToPoint3D());
                transform.Freeze();
                visual.Transform = transform;

                modelVisual3D.Children.Add(visual);
            }

            double offsetX = item.PositionX;
            double offsetY = item.PositionY;
            double offsetZ;

            bool isFloorInverted = item.IsHangarFloorInverted ?? isHangarFloorInverted;

            if (item.IsOnFloor)
            {
                if (isFloorInverted)
                {
                    offsetZ = hangarFloorZ + hangarObjects.HangarFloorInvertedHeight - model.ClosedSFoilsElevationInverted;
                }
                else
                {
                    offsetZ = hangarFloorZ + model.ClosedSFoilsElevation;
                }
            }
            else
            {
                offsetZ = item.PositionZ;
            }

            var transformGroup = new Transform3DGroup();

            int bridgeIndex = model.File.Meshes.IndexOf(model.File.Meshes.FirstOrDefault(t => t.Descriptor.MeshType == MeshType.Bridge));

            if (bridgeIndex != -1)
            {
                var transform = modelVisual3D.Children[bridgeIndex].Transform;
                transformGroup.Children.Add(transform);
            }

            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), item.HeadingZ * 360.0 / 65536)));
            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -item.HeadingXY * 360.0 / 65536)));
            transformGroup.Children.Add(new TranslateTransform3D(offsetY, -offsetX, offsetZ + itemOffsetZ));
            transformGroup.Freeze();

            modelVisual3D.Transform = transformGroup;

            return modelVisual3D;
        }

        public OptFile BuildOptMap(bool includeHangar = true)
        {
            HangarSkins hangarSkins = this.HangarSkins;
            HangarObjects hangarObjects = this.HangarObjects;
            HangarMap hangarMap = this.HangarMap;
            bool isRegularHangar = this.IsRegularHangar;

            var optFile = new OptFile();

            OptModel hangar = string.IsNullOrWhiteSpace(this.HangarModel) ? null : this.GetOptModel(this.HangarModel);

            double hangarFloorZ = 0;
            bool isHangarFloorInverted = false;

            if (hangar != null)
            {
                if (includeHangar)
                {
                    MergeOpt(optFile, hangar.File);
                }

                Hardpoint hangarFloorHardpoint = hangar.File
                    .Meshes
                    .SelectMany(t => t.Hardpoints)
                    .Where(t => t.HardpointType == HardpointType.InsideHangar)
                    .FirstOrDefault();

                if (hangarFloorHardpoint != null)
                {
                    hangarFloorZ = hangarFloorHardpoint.Position.Z;
                }

                isHangarFloorInverted = hangarObjects.IsHangarFloorInverted;
            }

            var collection = new List<Visual3D>(hangarMap.Count);

            foreach (HangarItem item in hangarMap)
            {
                OptFile model = this.CreateModelOpt(item, 0, hangarFloorZ, isHangarFloorInverted, hangarSkins, hangarObjects);

                if (model != null)
                {
                    MergeOpt(optFile, model);
                }
            }

            if (isRegularHangar && hangarObjects.LoadShuttle)
            {
                var shuttle = new HangarItem
                {
                    ModelIndex = hangarObjects.ShuttleModelIndex,
                    Markings = hangarObjects.ShuttleMarkings,
                    PositionX = hangarObjects.ShuttlePositionX,
                    PositionY = hangarObjects.ShuttlePositionY,
                    PositionZ = 0x7FFFFFFF,
                    HeadingXY = hangarObjects.ShuttleOrientation,
                    HeadingZ = 0,
                    ObjectProfile = hangarObjects.ShuttleObjectProfile
                };

                OptFile model = this.CreateModelOpt(shuttle, hangarObjects.ShuttlePositionZ, hangarFloorZ, hangarObjects.IsShuttleFloorInverted, hangarSkins, hangarObjects);

                if (model != null)
                {
                    MergeOpt(optFile, model);
                }
            }

            if (isRegularHangar)
            {
                var roofCrane = new HangarItem
                {
                    ModelIndex = 316,
                    Markings = 0,
                    PositionX = hangarObjects.HangarRoofCranePositionX,
                    PositionY = hangarObjects.HangarRoofCranePositionY,
                    PositionZ = hangarObjects.HangarRoofCranePositionZ,
                    HeadingXY = 0,
                    HeadingZ = 0,
                    ObjectProfile = "Default"
                };

                OptFile model = this.CreateModelOpt(roofCrane, 0, hangarFloorZ, isHangarFloorInverted, hangarSkins, hangarObjects);

                if (model != null)
                {
                    MergeOpt(optFile, model);
                }
            }

            optFile.CompactTextures();
            optFile.GenerateTexturesNames();

            return optFile;
        }

        private OptFile CreateModelOpt(HangarItem item, int itemOffsetZ, double hangarFloorZ, bool isHangarFloorInverted, HangarSkins hangarSkins, HangarObjects hangarObjects)
        {
            string name = (string)ExeModelIndexConverter.Default.Convert(item.ModelIndex, null, null, null);

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (hangarObjects.ObjectsReplace.TryGetValue(name, out string value))
            {
                name = value;
            }

            var objectProfiles = this.GetOptObjectProfiles(name);

            if (objectProfiles is null)
            {
                return null;
            }

            objectProfiles.TryGetValue(item.ObjectProfile, out IList<int> objectProfile);

            IList<string> skin = null;

            if (hangarSkins.TryGetValue(name, out Dictionary<int, IList<string>> skins))
            {
                if (!skins.TryGetValue(item.Markings, out skin))
                {
                    if (skins.ContainsKey(-1))
                    {
                        skin = skins[-1];
                    }
                }
            }

            var model = this.GetOptModel(name, item.Markings, objectProfile, skin);

            if (model is null)
            {
                return null;
            }

            var modelVisualOpt = model.File.Clone();

            foreach (var mesh in modelVisualOpt.Meshes)
            {
                if (mesh.Lods.Count == 0)
                {
                    continue;
                }

                for (int i = mesh.Lods.Count - 1; i >= 1; i--)
                {
                    mesh.Lods.RemoveAt(i);
                }

                var lod = mesh.Lods[0];
                lod.Distance = 0;

                foreach (var faceGroup in lod.FaceGroups)
                {
                    if (faceGroup.Textures.Count == 0)
                    {
                        continue;
                    }

                    int markings = item.Markings;

                    if (markings >= faceGroup.Textures.Count)
                    {
                        markings = faceGroup.Textures.Count - 1;
                    }

                    string texture = faceGroup.Textures[markings];

                    faceGroup.Textures.Clear();
                    faceGroup.Textures.Add(texture);
                }
            }

            for (int meshIndex = 0; meshIndex < modelVisualOpt.Meshes.Count; meshIndex++)
            {
                var mesh = model.File.Meshes[meshIndex];
                var sfoil = model.SFoils.Where(t => t.MeshIndex == meshIndex).FirstOrDefault() ?? new SFoil();

                double angle = sfoil.Angle * 360.0 / 255 * mesh.RotationScale.Look.LengthFactor();
                var transform = new RotateTransform3D(new AxisAngleRotation3D(mesh.RotationScale.Look.ToVector3D(), angle), mesh.RotationScale.Pivot.ToPoint3D());
                var transformRotation = new RotateTransform3D(new AxisAngleRotation3D(mesh.RotationScale.Look.ToVector3D(), angle));

                TransformMesh(modelVisualOpt.Meshes[meshIndex], transform, transformRotation);
            }

            double offsetX = item.PositionX;
            double offsetY = item.PositionY;
            double offsetZ;

            bool isFloorInverted = item.IsHangarFloorInverted ?? isHangarFloorInverted;

            if (item.IsOnFloor)
            {
                if (isFloorInverted)
                {
                    offsetZ = hangarFloorZ + hangarObjects.HangarFloorInvertedHeight - model.ClosedSFoilsElevationInverted;
                }
                else
                {
                    offsetZ = hangarFloorZ + model.ClosedSFoilsElevation;
                }
            }
            else
            {
                offsetZ = item.PositionZ;
            }

            int bridgeIndex = modelVisualOpt.Meshes.IndexOf(modelVisualOpt.Meshes.FirstOrDefault(t => t.Descriptor.MeshType == MeshType.Bridge));

            if (bridgeIndex != -1)
            {
                var mesh = model.File.Meshes[bridgeIndex];
                var sfoil = model.SFoils.Where(t => t.MeshIndex == bridgeIndex).FirstOrDefault() ?? new SFoil();

                double angle = sfoil.Angle * 360.0 / 255 * mesh.RotationScale.Look.LengthFactor();
                var transform = new RotateTransform3D(new AxisAngleRotation3D(mesh.RotationScale.Look.ToVector3D(), angle), mesh.RotationScale.Pivot.ToPoint3D());
                var transformRotation = new RotateTransform3D(new AxisAngleRotation3D(mesh.RotationScale.Look.ToVector3D(), angle));

                foreach (var modelMesh in modelVisualOpt.Meshes)
                {
                    TransformMesh(modelMesh, transform, transformRotation);
                }
            }

            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), item.HeadingZ * 360.0 / 65536)));
            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -item.HeadingXY * 360.0 / 65536)));
            transformGroup.Children.Add(new TranslateTransform3D(offsetY, -offsetX, offsetZ + itemOffsetZ));

            var transformGroupRotation = new Transform3DGroup();
            transformGroupRotation.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), item.HeadingZ * 360.0 / 65536)));
            transformGroupRotation.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -item.HeadingXY * 360.0 / 65536)));

            foreach (var modelMesh in modelVisualOpt.Meshes)
            {
                TransformMesh(modelMesh, transformGroup, transformGroupRotation);
            }

            return modelVisualOpt;
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
        }

        private static void MergeOpt(OptFile opt, OptFile import)
        {
            import = import.Clone();

            foreach (var mesh in import.Meshes)
            {
                mesh.EngineGlows.Clear();
                mesh.Hardpoints.Clear();
            }

            import.CompactBuffers();
            import.ComputeHitzones();
            import.CompactTextures();
            import.GenerateTexturesNames();

            string importName = opt.Textures.Count.ToString(CultureInfo.InvariantCulture) + "_";

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
    }
}
