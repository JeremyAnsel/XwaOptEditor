using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using JeremyAnsel.Xwa.Opt;
using Obj;

namespace OptObjConverter
{
    public static class Converter
    {
        public static void OptToObj(OptFile opt, string objPath)
        {
            Converter.OptToObj(opt, objPath, true);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public static void OptToObj(OptFile opt, string objPath, bool scale)
        {
            if (opt == null)
            {
                throw new ArgumentNullException("opt");
            }

            string objDirectory = Path.GetDirectoryName(objPath);
            string objName = Path.GetFileNameWithoutExtension(objPath);

            var objMaterials = new ObjMaterialDictionary();

            foreach (var texture in opt.Textures.Values)
            {
                var material = new ObjMaterial
                {
                    Name = texture.Name,
                    DiffuseMapFileName = string.Format(CultureInfo.InvariantCulture, "{0}.png", texture.Name)
                };

                texture.Save(Path.Combine(objDirectory, material.DiffuseMapFileName));

                if (texture.HasAlpha)
                {
                    material.AlphaMapFileName = string.Format(CultureInfo.InvariantCulture, "{0}_alpha.png", texture.Name);

                    texture.SaveAlphaMap(Path.Combine(objDirectory, material.AlphaMapFileName));
                }

                objMaterials.Add(texture.Name, material);
            }

            objMaterials.Save(Path.ChangeExtension(objPath, "mtl"));

            var distances = opt.Meshes
                .SelectMany(t => t.Lods)
                .Select(t => t.Distance)
                .Distinct()
                .OrderByDescending(t => t)
                .ToArray();

            for (int distance = 0; distance < distances.Length; distance++)
            {
                var obj = new ObjFile();

                int objectsIndex = 0;
                int verticesIndex = 0;
                int verticesTexIndex = 0;
                int verticesNormalIndex = 0;

                foreach (var mesh in opt.Meshes)
                {
                    var lod = mesh.Lods.FirstOrDefault(t => t.Distance <= distances[distance]);

                    if (lod == null)
                    {
                        continue;
                    }

                    var objMesh = new ObjMesh(string.Format(CultureInfo.InvariantCulture, "{0}.{1:D3}", mesh.Descriptor.MeshType, objectsIndex));
                    obj.Meshes.Add(objMesh);
                    objectsIndex++;

                    if (scale)
                    {
                        foreach (var v in mesh.Vertices)
                        {
                            obj.Vertices.Add(new ObjVector3(v.X * OptFile.ScaleFactor, v.Z * OptFile.ScaleFactor, v.Y * OptFile.ScaleFactor));
                        }
                    }
                    else
                    {
                        foreach (var v in mesh.Vertices)
                        {
                            obj.Vertices.Add(new ObjVector3(v.X, v.Z, v.Y));
                        }
                    }

                    foreach (var v in mesh.TextureCoordinates)
                    {
                        obj.VertexTexCoords.Add(new ObjVector2(v.U, -v.V));
                    }

                    foreach (var v in mesh.VertexNormals)
                    {
                        obj.VertexNormals.Add(new ObjVector3(v.X, v.Z, v.Y));
                    }

                    foreach (var faceGroup in lod.FaceGroups)
                    {
                        var objFaceGroup = new ObjFaceGroup();

                        if (faceGroup.Textures.Count > 0)
                        {
                            objFaceGroup.MaterialName = faceGroup.Textures[0];
                        }

                        objMesh.FaceGroups.Add(objFaceGroup);

                        foreach (var face in faceGroup.Faces)
                        {
                            if (face.VerticesIndex.D < 0)
                            {
                                objFaceGroup.Faces.Add(new ObjFace()
                                {
                                    VerticesIndex = new ObjIndex(
                                        verticesIndex + face.VerticesIndex.A,
                                        verticesIndex + face.VerticesIndex.B,
                                        verticesIndex + face.VerticesIndex.C
                                        ),

                                    VertexTexCoordsIndex = new ObjIndex(
                                        verticesTexIndex + face.TextureCoordinatesIndex.A,
                                        verticesTexIndex + face.TextureCoordinatesIndex.B,
                                        verticesTexIndex + face.TextureCoordinatesIndex.C),

                                    VertexNormalsIndex = new ObjIndex(
                                        verticesNormalIndex + face.VertexNormalsIndex.A,
                                        verticesNormalIndex + face.VertexNormalsIndex.B,
                                        verticesNormalIndex + face.VertexNormalsIndex.C)
                                });
                            }
                            else
                            {
                                objFaceGroup.Faces.Add(new ObjFace()
                                {
                                    VerticesIndex = new ObjIndex(
                                        verticesIndex + face.VerticesIndex.A,
                                        verticesIndex + face.VerticesIndex.B,
                                        verticesIndex + face.VerticesIndex.C,
                                        verticesIndex + face.VerticesIndex.D
                                        ),

                                    VertexTexCoordsIndex = new ObjIndex(
                                        verticesTexIndex + face.TextureCoordinatesIndex.A,
                                        verticesTexIndex + face.TextureCoordinatesIndex.B,
                                        verticesTexIndex + face.TextureCoordinatesIndex.C,
                                        verticesTexIndex + face.TextureCoordinatesIndex.D),

                                    VertexNormalsIndex = new ObjIndex(
                                        verticesNormalIndex + face.VertexNormalsIndex.A,
                                        verticesNormalIndex + face.VertexNormalsIndex.B,
                                        verticesNormalIndex + face.VertexNormalsIndex.C,
                                        verticesNormalIndex + face.VertexNormalsIndex.D)
                                });
                            }
                        }
                    }

                    verticesIndex += mesh.Vertices.Count;
                    verticesTexIndex += mesh.TextureCoordinates.Count;
                    verticesNormalIndex += mesh.VertexNormals.Count;
                }

                obj.Save(Path.Combine(objDirectory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}.obj", objName, distance)), objName);
            }
        }

        public static OptFile ObjToOpt(string objPath)
        {
            return Converter.ObjToOpt(objPath, true);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        public static OptFile ObjToOpt(string objPath, bool scale)
        {
            string objDirectory = Path.GetDirectoryName(objPath);

            var obj = ObjFile.FromFile(objPath);
            var opt = new OptFile();

            foreach (var mesh in obj.Meshes)
            {
                var optMesh = new Mesh();
                opt.Meshes.Add(optMesh);

                if (scale)
                {
                    foreach (var v in obj.Vertices)
                    {
                        optMesh.Vertices.Add(new Vector(v.X / OptFile.ScaleFactor, v.Z / OptFile.ScaleFactor, v.Y / OptFile.ScaleFactor));
                    }
                }
                else
                {
                    foreach (var v in obj.Vertices)
                    {
                        optMesh.Vertices.Add(new Vector(v.X, v.Z, v.Y));
                    }
                }

                foreach (var v in obj.VertexTexCoords)
                {
                    optMesh.TextureCoordinates.Add(new TextureCoordinates(v.U, -v.V));
                }

                foreach (var v in obj.VertexNormals)
                {
                    optMesh.VertexNormals.Add(new Vector(v.X, v.Z, v.Y));
                }

                optMesh.TextureCoordinates.Add(new TextureCoordinates(0, 0));
                optMesh.TextureCoordinates.Add(new TextureCoordinates(1, 0));
                optMesh.TextureCoordinates.Add(new TextureCoordinates(1, 1));
                optMesh.TextureCoordinates.Add(new TextureCoordinates(0, 1));

                var optLod = new MeshLod();
                optMesh.Lods.Add(optLod);

                foreach (var faceGroup in mesh.FaceGroups)
                {
                    var optFaceGroup = new FaceGroup();
                    optLod.FaceGroups.Add(optFaceGroup);

                    if (!string.IsNullOrEmpty(faceGroup.MaterialName))
                    {
                        optFaceGroup.Textures.Add(faceGroup.MaterialName);
                    }

                    foreach (var face in faceGroup.Faces)
                    {
                        Index verticesIndex = new Index(
                                face.VerticesIndex.A,
                                face.VerticesIndex.B,
                                face.VerticesIndex.C,
                                face.VerticesIndex.D);

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

                        Index textureCoordinatesIndex = new Index(
                                face.VertexTexCoordsIndex.A,
                                face.VertexTexCoordsIndex.B,
                                face.VertexTexCoordsIndex.C,
                                face.VertexTexCoordsIndex.D);

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

                        Index vertexNormalsIndex = new Index(
                                face.VertexNormalsIndex.A,
                                face.VertexNormalsIndex.B,
                                face.VertexNormalsIndex.C,
                                face.VertexNormalsIndex.D);

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
            }

            opt.CompactBuffers();
            opt.ComputeHitzones();

            foreach (var material in obj.Materials.Values)
            {
                Texture texture;

                if (material.DiffuseMapFileName == null)
                {
                    var color = material.DiffuseColor;
                    byte r = (byte)(color.X * 255.0f);
                    byte g = (byte)(color.Y * 255.0f);
                    byte b = (byte)(color.Z * 255.0f);

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
                }
                else
                {
                    string colorFileName = Path.Combine(objDirectory, material.DiffuseMapFileName);

                    texture = Texture.FromFile(colorFileName);
                    texture.Name = material.Name;
                }

                if (material.AlphaMapFileName != null)
                {
                    string alphaFileName = Path.Combine(objDirectory, material.AlphaMapFileName);

                    texture.SetAlphaMap(alphaFileName);
                }
                else if (material.DissolveFactor > 0.0f && material.DissolveFactor < 1.0f)
                {
                    byte alpha = (byte)(material.DissolveFactor * 255.0f);

                    int length = texture.Width * texture.Height;

                    byte[] alphaData = new byte[length];
                    var data = texture.ImageData;

                    for (int i = 0; i < length; i++)
                    {
                        alphaData[i] = alpha;
                        data[i * 4 + 3] = alpha;
                    }

                    texture.AlphaData = alphaData;
                }

                opt.Textures.Add(texture.Name, texture);
            }

            opt.GenerateTexturesMipmaps();

            return opt;
        }
    }
}
