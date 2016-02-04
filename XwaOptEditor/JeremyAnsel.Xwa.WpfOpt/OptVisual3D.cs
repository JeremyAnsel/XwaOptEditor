using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;

namespace JeremyAnsel.Xwa.WpfOpt
{
    public class OptVisual3D : SortingVisual3D
    {
        public OptVisual3D()
        {
            this.Method = SortingMethod.BoundingSphereSurface;
            this.CheckForOpaqueVisuals = true;
            this.SortingFrequency = 1;
        }

        public event EventHandler Changed;

        public event EventHandler ModelChanged;

        public event EventHandler AppearanceChanged;

        public Dictionary<Model3D, MeshLodFace> ModelToMeshLodFace { get; private set; }

        public OptCache Cache
        {
            get { return (OptCache)this.GetValue(CacheProperty); }
            set { this.SetValue(CacheProperty, value); }
        }

        public string FileName
        {
            get { return (string)this.GetValue(FileNameProperty); }
            set { this.SetValue(FileNameProperty, value); }
        }

        public OptFile File
        {
            get { return (OptFile)this.GetValue(FileProperty); }
            set { this.SetValue(FileProperty, value); }
        }

        public Mesh Mesh
        {
            get { return (Mesh)this.GetValue(MeshProperty); }
            set { this.SetValue(MeshProperty, value); }
        }

        public MeshLod Lod
        {
            get { return (MeshLod)this.GetValue(LodProperty); }
            set { this.SetValue(LodProperty, value); }
        }

        public float? Distance
        {
            get { return (float?)this.GetValue(DistanceProperty); }
            set { this.SetValue(DistanceProperty, value); }
        }

        public int Version
        {
            get { return (int)this.GetValue(VersionProperty); }
            set { this.SetValue(VersionProperty, value); }
        }

        public bool IsSolid
        {
            get { return (bool)this.GetValue(IsSolidProperty); }
            set { this.SetValue(IsSolidProperty, value); }
        }

        public bool IsWireframe
        {
            get { return (bool)this.GetValue(IsWireframeProperty); }
            set { this.SetValue(IsWireframeProperty, value); }
        }

        public static readonly DependencyProperty CacheProperty = DependencyProperty.Register("Cache", typeof(OptCache), typeof(OptVisual3D), new UIPropertyMetadata(null, ContentChanged));

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(OptVisual3D), new UIPropertyMetadata(null, ContentChanged));

        public static readonly DependencyProperty FileProperty = DependencyProperty.Register("File", typeof(OptFile), typeof(OptVisual3D), new UIPropertyMetadata(null, ContentChanged));

        public static readonly DependencyProperty MeshProperty = DependencyProperty.Register("Mesh", typeof(Mesh), typeof(OptVisual3D), new UIPropertyMetadata(null, ContentChanged));

