using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using JeremyAnsel.Xwa.Opt;

namespace JeremyAnsel.Xwa.WpfOpt
{
    public sealed class OptCache
    {
        internal OptFile file;

        internal Material nullTexture;

        internal Dictionary<string, Material> textures;

        internal MeshGeometry3D[][][][] meshes;

        internal IList<Point3D>[][] meshesWireframes;

        public OptCache(OptFile opt)
        {
            this.file = opt;
            this.BuildTextures(opt);
            this.BuildMeshes(opt);
            this.BuildMeshesWireframes(opt);
        }

        private static BitmapSource CreateTexture(OptFile opt, string name)
        {
            if (!opt.Textures.ContainsKey(name))
            {
                return null;
            }

            var texture = opt.Textures[name];

            int size = texture.Width * texture.Height;
            int bpp = texture.BitsPerPixel;

            if (bpp == 8)
            {
                var palette = new BitmapPalette(Enumerable.Range(0, 256)
                    .Select(i =>
                    {
                        ushort c = BitConverter.ToUInt16(texture.Palette, 8 * 512 + i * 2);

                        byte r = (byte)((c & 0xF800) >> 11);
                        byte g = (byte)((c & 0x7E0) >> 5);
                        byte b = (byte)(c & 0x1F);

                        r = (byte)((r * (0xffU * 2) + 0x1fU) / (0x1fU * 2));
                        g = (byte)((g * (0xffU * 2) + 0x3fU) / (0x3fU * 2));
                        b = (byte)((b * (0xffU * 2) + 0x1fU) / (0x1fU * 2));

                        return Color.FromRgb(r, g, b);
                    })
                    .ToList());

                if (texture.AlphaIllumData == null)
                {
                    byte[] imageData = new byte[size];
                    Array.Copy(texture.ImageData, imageData, size);

                    return BitmapSource.Create(texture.Width, texture.Height, 96, 96, PixelFormats.Indexed8, palette, imageData, texture.Width);
                }
                else
                {
                    byte[] imageData = new byte[size * 4];

                    for (int i = 0; i < size; i++)
                    {
                        int colorIndex = texture.ImageData[i];

                        imageData[i * 4 + 0] = palette.Colors[colorIndex].B;
                        imageData[i * 4 + 1] = palette.Colors[colorIndex].G;
                        imageData[i * 4 + 2] = palette.Colors[colorIndex].R;
                        imageData[i * 4 + 3] = texture.AlphaIllumData[i];
                    }

                    return BitmapSource.Create(texture.Width, texture.Height, 96, 96, PixelFormats.Bgra32, null, imageData, texture.Width * 4);
                }
            }
            else if (bpp == 32)
            {
                byte[] imageData = new byte[size * 4];
                Array.Copy(texture.ImageData, imageData, size * 4);

                return BitmapSource.Create(texture.Width, texture.Height, 96, 96, PixelFormats.Bgra32, null, imageData, texture.Width * 4);
            }
            else
            {
                return null;
            }
        }

        private void BuildTextures(OptFile opt)
        {
            this.nullTexture = null;
            this.textures = null;

            if (opt == null)
            {
                return;
            }

            this.nullTexture = new DiffuseMaterial(Brushes.White);

            this.textures = new Dictionary<string, Material>();

            foreach (var texture in opt.Textures.Values)
            {
                var image = CreateTexture(opt, texture.Name);
                image.Freeze();

                var brush = new ImageBrush(image)
                {
                    ViewportUnits = BrushMappingMode.Absolute,
                    Stretch = Stretch.Fill,
                    TileMode = TileMode.Tile,
                    Opacity = texture.HasAlpha ? 0.999 : 1.0
                };

                brush.Freeze();

                var material = new DiffuseMaterial(brush);
                material.Freeze();

                this.textures.Add(texture.Name, material);
            }
        }

        private static void MergeGeometry(MeshGeometry3D mesh1, MeshGeometry3D mesh2)
        {
            int index = mesh1.Positions.Count;

            foreach (var v in mesh2.Positions)
            {
                mesh1.Positions.Add(v);
            }

            foreach (var v in mesh2.Normals)
            {
                mesh1.Normals.Add(v);
            }

            foreach (var v in mesh2.TextureCoordinates)
            {
                mesh1.TextureCoordinates.Add(v);
            }

            foreach (var t in mesh2.TriangleIndices)
            {
                mesh1.TriangleIndices.Add(index + t);
            }
        }

