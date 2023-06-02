using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Extensions
{
    static class FaceGroupExtensions
    {
        public static void Quad2TriOptech(this FaceGroup faceGroup)
        {
            for (int faceIndex = 0; faceIndex < faceGroup.Faces.Count; faceIndex++)
            {
                var face = faceGroup.Faces[faceIndex];

                if (face.TrianglesCount != 2)
                {
                    continue;
                }

                faceGroup.Faces.Insert(faceIndex + 1, face.Clone());
                var newFace = faceGroup.Faces[faceIndex + 1];

                face.VerticesIndex = new Indices(face.VerticesIndex.A, face.VerticesIndex.B, face.VerticesIndex.D, -1);
                face.EdgesIndex = new Indices(face.EdgesIndex.A, face.EdgesIndex.B, face.EdgesIndex.D, -1);
                face.TextureCoordinatesIndex = new Indices(face.TextureCoordinatesIndex.A, face.TextureCoordinatesIndex.B, face.TextureCoordinatesIndex.D, -1);
                face.VertexNormalsIndex = new Indices(face.VertexNormalsIndex.A, face.VertexNormalsIndex.B, face.VertexNormalsIndex.D, -1);

                newFace.VerticesIndex = new Indices(newFace.VerticesIndex.B, newFace.VerticesIndex.C, newFace.VerticesIndex.D, -1);
                newFace.EdgesIndex = new Indices(newFace.EdgesIndex.B, newFace.EdgesIndex.C, newFace.EdgesIndex.D, -1);
                newFace.TextureCoordinatesIndex = new Indices(newFace.TextureCoordinatesIndex.B, newFace.TextureCoordinatesIndex.C, newFace.TextureCoordinatesIndex.D, -1);
                newFace.VertexNormalsIndex = new Indices(newFace.VertexNormalsIndex.B, newFace.VertexNormalsIndex.C, newFace.VertexNormalsIndex.D, -1);
            }
        }

        public static void Quad2Tri(this FaceGroup faceGroup)
        {
            for (int faceIndex = 0; faceIndex < faceGroup.Faces.Count; faceIndex++)
            {
                var face = faceGroup.Faces[faceIndex];

                if (face.TrianglesCount != 2)
                {
                    continue;
                }

                faceGroup.Faces.Insert(faceIndex + 1, face.Clone());
                var newFace = faceGroup.Faces[faceIndex + 1];

                face.VerticesIndex = new Indices(face.VerticesIndex.A, face.VerticesIndex.B, face.VerticesIndex.C, -1);
                face.EdgesIndex = new Indices(face.EdgesIndex.A, face.EdgesIndex.B, face.EdgesIndex.C, -1);
                face.TextureCoordinatesIndex = new Indices(face.TextureCoordinatesIndex.A, face.TextureCoordinatesIndex.B, face.TextureCoordinatesIndex.C, -1);
                face.VertexNormalsIndex = new Indices(face.VertexNormalsIndex.A, face.VertexNormalsIndex.B, face.VertexNormalsIndex.C, -1);

                newFace.VerticesIndex = new Indices(newFace.VerticesIndex.A, newFace.VerticesIndex.C, newFace.VerticesIndex.D, -1);
                newFace.EdgesIndex = new Indices(newFace.EdgesIndex.A, newFace.EdgesIndex.C, newFace.EdgesIndex.D, -1);
                newFace.TextureCoordinatesIndex = new Indices(newFace.TextureCoordinatesIndex.A, newFace.TextureCoordinatesIndex.C, newFace.TextureCoordinatesIndex.D, -1);
                newFace.VertexNormalsIndex = new Indices(newFace.VertexNormalsIndex.A, newFace.VertexNormalsIndex.C, newFace.VertexNormalsIndex.D, -1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AreVectorsNearEqual(Vector a, Vector b)
        {
            float e = 0.1f;

            if (b.X < a.X - e || b.X > a.X + e)
            {
                return false;
            }

            if (b.Y < a.Y - e || b.Y > a.Y + e)
            {
                return false;
            }

            if (b.Z < a.Z - e || b.Z > a.Z + e)
            {
                return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector VectorSubstract(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float VectorDotProduct(Vector v1, Vector v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector VectorCrossProduct(Vector v1, Vector v2)
        {
            return new Vector(
                (v1.Y * v2.Z) - (v1.Z * v2.Y),
                (v1.Z * v2.X) - (v1.X * v2.Z),
                (v1.X * v2.Y) - (v1.Y * v2.X));
        }

        private static float GetAngleBetweenVectors(Vector v1, Vector v2, Vector n)
        {
            float dot = VectorDotProduct(v1, v2);
            float length1 = (float)Math.Sqrt(VectorDotProduct(v1, v1));
            float length2 = (float)Math.Sqrt(VectorDotProduct(v2, v2));
            float length = length1 * length2;

            if (length > 0.0f)
            {
                dot /= length;
            }

            float det = VectorDotProduct(n, VectorCrossProduct(v1, v2));
            float angle = (float)Math.Atan2(det, dot);
            return angle;
        }

        private static readonly int[,] _sharedEdges = new int[9, 6]
        {
                {0, 1, 2, 1, 0, 2 },
                {0, 1, 2, 2, 1, 0 },
                {0, 1, 2, 0, 2, 1 },
                {1, 2, 0, 1, 0, 2 },
                {1, 2, 0, 2, 1, 0 },
                {1, 2, 0, 0, 2, 1 },
                {2, 0, 1, 1, 0, 2 },
                {2, 0, 1, 2, 1, 0 },
                {2, 0, 1, 0, 2, 1 },
        };

        private static int[] GetTriSharedEdge(Mesh mesh, Face face1, Face face2)
        {
            if (mesh == null || face1 == null || face2 == null || face1.TrianglesCount != 1 || face2.TrianglesCount != 1)
            {
                return null;
            }

            for (int edgeIndex = 0; edgeIndex < 9; edgeIndex++)
            {
                Vector a0 = mesh.Vertices.ElementAtOrDefault(face1.VerticesIndex.At(_sharedEdges[edgeIndex, 0]));
                Vector a1 = mesh.Vertices.ElementAtOrDefault(face1.VerticesIndex.At(_sharedEdges[edgeIndex, 1]));
                Vector b0 = mesh.Vertices.ElementAtOrDefault(face2.VerticesIndex.At(_sharedEdges[edgeIndex, 3]));
                Vector b1 = mesh.Vertices.ElementAtOrDefault(face2.VerticesIndex.At(_sharedEdges[edgeIndex, 4]));

                if (AreVectorsNearEqual(a0, b0) && AreVectorsNearEqual(a1, b1))
                {
                    return new int[6]
                    {
                        _sharedEdges[edgeIndex, 0],
                        _sharedEdges[edgeIndex, 1],
                        _sharedEdges[edgeIndex, 2],
                        _sharedEdges[edgeIndex, 3],
                        _sharedEdges[edgeIndex, 4],
                        _sharedEdges[edgeIndex, 5]
                    };
                }
            }

            return null;
        }

        private static bool IsTriFaceFlat(Mesh mesh, Face face)
        {
            Indices indices = face.VerticesIndex;
            Vector a0 = mesh.Vertices.ElementAtOrDefault(indices.A);
            Vector a1 = mesh.Vertices.ElementAtOrDefault(indices.B);
            Vector a2 = mesh.Vertices.ElementAtOrDefault(indices.C);

            if (a0 == a1 || a0 == a2 || a1 == a2)
            {
                return true;
            }

            return false;
        }

        public static void Tri2Quad(this FaceGroup faceGroup, Mesh mesh)
        {
            if (mesh == null)
            {
                return;
            }

            for (int faceIndex = 0; faceIndex < faceGroup.Faces.Count; faceIndex++)
            {
                var face = faceGroup.Faces[faceIndex];

                if (face.TrianglesCount != 1 || IsTriFaceFlat(mesh, face))
                {
                    continue;
                }

                for (int faceSubIndex = faceIndex + 1; faceSubIndex < faceGroup.Faces.Count; faceSubIndex++)
                {
                    var faceSub = faceGroup.Faces[faceSubIndex];

                    if (faceSub.TrianglesCount != 1 || IsTriFaceFlat(mesh, faceSub))
                    {
                        continue;
                    }

                    int[] edges = GetTriSharedEdge(mesh, face, faceSub);

                    if (edges == null)
                    {
                        continue;
                    }

                    Vector a0 = mesh.Vertices.ElementAtOrDefault(face.VerticesIndex.At(edges[0]));
                    Vector a1 = mesh.Vertices.ElementAtOrDefault(face.VerticesIndex.At(edges[1]));
                    Vector a2 = mesh.Vertices.ElementAtOrDefault(face.VerticesIndex.At(edges[2]));
                    Vector b0 = mesh.Vertices.ElementAtOrDefault(faceSub.VerticesIndex.At(edges[3]));
                    Vector b1 = mesh.Vertices.ElementAtOrDefault(faceSub.VerticesIndex.At(edges[4]));
                    Vector b2 = mesh.Vertices.ElementAtOrDefault(faceSub.VerticesIndex.At(edges[5]));

                    Vector normalA = Vector.Normal(a0, a1, a2);
                    Vector normalB = Vector.Normal(b1, b0, b2);

                    if (!AreVectorsNearEqual(normalA, normalB))
                    {
                        continue;
                    }

                    float angleA = GetAngleBetweenVectors(VectorSubstract(a2, a0), VectorSubstract(b2, a0), normalA);
                    float angleB = GetAngleBetweenVectors(VectorSubstract(b2, a1), VectorSubstract(a2, a1), normalA);

                    if (angleA <= 0.0f || angleB <= 0.0f)
                    {
                        continue;
                    }

                    face.Normal = normalA;

                    face.VerticesIndex = new Indices(
                        face.VerticesIndex.At(edges[0]),
                        faceSub.VerticesIndex.At(edges[5]),
                        face.VerticesIndex.At(edges[1]),
                        face.VerticesIndex.At(edges[2]));

                    face.EdgesIndex = new Indices(
                        face.EdgesIndex.At(edges[0]),
                        faceSub.EdgesIndex.At(edges[5]),
                        face.EdgesIndex.At(edges[1]),
                        face.EdgesIndex.At(edges[2]));

                    face.TextureCoordinatesIndex = new Indices(
                        face.TextureCoordinatesIndex.At(edges[0]),
                        faceSub.TextureCoordinatesIndex.At(edges[5]),
                        face.TextureCoordinatesIndex.At(edges[1]),
                        face.TextureCoordinatesIndex.At(edges[2]));

                    face.VertexNormalsIndex = new Indices(
                        face.VertexNormalsIndex.At(edges[0]),
                        faceSub.VertexNormalsIndex.At(edges[5]),
                        face.VertexNormalsIndex.At(edges[1]),
                        face.VertexNormalsIndex.At(edges[2]));

                    faceGroup.Faces.RemoveAt(faceSubIndex);

                    //{
                    //    Vector vA = mesh.Vertices.ElementAtOrDefault(face.VerticesIndex.A);
                    //    Vector vB = mesh.Vertices.ElementAtOrDefault(face.VerticesIndex.B);
                    //    Vector vC = mesh.Vertices.ElementAtOrDefault(face.VerticesIndex.C);
                    //    Vector vD = mesh.Vertices.ElementAtOrDefault(face.VerticesIndex.D);

                    //    float angleDAB = GetAngleBetweenVectors(VectorSubstract(vD, vA), VectorSubstract(vB, vA), normalA);
                    //    float angleABC = GetAngleBetweenVectors(VectorSubstract(vA, vB), VectorSubstract(vC, vB), normalA);
                    //    float angleBCD = GetAngleBetweenVectors(VectorSubstract(vB, vC), VectorSubstract(vD, vC), normalA);
                    //    float angleCDA = GetAngleBetweenVectors(VectorSubstract(vC, vD), VectorSubstract(vA, vD), normalA);
                    //}

                    break;
                }
            }
        }

        public static void GroupFaceGroups(this MeshLod lod)
        {
            var groups = new List<FaceGroup>(lod.FaceGroups.Count);

            foreach (var faceGroup in lod.FaceGroups)
            {
                FaceGroup index = null;

                foreach (var group in groups)
                {
                    if (group.Textures.Count != faceGroup.Textures.Count)
                    {
                        continue;
                    }

                    if (group.Textures.Count == 0)
                    {
                        index = group;
                        break;
                    }

                    int t = 0;
                    for (; t < group.Textures.Count; t++)
                    {
                        if (group.Textures[t] != faceGroup.Textures[t])
                        {
                            break;
                        }
                    }

                    if (t == group.Textures.Count)
                    {
                        index = group;
                        break;
                    }
                }

                if (index == null)
                {
                    groups.Add(faceGroup);
                }
                else
                {
                    foreach (var face in faceGroup.Faces)
                    {
                        index.Faces.Add(face);
                    }
                }
            }

            lod.FaceGroups.Clear();

            foreach (var group in groups)
            {
                lod.FaceGroups.Add(group);
            }
        }

        public static IList<FaceGroup> GetFaceGroups(this MeshLod lod, IList<FaceGroup> faceGroups)
        {
            var groups = new List<FaceGroup>(faceGroups.Count);

            foreach (var group in lod.FaceGroups)
            {
                foreach (var faceGroup in faceGroups)
                {
                    if (group.Textures.SequenceEqual(faceGroup.Textures))
                    {
                        groups.Add(group);
                        break;
                    }
                }
            }

            return groups;
        }
    }
}
