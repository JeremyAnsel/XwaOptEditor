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

        private SortedDictionary<string, OptFile> optFiles = new SortedDictionary<string, OptFile>();
        private SortedDictionary<string, OptCache> optCaches = new SortedDictionary<string, OptCache>();

        private Tuple<OptFile, OptCache> GetOptModel(string name)
        {
            OptFile file;
            if (!this.optFiles.TryGetValue(name, out file))
            {
                string fileName = AppSettings.WorkingDirectory + "FLIGHTMODELS\\" + name + ".opt";

                if (!System.IO.File.Exists(fileName))
                {
                    return null;
                }

                file = OptFile.FromFile(fileName);
                this.optFiles.Add(name, file);
            }

            OptCache cache;
            if (!this.optCaches.TryGetValue(name, out cache))
            {
                cache = new OptCache(file);
                this.optCaches.Add(name, cache);
            }

            return new Tuple<OptFile, OptCache>(file, cache);
        }

        public IList<int> ObjectIndices
        {
            get
            {
                return Enumerable.Range(0, (AppSettings.Objects?.Count).Value).ToList();
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

                if (!string.IsNullOrWhiteSpace(this.HangarModel))
                {
                    var hangar = this.GetOptModel(this.HangarModel);

                    if (hangar != null)
                    {
                        Hardpoint hangarFloorHardpoint = hangar.Item1
                            .Meshes
                            .SelectMany(t => t.Hardpoints)
                            .Where(t => t.HardpointType == HardpointType.InsideHangar)
                            .FirstOrDefault();

                        if (hangarFloorHardpoint != null)
                        {
                            hangarFloorZ = hangarFloorHardpoint.Position.Z;
                        }
                    }
                }

                HangarMap map = HangarMap.FromText(this.Text);

                var collection = new List<Visual3D>(map.Count);

                foreach (HangarItem item in map)
                {
                    Visual3D model = this.CreateModel3D(item, hangarFloorZ);

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
                model3D.Cache = hangar.Item2;
                model3D.SortingFrequency = .2;

                return model3D;
            }
        }

        private Visual3D CreateModel3D(HangarItem item, double hangarFloorZ)
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

            var model3D = new OptVisual3D();
            model3D.Cache = model.Item2;
            model3D.SortingFrequency = .2;

            double offsetX = item.PositionX;
            double offsetY = item.PositionY;
            double offsetZ;

            if (item.IsOnFloor)
            {
                offsetZ = hangarFloorZ + model.Item1.SpanSize.Z / 2;
            }
            else
            {
                offsetZ = item.PositionZ;
            }

            var transform = new Transform3DGroup();
            transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), item.HeadingZ * 360.0 / 65536)));
            transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -item.HeadingXY * 360.0 / 65536)));
            transform.Children.Add(new TranslateTransform3D(offsetY, -offsetX, offsetZ));
            transform.Freeze();

            model3D.Transform = transform;

            return model3D;
        }
    }
}
