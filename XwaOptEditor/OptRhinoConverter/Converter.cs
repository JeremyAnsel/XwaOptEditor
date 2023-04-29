using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using JeremyAnsel.Xwa.Opt;

namespace OptRhinoConverter
{
    public static class Converter
    {
        public static void OptToRhino(OptFile opt, string rhinoPath)
        {
            Converter.OptToRhino(opt, rhinoPath, true, null);
        }

        public static void OptToRhino(OptFile opt, string rhinoPath, Action<string> notify)
        {
            Converter.OptToRhino(opt, rhinoPath, true, notify);
        }

        public static void OptToRhino(OptFile opt, string rhinoPath, bool scale)
        {
            Converter.OptToRhino(opt, rhinoPath, scale, null);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
        public static void OptToRhino(OptFile opt, string rhinoPath, bool scale, Action<string> notify)
        {
            if (opt == null)
            {
                throw new ArgumentNullException("opt");
            }

            string rhinoDirectory = Path.GetDirectoryName(rhinoPath);
            string rhinoName = Path.GetFileNameWithoutExtension(rhinoPath);

            if (notify != null)
            {
                notify(string.Format(CultureInfo.InvariantCulture, "Exporting {0}.3dm...", rhinoName));
            }

            foreach (var texture in opt.Textures.Values)
            {
                texture.Save(Path.Combine(rhinoDirectory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}.png", rhinoName, texture.Name)));

                if (texture.HasAlpha)
                {
                    texture.SaveAlphaMap(Path.Combine(rhinoDirectory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}_alpha.png", rhinoName, texture.Name)));
                }

                if (texture.IsIlluminated)
                {
                    texture.SaveIllumMap(Path.Combine(rhinoDirectory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}_illum.png", rhinoName, texture.Name)));
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
                    notify(string.Format(CultureInfo.InvariantCulture, "Exporting {0}_{1}_{2}.3dm...", rhinoName, distance, version));
                }

                using (var file = new Rhino.FileIO.File3dm())
                {
                    file.Settings.ModelUnitSystem = Rhino.UnitSystem.Meters;

                    int objectsIndex = 0;
                    List<string> textureNames = new List<string>();

                    foreach (var mesh in opt.Meshes)
                    {
                        var lod = mesh.Lods.FirstOrDefault(t => t.Distance <= distances[distance]);

                        if (lod == null)
                        {
                            continue;
                        }

                        foreach (var texture in lod.FaceGroups
                            .SelectMany(t => t.Textures)
                            .Distinct())
                        {
                            textureNames.Add(texture);
                        }

                        string meshName = string.Format(CultureInfo.InvariantCulture, "{0:D3}.{1}", objectsIndex, mesh.Descriptor.MeshType);

                        using (var layer = new Rhino.DocObjects.Layer())
                        {
                            layer.Name = meshName;

                            file.AllLayers.Add(layer);
                        }

                        foreach (var faceGroup in lod.FaceGroups)
                        {
                            using (var rhinoMesh = new Rhino.Geometry.Mesh())
                            using (var rhinoAttributes = new Rhino.DocObjects.ObjectAttributes())
                            {
                                rhinoAttributes.Name = meshName;
                                rhinoAttributes.LayerIndex = objectsIndex;

                                if (faceGroup.Textures.Count > 0)
                                {
                                    int currentVersion = version;

                                    if (version < 0 || version >= faceGroup.Textures.Count)
                                    {
                                        currentVersion = faceGroup.Textures.Count - 1;
                                    }

                                    rhinoAttributes.MaterialIndex = textureNames.IndexOf(faceGroup.Textures[currentVersion]);
                                }

                                Action<Vector> addVertex;

                                if (scale)
                                {
                                    addVertex = vertex => rhinoMesh.Vertices.Add(vertex.X * OptFile.ScaleFactor, vertex.Y * OptFile.ScaleFactor, vertex.Z * OptFile.ScaleFactor);
                                }
                                else
                                {
                                    addVertex = vertex => rhinoMesh.Vertices.Add(vertex.X, vertex.Y, vertex.Z);
                                }

                                Action<TextureCoordinates> addTexCoords = texCoords => rhinoMesh.TextureCoordinates.Add(texCoords.U, texCoords.V);

                                Action<Vector> addNormal = normal => rhinoMesh.Normals.Add(normal.X, normal.Y, normal.Z);

                                int facesIndex = 0;

                                foreach (var face in faceGroup.Faces)
                                {
                                    var verticesIndex = face.VerticesIndex;
                                    var texCoordsIndex = face.TextureCoordinatesIndex;
                                    var normalsIndex = face.VertexNormalsIndex;

                                    addVertex(mesh.Vertices[verticesIndex.A]);
                                    addTexCoords(mesh.TextureCoordinates[texCoordsIndex.A]);
                                    addNormal(mesh.VertexNormals[normalsIndex.A]);
                                    facesIndex++;

                                    addVertex(mesh.Vertices[verticesIndex.B]);
                                    addTexCoords(mesh.TextureCoordinates[texCoordsIndex.B]);
                                    addNormal(mesh.VertexNormals[normalsIndex.B]);
                                    facesIndex++;

                                    addVertex(mesh.Vertices[verticesIndex.C]);
                                    addTexCoords(mesh.TextureCoordinates[texCoordsIndex.C]);
                                    addNormal(mesh.VertexNormals[normalsIndex.C]);
                                    facesIndex++;

                                    if (verticesIndex.D >= 0)
                                    {
                                        addVertex(mesh.Vertices[verticesIndex.D]);
                                        addTexCoords(mesh.TextureCoordinates[texCoordsIndex.D]);
                                        addNormal(mesh.VertexNormals[normalsIndex.D]);
                                        facesIndex++;
                                    }

                                    if (verticesIndex.D < 0)
                                    {
                                        rhinoMesh.Faces.AddFace(facesIndex - 1, facesIndex - 2, facesIndex - 3);
                                    }
                                    else
                                    {
                                        rhinoMesh.Faces.AddFace(facesIndex - 1, facesIndex - 2, facesIndex - 3, facesIndex - 4);
                                    }
                                }

                                rhinoMesh.Compact();

                                file.Objects.AddMesh(rhinoMesh, rhinoAttributes);
                            }
                        }

                        objectsIndex++;
                    }

                    foreach (var textureName in textureNames)
                    {
                        Texture texture;
                        opt.Textures.TryGetValue(textureName, out texture);

                        using (var material = new Rhino.DocObjects.Material())
                        {
                            material.Name = rhinoName + "_" + textureName;

                            if (texture == null)
                            {
                                material.DiffuseColor = System.Drawing.Color.White;
                            }
                            else
                            {
                                material.SetBitmapTexture(rhinoName + "_" + textureName + ".png");

                                if (texture.HasAlpha)
                                {
                                    material.SetTransparencyTexture(rhinoName + "_" + textureName + "_alpha.png");
                                }

                                if (texture.IsIlluminated)
                                {
                                }
                            }

                            file.AllMaterials.Add(material);
                        }
                    }

                    file.Write(Path.Combine(rhinoDirectory, string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}.3dm", rhinoName, distance, version)), 4);
                }
            }
        }

        public static OptFile RhinoToOpt(string rhinoPath)
        {
            return Converter.RhinoToOpt(rhinoPath, true);
        }

        [SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
        public static OptFile RhinoToOpt(string rhinoPath, bool scale)
        {
            string rhinoDirectory = Path.GetDirectoryName(rhinoPath);

            var opt = new OptFile();

            using (var file = Rhino.FileIO.File3dm.Read(rhinoPath))
            {
                float scaleFactor = scale ? (1.0f / OptFile.ScaleFactor) : 1.0f;

                if (file.Settings.ModelUnitSystem != Rhino.UnitSystem.Meters)
                {
                    scaleFactor *= (float)Rhino.RhinoMath.UnitScale(file.Settings.ModelUnitSystem, Rhino.UnitSystem.Meters);
                    scale = true;
                }

                var groups = file.Objects
                    .Where(t =>
                    {
                        using (var geometry = t.Geometry)
                        {
                            return geometry.ObjectType == Rhino.DocObjects.ObjectType.Mesh;
                        }
                    })
                    .GroupBy(t =>
                    {
                        using (var attributes = t.Attributes)
                        {
                            return attributes.LayerIndex;
                        }
                    })
                    .ToList();

                foreach (var group in groups)
                {
                    var mesh = new Mesh();
                    opt.Meshes.Add(mesh);

                    var lod = new MeshLod();
                    mesh.Lods.Add(lod);

                    foreach (var obj in group)
                    {
                        var faceGroup = new FaceGroup();
                        lod.FaceGroups.Add(faceGroup);

                        int baseIndex = mesh.Vertices.Count;

                        using (var geometry = (Rhino.Geometry.Mesh)obj.Geometry)
                        {
                            if (scale)
                            {
                                foreach (var vertex in geometry.Vertices)
                                {
                                    mesh.Vertices.Add(new Vector(vertex.X * scaleFactor, vertex.Y * scaleFactor, vertex.Z * scaleFactor));
                                }
                            }
                            else
                            {
                                foreach (var vertex in geometry.Vertices)
                                {
                                    mesh.Vertices.Add(new Vector(vertex.X, vertex.Y, vertex.Z));
                                }
                            }

                            foreach (var texCoords in geometry.TextureCoordinates)
                            {
                                mesh.TextureCoordinates.Add(new TextureCoordinates(texCoords.X, texCoords.Y));
                            }

                            foreach (var normal in geometry.Normals)
                            {
                                mesh.VertexNormals.Add(new Vector(normal.X, normal.Y, normal.Z));
                            }

                            foreach (var geoFace in geometry.Faces)
                            {
                                var face = new Face();
                                faceGroup.Faces.Add(face);

                                Indices index = geoFace.IsTriangle ?
                                    new Indices(baseIndex + geoFace.C, baseIndex + geoFace.B, baseIndex + geoFace.A) :
                                    new Indices(baseIndex + geoFace.D, baseIndex + geoFace.C, baseIndex + geoFace.B, baseIndex + geoFace.A);

                                face.VerticesIndex = index;
                                face.VertexNormalsIndex = index;
                                face.TextureCoordinatesIndex = index;

                                face.Normal = Vector.Normal(mesh.Vertices[index.A], mesh.Vertices[index.B], mesh.Vertices[index.C]);
                            }
                        }

                        using (var attributes = obj.Attributes)
                        {
                            if (attributes.MaterialIndex != -1)
                            {
                                using (var material = file.AllMaterials.FindIndex(attributes.MaterialIndex))
                                {
                                    faceGroup.Textures.Add(material.Name);
                                }
                            }
                        }
                    }
                }

                opt.CompactBuffers();
                opt.ComputeHitzones();

                for (int materialIndex = 0; materialIndex < file.AllMaterials.Count; materialIndex++)
                {
                    using (var material = file.AllMaterials.FindIndex(materialIndex))
                    {
                        if (opt.Textures.ContainsKey(material.Name))
                        {
                            continue;
                        }

                        string colorMap = null;
                        string alphaMap = null;

                        using (var tex = material.GetBitmapTexture())
                        {
                            if (tex != null && !string.IsNullOrEmpty(tex.FileName))
                            {
                                colorMap = Path.GetFileName(tex.FileName);
                            }
                        }

                        using (var tex = material.GetTransparencyTexture())
                        {
                            if (tex != null && !string.IsNullOrEmpty(tex.FileName))
                            {
                                alphaMap = Path.GetFileName(tex.FileName);
                            }
                        }

                        Texture texture;
                        int bpp;

                        if (colorMap == null)
                        {
                            var color = material.DiffuseColor;
                            byte r = color.R;
                            byte g = color.G;
                            byte b = color.B;

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
                            string colorFileName = Path.Combine(rhinoDirectory, colorMap);

                            texture = Texture.FromFile(colorFileName);
                            texture.Name = material.Name;

                            bpp = texture.BitsPerPixel;
                        }

                        if (alphaMap != null)
                        {
                            string alphaFileName = Path.Combine(rhinoDirectory, alphaMap);

                            texture.SetAlphaMap(alphaFileName);
                        }
                        else if (material.Transparency > 0.0 && material.Transparency < 1.0)
                        {
                            texture.Convert8To32();

                            byte alpha = (byte)(material.Transparency * 255.0f);

                            int length = texture.Width * texture.Height;

                            var data = texture.ImageData;

                            for (int i = 0; i < length; i++)
                            {
                                data[i * 4 + 3] = alpha;
                            }

                            texture.Palette[2] = 0xff;
                        }

                        if (bpp == 32)
                        {
                            texture.GenerateMipmaps();
                        }

                        opt.Textures.Add(texture.Name, texture);
                    }
                }
            }

            return opt;
        }
    }
}
