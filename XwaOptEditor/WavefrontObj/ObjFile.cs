using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WavefrontObj
{
    public class ObjFile
    {
        public ObjFile()
        {
            this.Meshes = new List<ObjMesh>();
            this.Vertices = new List<ObjVector3>();
            this.VertexTexCoords = new List<ObjVector2>();
            this.VertexNormals = new List<ObjVector3>();
            this.Materials = new ObjMaterialDictionary();
        }

        public string FileName { get; private set; }

        public IList<ObjMesh> Meshes { get; private set; }

        public IList<ObjVector3> Vertices { get; private set; }

        public IList<ObjVector2> VertexTexCoords { get; private set; }

        public IList<ObjVector3> VertexNormals { get; private set; }

        public ObjMaterialDictionary Materials { get; private set; }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        public static ObjFile FromFile(string fileName)
        {
            ObjFile obj = new ObjFile();

            obj.FileName = fileName;

            using (StreamReader file = new StreamReader(fileName))
            {
                string line;

                string materialName = null;

                ObjMesh mesh = new ObjMesh("unnamed");
                obj.Meshes.Add(mesh);

                ObjFaceGroup faceGroup = new ObjFaceGroup();
                mesh.FaceGroups.Add(faceGroup);

                while ((line = file.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    line = line.Trim();

                    if (line.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string[] values = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    switch (values[0].ToUpperInvariant())
                    {
                        case "MTLLIB":
                            if (values.Length != 2)
                            {
                                throw new InvalidDataException("missing mtllib name");
                            }

                            if (obj.Materials.Count > 0)
                            {
                                throw new InvalidDataException("multiple mtllib");
                            }

                            obj.Materials = ObjMaterialDictionary.FromFile(Path.Combine(Path.GetDirectoryName(fileName), values[1]));
                            break;

                        case "V":
                            if (values.Length < 4)
                            {
                                throw new InvalidDataException("v must be x y z");
                            }

                            obj.Vertices.Add(new ObjVector3(
                                float.Parse(values[1], CultureInfo.InvariantCulture),
                                float.Parse(values[2], CultureInfo.InvariantCulture),
                                float.Parse(values[3], CultureInfo.InvariantCulture)));
                            break;

                        case "VT":
                            if (values.Length < 3)
                            {
                                throw new InvalidDataException("vt must be u v");
                            }

                            obj.VertexTexCoords.Add(new ObjVector2(
                                float.Parse(values[1], CultureInfo.InvariantCulture),
                                float.Parse(values[2], CultureInfo.InvariantCulture)));
                            break;

                        case "VN":
                            if (values.Length < 4)
                            {
                                throw new InvalidDataException("vn must be x y z");
                            }

                            obj.VertexNormals.Add(new ObjVector3(
                                float.Parse(values[1], CultureInfo.InvariantCulture),
                                float.Parse(values[2], CultureInfo.InvariantCulture),
                                float.Parse(values[3], CultureInfo.InvariantCulture)));
                            break;

                        case "O":
                        case "G":
                            if (values.Length < 2)
                            {
                                throw new InvalidDataException("missing object name");
                            }

                            if (faceGroup.Faces.Count == 0)
                            {
                                mesh.Name = values[1];
                            }
                            else
                            {
                                mesh = new ObjMesh(values[1]);
                                obj.Meshes.Add(mesh);

                                faceGroup = new ObjFaceGroup();
                                faceGroup.MaterialName = materialName;
                                mesh.FaceGroups.Add(faceGroup);
                            }
                            break;

                        case "USEMTL":
                            if (values.Length != 2)
                            {
                                throw new InvalidDataException("missing material name");
                            }

                            materialName = values[1];

                            if (faceGroup.Faces.Count == 0)
                            {
                                faceGroup.MaterialName = materialName;
                            }
                            else
                            {
                                faceGroup = new ObjFaceGroup();
                                faceGroup.MaterialName = materialName;
                                mesh.FaceGroups.Add(faceGroup);
                            }
                            break;

                        case "F":
                            if (values.Length == 4)
                            {
                                var v1 = ParseFaceVertex(values[1]);
                                var v2 = ParseFaceVertex(values[2]);
                                var v3 = ParseFaceVertex(values[3]);

                                var face = new ObjFace();
                                face.VerticesIndex = new ObjIndex(v1.Item1, v2.Item1, v3.Item1);
                                face.VertexTexCoordsIndex = new ObjIndex(v1.Item2, v2.Item2, v3.Item2);
                                face.VertexNormalsIndex = new ObjIndex(v1.Item3, v2.Item3, v3.Item3);

                                faceGroup.Faces.Add(face);
                            }
                            else if (values.Length == 5)
                            {
                                var v1 = ParseFaceVertex(values[1]);
                                var v2 = ParseFaceVertex(values[2]);
                                var v3 = ParseFaceVertex(values[3]);
                                var v4 = ParseFaceVertex(values[4]);

                                var face = new ObjFace();
                                face.VerticesIndex = new ObjIndex(v1.Item1, v2.Item1, v3.Item1, v4.Item1);
                                face.VertexTexCoordsIndex = new ObjIndex(v1.Item2, v2.Item2, v3.Item2, v4.Item2);
                                face.VertexNormalsIndex = new ObjIndex(v1.Item3, v2.Item3, v3.Item3, v4.Item3);

                                faceGroup.Faces.Add(face);
                            }
                            else
                            {
                                throw new InvalidDataException("invalid face");
                            }
                            break;
                    }
                }
            }

            return obj;
        }

        private static Tuple<int, int, int> ParseFaceVertex(string vertex)
        {
            string[] values = vertex.Split('/');

            if (values.Length == 0)
            {
                throw new InvalidDataException("face vertex must be v/vt/vn in:\n" + vertex);
            }

            if (string.IsNullOrEmpty(values[0]))
            {
                throw new InvalidDataException("face vertex must be v/vt/vn: v must not be empty in:\n" + vertex);
            }

            if (values.Length == 1)
            {
                return new Tuple<int, int, int>(
                    int.Parse(values[0], CultureInfo.InvariantCulture) - 1,
                    -1,
                    -1);
            }

            if (values.Length == 2)
            {
                return new Tuple<int, int, int>(
                    int.Parse(values[0], CultureInfo.InvariantCulture) - 1,
                    string.IsNullOrEmpty(values[1]) ? -1 : int.Parse(values[1], CultureInfo.InvariantCulture) - 1,
                    -1);
            }

            return new Tuple<int, int, int>(
                int.Parse(values[0], CultureInfo.InvariantCulture) - 1,
                string.IsNullOrEmpty(values[1]) ? -1 : int.Parse(values[1], CultureInfo.InvariantCulture) - 1,
                string.IsNullOrEmpty(values[2]) ? -1 : int.Parse(values[2], CultureInfo.InvariantCulture) - 1);
        }

        public void Save(string fileName)
        {
            this.Save(fileName, null);
        }

        public void Save(string fileName, string mtlName)
        {
            this.AddTextureCoordinates();
            this.AddVertexNormals();
            this.CompactBuffers();

            if (mtlName == null)
            {
                this.Materials.Save(Path.ChangeExtension(fileName, "mtl"));
            }

            using (StreamWriter file = new StreamWriter(fileName))
            {
                file.WriteLine("mtllib {0}.mtl", mtlName ?? Path.GetFileNameWithoutExtension(fileName));

                foreach (var v in this.Vertices)
                {
                    file.WriteLine(string.Format(
                        CultureInfo.InvariantCulture,
                        "v {0:F6} {1:F6} {2:F6}",
                        v.X,
                        v.Y,
                        v.Z));
                }

                foreach (var v in this.VertexTexCoords)
                {
                    file.WriteLine(string.Format(
                        CultureInfo.InvariantCulture,
                        "vt {0:F6} {1:F6}",
                        v.U,
                        v.V));
                }

                foreach (var v in this.VertexNormals)
                {
                    file.WriteLine(string.Format(
                        CultureInfo.InvariantCulture,
                        "vn {0:F6} {1:F6} {2:F6}",
                        v.X,
                        v.Y,
                        v.Z));
                }

                foreach (var mesh in this.Meshes)
                {
                    file.WriteLine(string.Format(
                        CultureInfo.InvariantCulture,
                        "o {0}",
                        mesh.Name));

                    foreach (var faceGroup in mesh.FaceGroups)
                    {
                        if (!string.IsNullOrWhiteSpace(faceGroup.MaterialName))
                        {
                            file.WriteLine(string.Format(
                                CultureInfo.InvariantCulture,
                                "usemtl {0}",
                                faceGroup.MaterialName));
                        }

                        foreach (var face in faceGroup.Faces)
                        {
                            string v1 = FaceVertexToString(
                                face.VerticesIndex.A,
                                face.VertexTexCoordsIndex.A,
                                face.VertexNormalsIndex.A);

                            string v2 = FaceVertexToString(face.VerticesIndex.B,
                                face.VertexTexCoordsIndex.B,
                                face.VertexNormalsIndex.B);

                            string v3 = FaceVertexToString(face.VerticesIndex.C,
                                face.VertexTexCoordsIndex.C,
                                face.VertexNormalsIndex.C);

                            if (face.VerticesIndex.D < 0)
                            {
                                file.WriteLine("f {0} {1} {2}", v1, v2, v3);
                            }
                            else
                            {
                                string v4 = FaceVertexToString(face.VerticesIndex.D,
                                    face.VertexTexCoordsIndex.D,
                                    face.VertexNormalsIndex.D);

                                file.WriteLine("f {0} {1} {2} {3}", v1, v2, v3, v4);
                            }
                        }
                    }
                }
            }

            this.FileName = fileName;
        }

        private static string FaceVertexToString(int v, int t, int n)
        {
            if (v < 0)
            {
                throw new InvalidDataException("invalid vertex index");
            }

            return string.Format(CultureInfo.InvariantCulture,
                "{0}/{1}/{2}",
                v + 1,
                t < 0 ? string.Empty : (t + 1).ToString(CultureInfo.InvariantCulture),
                n < 0 ? string.Empty : (n + 1).ToString(CultureInfo.InvariantCulture));
        }

        public void CompactBuffers()
        {
            this.CompactVerticesBuffer();
            this.CompactTextureCoordinatesBuffer();
            this.CompactVertexNormalsBuffer();
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        private void CompactVerticesBuffer()
        {
            if (this.Vertices.Count == 0)
            {
                return;
            }

            bool[] isUsed = new bool[this.Vertices.Count];

            foreach (int i in this.Meshes
                .SelectMany(t => t.FaceGroups)
                .SelectMany(t => t.Faces)
                .Select(t => t.VerticesIndex)
                .SelectMany(t => new int[] { t.A, t.B, t.C, t.D }))
            {
                if (i >= 0 && i < this.Vertices.Count)
                {
                    isUsed[i] = true;
                }
            }

            List<ObjVector3> newValues = new List<ObjVector3>(this.Vertices.Count);
            int[] newIndices = new int[this.Vertices.Count];

            for (int i = 0; i < this.Vertices.Count; i++)
            {
                if (!isUsed[i])
                {
                    continue;
                }

                ObjVector3 value = this.Vertices[i];
                int index = -1;

                for (int j = 0; j < newValues.Count; j++)
                {
                    if (newValues[j] == value)
                    {
                        index = j;
                        break;
                    }
                }

                if (index == -1)
                {
                    newIndices[i] = newValues.Count;
                    newValues.Add(value);
                }
                else
                {
                    newIndices[i] = index;
                }
            }

            newValues.TrimExcess();

            this.Vertices = newValues;

            foreach (var face in this.Meshes
                .SelectMany(t => t.FaceGroups)
                .SelectMany(t => t.Faces))
            {
                ObjIndex index = face.VerticesIndex;

                face.VerticesIndex = new ObjIndex(
                    index.A < 0 ? -1 : newIndices[index.A],
                    index.B < 0 ? -1 : newIndices[index.B],
                    index.C < 0 ? -1 : newIndices[index.C],
                    index.D < 0 ? -1 : newIndices[index.D]);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        private void CompactTextureCoordinatesBuffer()
        {
            if (this.VertexTexCoords.Count == 0)
            {
                return;
            }

            bool[] isUsed = new bool[this.VertexTexCoords.Count];

            foreach (int i in this.Meshes
                .SelectMany(t => t.FaceGroups)
                .SelectMany(t => t.Faces)
                .Select(t => t.VertexTexCoordsIndex)
                .SelectMany(t => new int[] { t.A, t.B, t.C, t.D }))
            {
                if (i >= 0 && i < this.VertexTexCoords.Count)
                {
                    isUsed[i] = true;
                }
            }

            List<ObjVector2> newValues = new List<ObjVector2>(this.VertexTexCoords.Count);
            int[] newIndices = new int[this.VertexTexCoords.Count];

            for (int i = 0; i < this.VertexTexCoords.Count; i++)
            {
                if (!isUsed[i])
                {
                    continue;
                }

                ObjVector2 value = this.VertexTexCoords[i];
                int index = -1;

                for (int j = 0; j < newValues.Count; j++)
                {
                    if (newValues[j] == value)
                    {
                        index = j;
                        break;
                    }
                }

                if (index == -1)
                {
                    newIndices[i] = newValues.Count;
                    newValues.Add(value);
                }
                else
                {
                    newIndices[i] = index;
                }
            }

            newValues.TrimExcess();

            this.VertexTexCoords = newValues;

            foreach (var face in this.Meshes
                .SelectMany(t => t.FaceGroups)
                .SelectMany(t => t.Faces))
            {
                ObjIndex index = face.VertexTexCoordsIndex;

                face.VertexTexCoordsIndex = new ObjIndex(
                    index.A < 0 ? -1 : newIndices[index.A],
                    index.B < 0 ? -1 : newIndices[index.B],
                    index.C < 0 ? -1 : newIndices[index.C],
                    index.D < 0 ? -1 : newIndices[index.D]);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        private void CompactVertexNormalsBuffer()
        {
            if (this.VertexNormals.Count == 0)
            {
                return;
            }

            bool[] isUsed = new bool[this.VertexNormals.Count];

            foreach (int i in this.Meshes
                .SelectMany(t => t.FaceGroups)
                .SelectMany(t => t.Faces)
                .Select(t => t.VertexNormalsIndex)
                .SelectMany(t => new int[] { t.A, t.B, t.C, t.D }))
            {
                if (i >= 0 && i < this.VertexNormals.Count)
                {
                    isUsed[i] = true;
                }
            }

            List<ObjVector3> newValues = new List<ObjVector3>(this.VertexNormals.Count);
            int[] newIndices = new int[this.VertexNormals.Count];

            for (int i = 0; i < this.VertexNormals.Count; i++)
            {
                if (!isUsed[i])
                {
                    continue;
                }

                ObjVector3 value = this.VertexNormals[i];
                int index = -1;

                for (int j = 0; j < newValues.Count; j++)
                {
                    if (newValues[j] == value)
                    {
                        index = j;
                        break;
                    }
                }

                if (index == -1)
                {
                    newIndices[i] = newValues.Count;
                    newValues.Add(value);
                }
                else
                {
                    newIndices[i] = index;
                }
            }

            newValues.TrimExcess();

            this.VertexNormals = newValues;

            foreach (var face in this.Meshes
                .SelectMany(t => t.FaceGroups)
                .SelectMany(t => t.Faces))
            {
                ObjIndex index = face.VertexNormalsIndex;

                face.VertexNormalsIndex = new ObjIndex(
                    index.A < 0 ? -1 : newIndices[index.A],
                    index.B < 0 ? -1 : newIndices[index.B],
                    index.C < 0 ? -1 : newIndices[index.C],
                    index.D < 0 ? -1 : newIndices[index.D]);
            }
        }

        public void CompactFaceGroups()
        {
            this.Meshes
                .AsParallel()
                .ForAll(t => t.CompactFaceGroups());
        }

        public void SplitFaceGroups()
        {
            this.Meshes
                .AsParallel()
                .ForAll(t => t.SplitFaceGroups());
        }

        public void AddTextureCoordinates()
        {
            bool texCoordsAdded = false;

            foreach (var face in this.Meshes.SelectMany(t => t.FaceGroups).SelectMany(t => t.Faces))
            {
                ObjIndex verticesIndex = face.VerticesIndex;
                ObjIndex texCoord = face.VertexTexCoordsIndex;

                if (texCoord.A < 0 || texCoord.B < 0 || texCoord.C < 0 || (verticesIndex.D >= 0 && texCoord.D < 0))
                {
                    if (!texCoordsAdded)
                    {
                        this.VertexTexCoords.Add(new ObjVector2(0, 0));
                        this.VertexTexCoords.Add(new ObjVector2(1, 0));
                        this.VertexTexCoords.Add(new ObjVector2(1, 1));
                        this.VertexTexCoords.Add(new ObjVector2(0, 1));
                        texCoordsAdded = true;
                    }

                    texCoord.A = this.VertexTexCoords.Count - 4;
                    texCoord.B = this.VertexTexCoords.Count - 3;
                    texCoord.C = this.VertexTexCoords.Count - 2;
                    texCoord.D = verticesIndex.D < 0 ? -1 : this.VertexTexCoords.Count - 1;

                    face.VertexTexCoordsIndex = texCoord;
                }
            }
        }

        public void AddVertexNormals()
        {
            foreach (var face in this.Meshes.SelectMany(t => t.FaceGroups).SelectMany(t => t.Faces))
            {
                ObjIndex verticesIndex = face.VerticesIndex;
                ObjIndex vertexNormalsIndex = face.VertexNormalsIndex;

                if (vertexNormalsIndex.A < 0 || vertexNormalsIndex.B < 0 || vertexNormalsIndex.C < 0 || (verticesIndex.D >= 0 && vertexNormalsIndex.D < 0))
                {
                    ObjVector3 normal = ObjVector3.Normal(
                        this.Vertices.ElementAtOrDefault(verticesIndex.A),
                        this.Vertices.ElementAtOrDefault(verticesIndex.B),
                        this.Vertices.ElementAtOrDefault(verticesIndex.C));

                    this.VertexNormals.Add(normal);

                    vertexNormalsIndex.A = this.VertexNormals.Count - 1;
                    vertexNormalsIndex.B = this.VertexNormals.Count - 1;
                    vertexNormalsIndex.C = this.VertexNormals.Count - 1;
                    vertexNormalsIndex.D = verticesIndex.D < 0 ? -1 : this.VertexNormals.Count - 1;

                    face.VertexNormalsIndex = vertexNormalsIndex;
                }
            }
        }
    }
}
