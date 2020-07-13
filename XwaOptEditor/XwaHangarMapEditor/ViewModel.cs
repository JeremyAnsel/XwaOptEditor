using JeremyAnsel.Xwa.HooksConfig;
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
    class ViewModel : INotifyPropertyChanged
    {
        public const string DefaultText =
@"; Must contain at least 4 object line.
; Format is : model index, position X, position Y, position Z, heading XY, heading Z
; or : model index, markings, position X, position Y, position Z, heading XY, heading Z
; Numbers can be in decimal or hexadecimal (0x) notation.
; When position Z is set to 0x7FFFFFFF, this means that the object stands at the ground.

; Xwing
1, 0, 0, 0x7FFFFFFF, 0, 0
";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Update()
        {
            this.OnPropertyChanged(nameof(this.ObjectIndices));
            this.OnPropertyChanged(nameof(this.Text));
            this.OnPropertyChanged(nameof(this.Map));
            this.OnPropertyChanged(nameof(this.Map3D));
            this.OnPropertyChanged(nameof(this.HangarModel));
            this.OnPropertyChanged(nameof(this.HangarModel3D));
        }

        private SortedDictionary<string, OptModel> optModels = new SortedDictionary<string, OptModel>();

        private OptModel GetOptModel(string name)
        {
            OptModel model;
            if (!this.optModels.TryGetValue(name, out model))
            {
                string fileName = AppSettings.WorkingDirectory + "FLIGHTMODELS\\" + name + ".opt";

                if (!System.IO.File.Exists(fileName))
                {
                    return null;
                }

                model = new OptModel();
                model.File = OptFile.FromFile(fileName);
                model.Cache = new OptCache(model.File);
                model.SFoils = SFoil.GetSFoilsList(fileName);

                string ship = XwaHooksConfig.GetStringWithoutExtension(fileName);
                IList<string> lines;

                lines = XwaHooksConfig.GetFileLines(ship + "Size.txt");
                if (lines.Count == 0)
                {
                    lines = XwaHooksConfig.GetFileLines(ship + ".ini", "Size");
                }

                model.ClosedSFoilsElevation = XwaHooksConfig.GetFileKeyValueInt(lines, "ClosedSFoilsElevation");
                if (model.ClosedSFoilsElevation == 0)
                {
                    if (string.Equals(name, "BWing", StringComparison.OrdinalIgnoreCase))
                    {
                        model.ClosedSFoilsElevation = 50;
                    }
                    else
                    {
                        model.ClosedSFoilsElevation = model.File.SpanSize.Z / 2;
                    }
                }

                lines = XwaHooksConfig.GetFileLines(ship + "HangarObjects.txt");
                if (lines.Count == 0)
                {
                    lines = XwaHooksConfig.GetFileLines(ship + ".ini", "HangarObjects");
                }

                model.IsHangarFloorInverted = XwaHooksConfig.GetFileKeyValueInt(lines, "IsHangarFloorInverted") != 0;

                this.optModels.Add(name, model);
            }

            return model;
        }

        public IList<int> ObjectIndices
        {
            get
            {
                return Enumerable.Range(0, AppSettings.Objects?.Count ?? 0).ToList();
            }
        }

        private string textFileName;

        public string TextFileName
        {
            get
            {
                return this.textFileName;
            }

            set
            {
                if (this.textFileName == value)
                    return;

                this.textFileName = value;

                this.OnPropertyChanged();
            }
        }

        private string text = ViewModel.DefaultText;

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text == value)
                    return;

                this.text = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.Map));
                this.OnPropertyChanged(nameof(this.Map3D));
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
                    return;

                this.hangarModel = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HangarModel3D));
                this.OnPropertyChanged(nameof(this.Map3D));
            }
        }

        public HangarMap Map
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Text))
                {
                    return null;
                }

                return HangarMap.FromText(this.Text);
            }
        }

        public IList<Visual3D> Map3D
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Text))
                {
                    return new List<Visual3D>();
                }

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

                        isHangarFloorInverted = hangar.IsHangarFloorInverted;
                    }
                }

                HangarMap map = HangarMap.FromText(this.Text);

                var collection = new List<Visual3D>(map.Count);

                foreach (HangarItem item in map)
                {
                    Visual3D model = this.CreateModel3D(item, hangarFloorZ, isHangarFloorInverted);

                    if (model != null)
                    {
                        collection.Add(model);
                    }
                }

                return collection;
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

                var model3D = new OptVisual3D();
                model3D.Cache = hangar.Cache;
                model3D.SortingFrequency = .2;

                return model3D;
            }
        }

        private Visual3D CreateModel3D(HangarItem item, double hangarFloorZ, bool isHangarFloorInverted)
        {
            string name = (string)ExeModelIndexConverter.Default.Convert(item.ModelIndex, null, null, null);

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var model = this.GetOptModel(name);

            if (model == null)
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

            if (item.IsOnFloor)
            {
                if (isHangarFloorInverted)
                {
                    offsetZ = hangarFloorZ - model.ClosedSFoilsElevation;
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
            transformGroup.Children.Add(new TranslateTransform3D(offsetY, -offsetX, offsetZ));
            transformGroup.Freeze();

            modelVisual3D.Transform = transformGroup;

            return modelVisual3D;
        }

        public OptFile BuildOptMap()
        {
            var optFile = new OptFile();

            if (string.IsNullOrWhiteSpace(this.Text))
            {
                return null;
            }

            OptModel hangar = string.IsNullOrWhiteSpace(this.HangarModel) ? null : this.GetOptModel(this.HangarModel);

            double hangarFloorZ = 0;
            bool isHangarFloorInverted = false;

            if (hangar != null)
            {
                MergeOpt(optFile, hangar.File);

                Hardpoint hangarFloorHardpoint = hangar.File
                    .Meshes
                    .SelectMany(t => t.Hardpoints)
                    .Where(t => t.HardpointType == HardpointType.InsideHangar)
                    .FirstOrDefault();

                if (hangarFloorHardpoint != null)
                {
                    hangarFloorZ = hangarFloorHardpoint.Position.Z;
                }

                isHangarFloorInverted = hangar.IsHangarFloorInverted;
            }

            HangarMap map = HangarMap.FromText(this.Text);

            var collection = new List<Visual3D>(map.Count);

            foreach (HangarItem item in map)
            {
                OptFile model = this.CreateModelOpt(item, hangarFloorZ, isHangarFloorInverted);

                if (model != null)
                {
                    MergeOpt(optFile, model);
                }
            }

            optFile.CompactTextures();
            optFile.GenerateTexturesNames();

            return optFile;
        }

        private OptFile CreateModelOpt(HangarItem item, double hangarFloorZ, bool isHangarFloorInverted)
        {
            string name = (string)ExeModelIndexConverter.Default.Convert(item.ModelIndex, null, null, null);

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var model = this.GetOptModel(name);

            if (model == null)
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

            if (item.IsOnFloor)
            {
                if (isHangarFloorInverted)
                {
                    offsetZ = hangarFloorZ - model.ClosedSFoilsElevation;
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
            transformGroup.Children.Add(new TranslateTransform3D(offsetY, -offsetX, offsetZ));

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
