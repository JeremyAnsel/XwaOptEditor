using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using JeremyAnsel.Media.An8;
using JeremyAnsel.Xwa.Opt;
using System.Threading.Tasks;

namespace OptAn8Converter
{
    public static class Converter
    {
        public static void OptToAn8(OptFile opt, string an8Path)
        {
            Converter.OptToAn8(opt, an8Path, true, null);
        }

        public static void OptToAn8(OptFile opt, string an8Path, Action<string> notify)
        {
            Converter.OptToAn8(opt, an8Path, true, notify);
        }

        public static void OptToAn8(OptFile opt, string an8Path, bool scale)
        {
            Converter.OptToAn8(opt, an8Path, scale, null);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public static void OptToAn8(OptFile opt, string an8Path, bool scale, Action<string> notify)
        {
            if (opt == null)
            {
                throw new ArgumentNullException("opt");
            }

            string an8Directory = Path.GetDirectoryName(an8Path);
            string an8Name = Path.GetFileNameWithoutExtension(an8Path);

            if (notify != null)
            {
                notify(string.Format(CultureInfo.InvariantCulture, "Exporting {0}.an8...", an8Name));
            }

            foreach (var texture in opt.Textures.Values)
            {
                texture.Save(Path.Combine(an8Directory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}.png", an8Name, texture.Name)));

                if (texture.HasAlpha)
                {
                    texture.SaveAlphaMap(Path.Combine(an8Directory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}_alpha.png", an8Name, texture.Name)));
                }

                if (texture.IsIlluminated)
                {
                    texture.SaveIllumMap(Path.Combine(an8Directory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}_illum.png", an8Name, texture.Name)));
                }
            }

            var distances = opt.Meshes
                .SelectMany(t => t.Lods)
                .Select(t => t.Distance)
                .Distinct()
                .OrderByDescending(t => t)
                .ToArray();

            int versions = opt.MaxTextureVersion;

            var items = new List<Tuple<int, int>>();
            for (int distance = 0; distance < distances.Length; distance++)
            {
                for (int version = 0; version < versions; version++)
                {
                    items.Add(Tuple.Create(distance, version));
                }
            }

            foreach (var item in items)
            {
                int distance = item.Item1;
                int version = item.Item2;

                if (notify != null)
                {
                    notify(string.Format(CultureInfo.InvariantCulture, "Exporting {0}_{1}_{2}.an8...", an8Name, distance, version));
                }

                var an8 = new An8File();

                foreach (var texture in opt.Textures.Values)
                {
                    var an8Texture = new An8Texture();
                    an8Texture.Name = an8Name + "_" + texture.Name;
                    an8Texture.Files.Add(string.Concat(an8Name, "_", texture.Name, ".png"));
                    an8.Textures.Add(an8Texture);

                    var an8Material = new An8Material();
                    an8Material.Name = an8Name + "_" + texture.Name;
                    an8Material.FrontSurface = new An8Surface();
                    an8Material.FrontSurface.Diffuse = new An8MaterialColor
                    {
                        TextureName = an8Name + "_" + texture.Name,
                        TextureParams = new An8TextureParams
                        {
                            AlphaMode = texture.HasAlpha ? An8AlphaMode.Final : An8AlphaMode.None,
                            BlendMode = An8BlendMode.Darken
                        }
                    };

                    an8.Materials.Add(an8Material);
                }

                var an8Object = new An8Object();
                an8Object.Name = Path.GetFileNameWithoutExtension(opt.FileName);
                an8.Objects.Add(an8Object);

                int objectsIndex = 0;

                foreach (var mesh in opt.Meshes)
                {
                    var lod = mesh.Lods.FirstOrDefault(t => t.Distance <= distances[distance]);

                    if (lod == null)
                    {
                        continue;
                    }

                    var an8Mesh = new An8Mesh();
                    an8Mesh.Name = string.Format(CultureInfo.InvariantCulture, "{0:D3}.{1}", objectsIndex, mesh.Descriptor.MeshType);
                    an8Object.Components.Add(an8Mesh);
                    objectsIndex++;

                    foreach (var texture in lod.FaceGroups
                        .SelectMany(t => t.Textures)
                        .Distinct())
                    {
                        an8Mesh.MaterialList.Add(an8Name + "_" + texture);
                    }

                    if (scale)
                    {
                        foreach (var v in mesh.Vertices)
                        {
                            an8Mesh.Points.Add(new An8Point
                            {
                                X = v.X * OptFile.ScaleFactor,
                                Y = v.Z * OptFile.ScaleFactor,
                                Z = v.Y * OptFile.ScaleFactor
                            });
                        }
                    }
                    else
                    {
                        foreach (var v in mesh.Vertices)
                        {
                            an8Mesh.Points.Add(new An8Point
                            {
                                X = v.X,
                                Y = v.Z,
                                Z = v.Y
                            });
                        }
                    }


                    foreach (var v in mesh.TextureCoordinates)
                    {
                        an8Mesh.TexCoords.Add(new An8TexCoord
                        {
                            U = v.U,
                            V = v.V
                        });
                    }

                    foreach (var v in mesh.VertexNormals)
                    {
                        an8Mesh.Normals.Add(new An8Point
                        {
                            X = v.X,
                            Y = v.Z,
                            Z = v.Y
                        });
                    }

                    int verticesIndex = 0;
                    int verticesTexIndex = 0;
                    int verticesNormalIndex = 0;

                    foreach (var faceGroup in lod.FaceGroups)
                    {
                        int materialIndex = 0;

                        if (faceGroup.Textures.Count > 0)
                        {
                            int currentVersion = version;

                            if (version < 0 || version >= faceGroup.Textures.Count)
                            {
                                currentVersion = faceGroup.Textures.Count - 1;
                            }

                            materialIndex = an8Mesh.MaterialList.IndexOf(an8Name + "_" + faceGroup.Textures[currentVersion]);
                        }

                        foreach (var face in faceGroup.Faces)
                        {
                            if (face.VerticesIndex.D < 0)
                            {
                                var an8Face = new An8Face
                                {
                                    MaterialIndex = materialIndex,
                                    FlatNormalIndex = -1
                                };

                                an8Face.PointIndexes = new int[]
                                {
                                    verticesIndex + face.VerticesIndex.A,
                                    verticesIndex + face.VerticesIndex.B,
                                    verticesIndex + face.VerticesIndex.C
                                };

                                an8Face.TexCoordIndexes = new int[]
                                {
                                    verticesTexIndex + face.TextureCoordinatesIndex.A,
                                    verticesTexIndex + face.TextureCoordinatesIndex.B,
                                    verticesTexIndex + face.TextureCoordinatesIndex.C
                                };

                                an8Face.NormalIndexes = new int[]
                                {
                                    verticesNormalIndex + face.VertexNormalsIndex.A,
                                    verticesNormalIndex + face.VertexNormalsIndex.B,
                                    verticesNormalIndex + face.VertexNormalsIndex.C
                                };

                                an8Mesh.Faces.Add(an8Face);
                            }
                            else
                            {
                                var an8Face = new An8Face
                                {
                                    MaterialIndex = materialIndex,
                                    FlatNormalIndex = -1
                                };

                                an8Face.PointIndexes = new int[]
                                {
                                    verticesIndex + face.VerticesIndex.A,
                                    verticesIndex + face.VerticesIndex.B,
                                    verticesIndex + face.VerticesIndex.C,
                                    verticesIndex + face.VerticesIndex.D
                                };

                                an8Face.TexCoordIndexes = new int[]
                                {
                                    verticesTexIndex + face.TextureCoordinatesIndex.A,
                                    verticesTexIndex + face.TextureCoordinatesIndex.B,
                                    verticesTexIndex + face.TextureCoordinatesIndex.C,
                                    verticesTexIndex + face.TextureCoordinatesIndex.D
                                };

                                an8Face.NormalIndexes = new int[]
                                {
                                    verticesNormalIndex + face.VertexNormalsIndex.A,
                                    verticesNormalIndex + face.VertexNormalsIndex.B,
                                    verticesNormalIndex + face.VertexNormalsIndex.C,
                                    verticesNormalIndex + face.VertexNormalsIndex.D
                                };

                                an8Mesh.Faces.Add(an8Face);
                            }
                        }
                    }

                    verticesIndex += mesh.Vertices.Count;
                    verticesTexIndex += mesh.TextureCoordinates.Count;
                    verticesNormalIndex += mesh.VertexNormals.Count;
                }

                an8.Save(Path.Combine(an8Directory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}.an8", an8Name, distance, version)));
            }
        }

        public static OptFile An8ToOpt(string an8Path)
        {
            return Converter.An8ToOpt(an8Path, true);
        }

        [SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals")]
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public static OptFile An8ToOpt(string an8Path, bool scale)
        {
            string an8Directory = Path.GetDirectoryName(an8Path);

            var an8 = An8File.FromFile(an8Path);
            var opt = new OptFile();

            foreach (var mesh in an8.Objects
                .SelectMany(t => t.Components)
                .SelectMany(t => Converter.EnumMeshes(t)))
            {
                var optMesh = new Mesh();
                opt.Meshes.Add(optMesh);

                if (scale)
                {
                    foreach (var v in mesh.Points)
                    {
                        optMesh.Vertices.Add(new Vector(v.X / OptFile.ScaleFactor, v.Z / OptFile.ScaleFactor, v.Y / OptFile.ScaleFactor));
                    }
                }
                else
                {
                    foreach (var v in mesh.Points)
                    {
                        optMesh.Vertices.Add(new Vector(v.X, v.Z, v.Y));
                    }
                }

                foreach (var v in mesh.TexCoords)
                {
                    optMesh.TextureCoordinates.Add(new TextureCoordinates(v.U, v.V));
                }

                foreach (var v in mesh.Normals)
                {
                    optMesh.VertexNormals.Add(new Vector(v.X, v.Z, v.Y));
                }

                optMesh.TextureCoordinates.Add(new TextureCoordinates(0, 0));
                optMesh.TextureCoordinates.Add(new TextureCoordinates(1, 0));
                optMesh.TextureCoordinates.Add(new TextureCoordinates(1, 1));
                optMesh.TextureCoordinates.Add(new TextureCoordinates(0, 1));

                var optLod = new MeshLod();
                optMesh.Lods.Add(optLod);

                foreach (var face in mesh.Faces)
                {
                    if (face.PointIndexes.Length < 3)
                    {
                        continue;
                    }

                    bool isQuad = face.PointIndexes.Length > 3;

                    var optFaceGroup = new FaceGroup();
                    optLod.FaceGroups.Add(optFaceGroup);

                    var materialName = mesh.MaterialList.ElementAtOrDefault(face.MaterialIndex);

                    if (!string.IsNullOrEmpty(materialName))
                    {
                        optFaceGroup.Textures.Add(materialName);
                    }

                    Indices verticesIndex = new Indices(
                        face.PointIndexes[0],
                        face.PointIndexes[1],
                        face.PointIndexes[2],
                        isQuad ? face.PointIndexes[3] : -1);

                    if (verticesIndex.A >= optMesh.Vertices.Count)
                    {
                        verticesIndex.A = 0;
                    }

                    if (verticesIndex.B >= optMesh.Vertices.Count)
                    {
                        verticesIndex.B = 0;
                    }

                    if (verticesIndex.C >= optMesh.Vertices.Count)
                    {
                        verticesIndex.C = 0;
                    }

                    if (verticesIndex.D >= optMesh.Vertices.Count)
                    {
                        verticesIndex.D = 0;
                    }

                    Indices textureCoordinatesIndex = new Indices(
                        face.TexCoordIndexes[0],
                        face.TexCoordIndexes[1],
                        face.TexCoordIndexes[2],
                        isQuad ? face.TexCoordIndexes[3] : -1);

                    if (textureCoordinatesIndex.A >= optMesh.TextureCoordinates.Count)
                    {
                        textureCoordinatesIndex.A = 0;
                    }

                    if (textureCoordinatesIndex.B >= optMesh.TextureCoordinates.Count)
                    {
                        textureCoordinatesIndex.B = 0;
                    }

                    if (textureCoordinatesIndex.C >= optMesh.TextureCoordinates.Count)
                    {
                        textureCoordinatesIndex.C = 0;
                    }

                    if (textureCoordinatesIndex.D >= optMesh.TextureCoordinates.Count)
                    {
                        textureCoordinatesIndex.D = 0;
                    }

                    Indices vertexNormalsIndex = new Indices(
                        face.NormalIndexes[0],
                        face.NormalIndexes[1],
                        face.NormalIndexes[2],
                        isQuad ? face.NormalIndexes[3] : -1);

                    if (vertexNormalsIndex.A >= optMesh.VertexNormals.Count)
                    {
                        vertexNormalsIndex.A = 0;
                    }

                    if (vertexNormalsIndex.B >= optMesh.VertexNormals.Count)
                    {
                        vertexNormalsIndex.B = 0;
                    }

                    if (vertexNormalsIndex.C >= optMesh.VertexNormals.Count)
                    {
                        vertexNormalsIndex.C = 0;
                    }

                    if (vertexNormalsIndex.D >= optMesh.VertexNormals.Count)
                    {
                        vertexNormalsIndex.D = 0;
                    }

                    if (textureCoordinatesIndex.A < 0 || textureCoordinatesIndex.B < 0 || textureCoordinatesIndex.C < 0 || (verticesIndex.D >= 0 && textureCoordinatesIndex.D < 0))
                    {
                        textureCoordinatesIndex.A = optMesh.TextureCoordinates.Count - 4;
                        textureCoordinatesIndex.B = optMesh.TextureCoordinates.Count - 3;
                        textureCoordinatesIndex.C = optMesh.TextureCoordinates.Count - 2;
                        textureCoordinatesIndex.D = verticesIndex.D < 0 ? -1 : optMesh.TextureCoordinates.Count - 1;
                    }

                    Vector normal = Vector.Normal(
                        optMesh.Vertices.ElementAtOrDefault(verticesIndex.A),
                        optMesh.Vertices.ElementAtOrDefault(verticesIndex.B),
                        optMesh.Vertices.ElementAtOrDefault(verticesIndex.C));

                    if (vertexNormalsIndex.A < 0 || vertexNormalsIndex.B < 0 || vertexNormalsIndex.C < 0 || (verticesIndex.D >= 0 && vertexNormalsIndex.D < 0))
                    {
                        optMesh.VertexNormals.Add(normal);

                        vertexNormalsIndex.A = optMesh.VertexNormals.Count - 1;
                        vertexNormalsIndex.B = optMesh.VertexNormals.Count - 1;
                        vertexNormalsIndex.C = optMesh.VertexNormals.Count - 1;
                        vertexNormalsIndex.D = verticesIndex.D < 0 ? -1 : optMesh.VertexNormals.Count - 1;
                    }

                    var optFace = new Face()
                    {
                        VerticesIndex = verticesIndex,
                        TextureCoordinatesIndex = textureCoordinatesIndex,
                        VertexNormalsIndex = vertexNormalsIndex,
                        Normal = normal
                    };

                    optFaceGroup.Faces.Add(optFace);
                }
            }

            opt.CompactBuffers();
            opt.ComputeHitzones();

            foreach (var material in an8.Materials
                .Concat(an8.Objects.SelectMany(t => t.Materials))
                .Where(t => t.FrontSurface != null)
                .Select(t => new
                {
                    Name = t.Name,
                    Diffuse = t.FrontSurface.Diffuse,
                    Alpha = t.FrontSurface.Alpha
                }))
            {
                var an8Texture = material.Diffuse.TextureName != null ?
                    an8.Textures.FirstOrDefault(t => string.Equals(t.Name, material.Diffuse.TextureName, StringComparison.Ordinal)) :
                    null;

                Texture texture;
                int bpp;

                if (an8Texture == null)
                {
                    byte r = material.Diffuse.Red;
                    byte g = material.Diffuse.Green;
                    byte b = material.Diffuse.Blue;

                    int width = 8;
                    int height = 8;
                    int length = width * height;
                    byte[] data = new byte[length * 4];

                    for (int i = 0; i < length; i++)
                    {
                        data[i * 4 + 0] = b;
                        data[i * 4 + 1] = g;
                        data[i * 4 + 2] = r;
                        data[i * 4 + 3] = 255;
                    }

                    texture = new Texture();
                    texture.Name = material.Name;
                    texture.Width = width;
                    texture.Height = height;
                    texture.ImageData = data;

                    texture.Convert32To8();
                    bpp = 8;
                }
                else
                {
                    string colorFileName = Path.Combine(an8Directory, Path.GetFileName(an8Texture.Files[0]));

                    texture = Texture.FromFile(colorFileName);
                    texture.Name = material.Name;

                    bpp = texture.BitsPerPixel;
                }

                if (material.Alpha > 0 && material.Alpha < 255)
                {
                    texture.Convert8To32();

                    byte alpha = (byte)material.Alpha;

                    int length = texture.Width * texture.Height;

                    var data = texture.ImageData;

                    for (int i = 0; i < length; i++)
                    {
                        data[i * 4 + 3] = alpha;
                    }

                    texture.Palette[2] = 0xff;
                }

                texture.GenerateMipmaps();

                if (bpp == 8)
                {
                    texture.Convert32To8();
                }

                opt.Textures.Add(texture.Name, texture);
            }

            return opt;
        }

        private static IEnumerable<An8Mesh> EnumMeshes(An8Component component)
        {
            var mesh = component as An8Mesh;

            if (mesh != null)
            {
                yield return mesh;
            }

            var group = component as An8Group;

            if (group != null)
            {
                foreach (var subMesh in group.Components.SelectMany(t => Converter.EnumMeshes(t)))
                {
                    yield return subMesh;
                }
            }
        }
    }
}
