using JeremyAnsel.Xwa.HooksConfig;
using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    }
}