        private void BuildMeshes(OptFile opt)
        {
            this.meshes = null;

            if (opt == null)
            {
                return;
            }

            this.meshes = new MeshGeometry3D[opt.Meshes.Count][][][];

            for (int meshIndex = 0; meshIndex < opt.Meshes.Count; meshIndex++)
            {
                var mesh = opt.Meshes[meshIndex];

                var positions = new Point3DCollection(
                    mesh.Vertices
                    .Select(t => new Point3D(-t.Y, -t.X, t.Z)));

                var normals = new Vector3DCollection(
                    mesh.VertexNormals
                    .Select(t => new Vector3D(-t.Y, -t.X, t.Z)));

                var textureCoordinates = new PointCollection(
                    mesh.TextureCoordinates
                    .Select(t => new Point(t.U, -t.V)));

                this.meshes[meshIndex] = new MeshGeometry3D[mesh.Lods.Count][][];

                for (int lodIndex = 0; lodIndex < mesh.Lods.Count; lodIndex++)
                {
                    var lod = mesh.Lods[lodIndex];

                    this.meshes[meshIndex][lodIndex] = new MeshGeometry3D[lod.FaceGroups.Count][];

                    for (int faceGroupIndex = 0; faceGroupIndex < lod.FaceGroups.Count; faceGroupIndex++)
                    {
                        var faceGroup = lod.FaceGroups[faceGroupIndex];

                        MeshGeometry3D[] geometries = new MeshGeometry3D[faceGroup.Faces.Count + 1];

                        for (int faceIndex = 0; faceIndex < faceGroup.Faces.Count; faceIndex++)
                        {
                            var face = faceGroup.Faces[faceIndex];

                            MeshGeometry3D geometry = new MeshGeometry3D();
                            int index = 0;

                            Index positionsIndex = face.VerticesIndex;
                            Index normalsIndex = face.VertexNormalsIndex;
                            Index textureCoordinatesIndex = face.TextureCoordinatesIndex;

                            geometry.Positions.Add(positions.ElementAtOrDefault(positionsIndex.A));
                            geometry.Normals.Add(normals.ElementAtOrDefault(normalsIndex.A));
                            geometry.TextureCoordinates.Add(textureCoordinates.ElementAtOrDefault(textureCoordinatesIndex.A));
                            geometry.TriangleIndices.Add(index);
                            index++;

                            geometry.Positions.Add(positions.ElementAtOrDefault(positionsIndex.B));
                            geometry.Normals.Add(normals.ElementAtOrDefault(normalsIndex.B));
                            geometry.TextureCoordinates.Add(textureCoordinates.ElementAtOrDefault(textureCoordinatesIndex.B));
                            geometry.TriangleIndices.Add(index);
                            index++;

                            geometry.Positions.Add(positions.ElementAtOrDefault(positionsIndex.C));
                            geometry.Normals.Add(normals.ElementAtOrDefault(normalsIndex.C));
                            geometry.TextureCoordinates.Add(textureCoordinates.ElementAtOrDefault(textureCoordinatesIndex.C));
                            geometry.TriangleIndices.Add(index);
                            index++;

                            if (positionsIndex.D >= 0)
                            {
                                geometry.TriangleIndices.Add(index - 3);
                                geometry.TriangleIndices.Add(index - 1);

                                geometry.Positions.Add(positions.ElementAtOrDefault(positionsIndex.D));
                                geometry.Normals.Add(normals.ElementAtOrDefault(normalsIndex.D));
                                geometry.TextureCoordinates.Add(textureCoordinates.ElementAtOrDefault(textureCoordinatesIndex.D));
                                geometry.TriangleIndices.Add(index);
                                index++;
                            }

                            geometry.Freeze();
                            geometries[1 + faceIndex] = geometry;
                        }

                        MeshGeometry3D geometryGroup = new MeshGeometry3D();

                        for (int i = 1; i < geometries.Length; i++)
                        {
                            MergeGeometry(geometryGroup, geometries[i]);
                        }

                        geometryGroup.Freeze();
                        geometries[0] = geometryGroup;

                        this.meshes[meshIndex][lodIndex][faceGroupIndex] = geometries;
                    }
                }
            }
        }

        private void BuildMeshesWireframes(OptFile opt)
        {
            this.meshesWireframes = null;

            if (opt == null)
            {
                return;
            }

            this.meshesWireframes = opt.Meshes
                .AsParallel()
                .AsOrdered()
                .Select(mesh =>
                {
                    var positions = new Point3DCollection(
                        mesh.Vertices
                        .Select(t => new Point3D(-t.Y, -t.X, t.Z)));

                    var wireframes = new IList<Point3D>[mesh.Lods.Count];

                    for (int lodIndex = 0; lodIndex < mesh.Lods.Count; lodIndex++)
                    {
                        var lod = mesh.Lods[lodIndex];

                        var lines = new List<Tuple<int, int>>(lod.TrianglesCount * 3);

                        var addLine = new Action<int, int>((a, b) =>
                        {
                            if (a < b)
                            {
                                lines.Add(new Tuple<int, int>(a, b));
                            }
                            else
                            {
                                lines.Add(new Tuple<int, int>(b, a));
                            }
                        });

                        for (int faceGroupIndex = 0; faceGroupIndex < lod.FaceGroups.Count; faceGroupIndex++)
                        {
                            var faceGroup = lod.FaceGroups[faceGroupIndex];

                            for (int faceIndex = 0; faceIndex < faceGroup.Faces.Count; faceIndex++)
                            {
                                var face = faceGroup.Faces[faceIndex];

                                Index positionsIndex = face.VerticesIndex;

                                addLine(positionsIndex.A, positionsIndex.B);
                                addLine(positionsIndex.B, positionsIndex.C);

                                if (positionsIndex.D < 0)
                                {
                                    addLine(positionsIndex.C, positionsIndex.A);
                                }
                                else
                                {
                                    addLine(positionsIndex.C, positionsIndex.D);
                                    addLine(positionsIndex.D, positionsIndex.A);
                                }
                            }
                        }

                        var points = lines
                            .Distinct()
                            .SelectMany(t => new List<Point3D>
                        {
                            positions.ElementAtOrDefault(t.Item1),
                            positions.ElementAtOrDefault(t.Item2)
                        })
                            .ToList();

                        wireframes[lodIndex] = points;
                    }

                    return wireframes;
                })
                .ToArray();
        }
    }
}