        public static readonly DependencyProperty LodProperty = DependencyProperty.Register("Lod", typeof(MeshLod), typeof(OptVisual3D), new UIPropertyMetadata(null, ContentChanged));

        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(float?), typeof(OptVisual3D), new UIPropertyMetadata(null, ContentChanged));

        public static readonly DependencyProperty VersionProperty = DependencyProperty.Register("Version", typeof(int), typeof(OptVisual3D), new UIPropertyMetadata((int)0, ContentChanged));

        public static readonly DependencyProperty IsSolidProperty = DependencyProperty.Register("IsSolid", typeof(bool), typeof(OptVisual3D), new UIPropertyMetadata(true, ContentChanged));

        public static readonly DependencyProperty IsWireframeProperty = DependencyProperty.Register("IsWireframe", typeof(bool), typeof(OptVisual3D), new UIPropertyMetadata(false, ContentChanged));

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static void ContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var opt = (OptVisual3D)obj;

            switch (args.Property.Name)
            {
                case "FileName":
                    opt.File = null;

                    if (!string.IsNullOrEmpty(opt.FileName))
                    {
                        try
                        {
                            opt.File = OptFile.FromFile(opt.FileName);
                        }
                        catch (InvalidDataException)
                        {
                        }
                    }

                    break;

                case "File":
                    opt.Cache = null;

                    if (opt.File != null)
                    {
                        opt.Cache = new OptCache(opt.File);
                    }

                    break;

                case "Cache":
                case "Mesh":
                case "Lod":
                case "Distance":
                case "Version":
                case "IsSolid":
                case "IsWireframe":
                    if (opt.Distance == null)
                    {
                        opt.LoadOpt(opt.Mesh, opt.Lod, opt.Version);
                    }
                    else
                    {
                        opt.LoadOpt(opt.Mesh, opt.Distance.Value, opt.Version);
                    }

                    break;
            }

            if (opt.Changed != null)
            {
                opt.Changed(opt, EventArgs.Empty);
            }

            switch (args.Property.Name)
            {
                case "Cache":
                case "Mesh":
                case "Lod":
                case "Distance":
                    if (opt.ModelChanged != null)
                    {
                        opt.ModelChanged(opt, EventArgs.Empty);
                    }
                    break;

                case "Version":
                case "IsSolid":
                case "IsWireframe":
                    if (opt.AppearanceChanged != null)
                    {
                        opt.AppearanceChanged(opt, EventArgs.Empty);
                    }
                    break;
            }

        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        private void LoadOpt(Mesh mesh, float distance, int version)
        {
            this.Content = null;
            this.Children.Clear();
            this.ModelToMeshLodFace = new Dictionary<Model3D, MeshLodFace>();

            if (this.Cache == null || this.Cache.file == null)
            {
                return;
            }

            var opt = this.Cache.file;

            if (mesh == null)
            {
                if (this.IsSolid)
                {
                    for (int meshIndex = 0; meshIndex < opt.Meshes.Count; meshIndex++)
                    {
                        for (int lodIndex = 0; lodIndex < opt.Meshes[meshIndex].Lods.Count; lodIndex++)
                        {
                            if (opt.Meshes[meshIndex].Lods[lodIndex].Distance <= distance)
                            {
                                foreach (var model in CreateMeshModel(opt, meshIndex, lodIndex, version)
                                    .Where(t => t != null))
                                {
                                    this.Children.Add(model);
                                }
                                break;
                            }
                        }
                    }
                }

                if (this.IsWireframe)
                {
                    for (int meshIndex = 0; meshIndex < opt.Meshes.Count; meshIndex++)
                    {
                        for (int lodIndex = 0; lodIndex < opt.Meshes[meshIndex].Lods.Count; lodIndex++)
                        {
                            if (opt.Meshes[meshIndex].Lods[lodIndex].Distance <= distance)
                            {
                                foreach (var model in CreateMeshModelWireframe(opt, meshIndex, lodIndex)
                                    .Where(t => t != null))
                                {
                                    this.Children.Add(model);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                int meshIndex = opt.Meshes.IndexOf(mesh);

                if (meshIndex == -1)
                {
                    return;
                }

                if (this.IsSolid)
                {
                    for (int lodIndex = 0; lodIndex < mesh.Lods.Count; lodIndex++)
                    {
                        if (mesh.Lods[lodIndex].Distance <= distance)
                        {
                            foreach (var model in CreateMeshModel(opt, meshIndex, lodIndex, version)
                                .Where(t => t != null))
                            {
                                this.Children.Add(model);
                            }
                            break;
                        }
                    }
                }

                if (this.IsWireframe)
                {
                    for (int lodIndex = 0; lodIndex < mesh.Lods.Count; lodIndex++)
                    {
                        if (mesh.Lods[lodIndex].Distance <= distance)
                        {
                            foreach (var model in CreateMeshModelWireframe(opt, meshIndex, lodIndex)
                                .Where(t => t != null))
                            {
                                this.Children.Add(model);
                            }
                            break;
                        }
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        private void LoadOpt(Mesh mesh, MeshLod lod, int version)
        {
            this.Content = null;
            this.Children.Clear();
            this.ModelToMeshLodFace = new Dictionary<Model3D, MeshLodFace>();

            if (this.Cache == null || this.Cache.file == null)
            {
                return;
            }

            var opt = this.Cache.file;

            if (mesh == null && lod == null)
            {
                if (this.IsSolid)
                {
                    foreach (var model in Enumerable
                        .Range(0, opt.Meshes.Count)
                        .SelectMany(t => CreateMeshModel(opt, t, 0, version))
                        .Where(t => t != null))
                    {
                        this.Children.Add(model);
                    }
                }

                if (this.IsWireframe)
                {
                    foreach (var model in Enumerable
                        .Range(0, opt.Meshes.Count)
                        .SelectMany(t => CreateMeshModelWireframe(opt, t, 0))
                        .Where(t => t != null))
                    {
                        this.Children.Add(model);
                    }
                }
            }
            else if (lod == null)
            {
                int meshIndex = opt.Meshes.IndexOf(mesh);

                if (meshIndex == -1)
                {
                    return;
                }

                if (this.IsSolid)
                {
                    foreach (var model in CreateMeshModel(opt, meshIndex, 0, version)
                        .Where(t => t != null))
                    {
                        this.Children.Add(model);
                    }
                }

                if (this.IsWireframe)
                {
                    foreach (var model in CreateMeshModelWireframe(opt, meshIndex, 0)
                        .Where(t => t != null))
                    {
                        this.Children.Add(model);
                    }
                }
            }
            else
            {
                int meshIndex = opt.Meshes.IndexOf(mesh);

                if (meshIndex == -1)
                {
                    return;
                }

                int lodIndex = mesh.Lods.IndexOf(lod);

                if (lodIndex == -1)
                {
                    return;
                }

                if (this.IsSolid)
                {
                    foreach (var model in CreateMeshModel(opt, meshIndex, lodIndex, version)
                        .Where(t => t != null))
                    {
                        this.Children.Add(model);
                    }
                }

                if (this.IsWireframe)
                {
                    foreach (var model in CreateMeshModelWireframe(opt, meshIndex, lodIndex)
                        .Where(t => t != null))
                    {
                        this.Children.Add(model);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
        private List<ModelVisual3D> CreateMeshModel(OptFile opt, int meshIndex, int lodIndex, int version)
        {
            if (opt == null ||
                opt.Meshes.ElementAtOrDefault(meshIndex) == null ||
                opt.Meshes[meshIndex].Lods.ElementAtOrDefault(lodIndex) == null)
            {
                return new List<ModelVisual3D>();
            }

            var cache = this.Cache;

            var mesh = opt.Meshes[meshIndex];
            var lod = mesh.Lods[lodIndex];

            if (meshIndex >= cache.meshes.Length || lodIndex >= cache.meshes[meshIndex].Length || lod.FaceGroups.Count > cache.meshes[meshIndex][lodIndex].Length)
            {
                return new List<ModelVisual3D>();
            }

            List<ModelVisual3D> group = new List<ModelVisual3D>();

            for (int faceGroupIndex = 0; faceGroupIndex < lod.FaceGroups.Count; faceGroupIndex++)
            {
                var faceGroup = lod.FaceGroups[faceGroupIndex];

                Material texture = null;
                bool alpha = false;

                if (faceGroup.Textures.Count != 0)
                {
                    int currentVersion = version;

                    if (version < 0 || version >= faceGroup.Textures.Count)
                    {
                        currentVersion = faceGroup.Textures.Count - 1;
                    }

                    string textureName = faceGroup.Textures[currentVersion];

                    if (cache.textures.ContainsKey(textureName))
                    {
                        texture = cache.textures[textureName];
                        alpha = opt.Textures[textureName].HasAlpha;
                    }
                }

                var geometries = cache.meshes[meshIndex][lodIndex][faceGroupIndex];

                if (alpha)
                {
                    for (int i = 1; i < geometries.Length; i++)
                    {
                        GeometryModel3D model = new GeometryModel3D();

                        model.Geometry = geometries[i];
                        model.Material = texture == null ? cache.nullTexture : texture;
                        model.BackMaterial = texture == null ? cache.nullTexture : texture;

                        model.Freeze();

                        group.Add(new ModelVisual3D() { Content = model });

                        this.ModelToMeshLodFace.Add(model, new MeshLodFace(opt.Meshes[meshIndex], opt.Meshes[meshIndex].Lods[lodIndex], faceGroup));
                    }
                }
                else
                {
                    GeometryModel3D model = new GeometryModel3D();

                    model.Geometry = geometries[0];
                    model.Material = texture == null ? cache.nullTexture : texture;

                    model.Freeze();

                    group.Add(new ModelVisual3D() { Content = model });

                    this.ModelToMeshLodFace.Add(model, new MeshLodFace(opt.Meshes[meshIndex], opt.Meshes[meshIndex].Lods[lodIndex], faceGroup));
                }
            }

            return group;
        }

        private List<ModelVisual3D> CreateMeshModelWireframe(OptFile opt, int meshIndex, int lodIndex)
        {
            if (opt == null ||
                opt.Meshes.ElementAtOrDefault(meshIndex) == null ||
                opt.Meshes[meshIndex].Lods.ElementAtOrDefault(lodIndex) == null)
            {
                return new List<ModelVisual3D>();
            }

            var cache = this.Cache;

            if (meshIndex >= cache.meshesWireframes.Length || lodIndex >= cache.meshesWireframes[meshIndex].Length)
            {
                return new List<ModelVisual3D>();
            }


            return new List<ModelVisual3D>
            {
                new LinesVisual3D
                {
                    Points = new Point3DCollection(cache.meshesWireframes[meshIndex][lodIndex]),
                    DepthOffset = 0.0001,
                    Color = Colors.White
                }
            };
        }
    }
}
