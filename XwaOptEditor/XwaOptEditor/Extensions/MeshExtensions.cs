using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Extensions
{
    static class MeshExtensions
    {
        public static void RotateXY(this Mesh mesh, float value, float centerX, float centerY)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i] = mesh.Vertices[i].RotateXY(value, centerX, centerY);
            }

            for (int i = 0; i < mesh.VertexNormals.Count; i++)
            {
                mesh.VertexNormals[i] = mesh.VertexNormals[i].RotateXY(value, centerX, centerY);
            }

            foreach (var lod in mesh.Lods)
            {
                foreach (var faceGroup in lod.FaceGroups)
                {
                    foreach (var face in faceGroup.Faces)
                    {
                        face.Normal = face.Normal.RotateXY(value, centerX, centerY);
                    }
                }
            }

            if (mesh.RotationScale != null)
            {
                mesh.RotationScale.Pivot = mesh.RotationScale.Pivot.RotateXY(value, centerX, centerY);
                mesh.RotationScale.Look = mesh.RotationScale.Look.RotateXY(value, 0, 0);
                mesh.RotationScale.Up = mesh.RotationScale.Up.RotateXY(value, 0, 0);
                mesh.RotationScale.Right = mesh.RotationScale.Right.RotateXY(value, 0, 0);
            }

            foreach (var hardpoint in mesh.Hardpoints)
            {
                hardpoint.Position = hardpoint.Position.RotateXY(value, centerX, centerY);
            }

            foreach (var engineGlow in mesh.EngineGlows)
            {
                engineGlow.Position = engineGlow.Position.RotateXY(value, centerX, centerY);
                engineGlow.Look = engineGlow.Look.RotateXY(value, 0, 0);
                engineGlow.Up = engineGlow.Up.RotateXY(value, 0, 0);
                engineGlow.Right = engineGlow.Right.RotateXY(value, 0, 0);
            }

            mesh.ComputeHitzone();
        }

        public static void RotateXZ(this Mesh mesh, float value, float centerX, float centerZ)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i] = mesh.Vertices[i].RotateXZ(value, centerX, centerZ);
            }

            for (int i = 0; i < mesh.VertexNormals.Count; i++)
            {
                mesh.VertexNormals[i] = mesh.VertexNormals[i].RotateXZ(value, centerX, centerZ);
            }

            foreach (var lod in mesh.Lods)
            {
                foreach (var faceGroup in lod.FaceGroups)
                {
                    foreach (var face in faceGroup.Faces)
                    {
                        face.Normal = face.Normal.RotateXZ(value, centerX, centerZ);
                    }
                }
            }

            if (mesh.RotationScale != null)
            {
                mesh.RotationScale.Pivot = mesh.RotationScale.Pivot.RotateXZ(value, centerX, centerZ);
                mesh.RotationScale.Look = mesh.RotationScale.Look.RotateXZ(value, 0, 0);
                mesh.RotationScale.Up = mesh.RotationScale.Up.RotateXZ(value, 0, 0);
                mesh.RotationScale.Right = mesh.RotationScale.Right.RotateXZ(value, 0, 0);
            }

            foreach (var hardpoint in mesh.Hardpoints)
            {
                hardpoint.Position = hardpoint.Position.RotateXZ(value, centerX, centerZ);
            }

            foreach (var engineGlow in mesh.EngineGlows)
            {
                engineGlow.Position = engineGlow.Position.RotateXZ(value, centerX, centerZ);
                engineGlow.Look = engineGlow.Look.RotateXZ(value, 0, 0);
                engineGlow.Up = engineGlow.Up.RotateXZ(value, 0, 0);
                engineGlow.Right = engineGlow.Right.RotateXZ(value, 0, 0);
            }

            mesh.ComputeHitzone();
        }

        public static void RotateYZ(this Mesh mesh, float value, float centerY, float centerZ)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i] = mesh.Vertices[i].RotateYZ(value, centerY, centerZ);
            }

            for (int i = 0; i < mesh.VertexNormals.Count; i++)
            {
                mesh.VertexNormals[i] = mesh.VertexNormals[i].RotateYZ(value, centerY, centerZ);
            }

            foreach (var lod in mesh.Lods)
            {
                foreach (var faceGroup in lod.FaceGroups)
                {
                    foreach (var face in faceGroup.Faces)
                    {
                        face.Normal = face.Normal.RotateYZ(value, centerY, centerZ);
                    }
                }
            }

            if (mesh.RotationScale != null)
            {
                mesh.RotationScale.Pivot = mesh.RotationScale.Pivot.RotateYZ(value, centerY, centerZ);
                mesh.RotationScale.Look = mesh.RotationScale.Look.RotateYZ(value, 0, 0);
                mesh.RotationScale.Up = mesh.RotationScale.Up.RotateYZ(value, 0, 0);
                mesh.RotationScale.Right = mesh.RotationScale.Right.RotateYZ(value, 0, 0);
            }

            foreach (var hardpoint in mesh.Hardpoints)
            {
                hardpoint.Position = hardpoint.Position.RotateYZ(value, centerY, centerZ);
            }

            foreach (var engineGlow in mesh.EngineGlows)
            {
                engineGlow.Position = engineGlow.Position.RotateYZ(value, centerY, centerZ);
                engineGlow.Look = engineGlow.Look.RotateYZ(value, 0, 0);
                engineGlow.Up = engineGlow.Up.RotateYZ(value, 0, 0);
                engineGlow.Right = engineGlow.Right.RotateYZ(value, 0, 0);
            }

            mesh.ComputeHitzone();
        }
    }
}
