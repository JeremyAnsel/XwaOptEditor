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
                mesh.RotationScale.RotationAxis = mesh.RotationScale.RotationAxis.RotateXY(value, 0, 0);
                mesh.RotationScale.DirectionAxis = mesh.RotationScale.DirectionAxis.RotateXY(value, 0, 0);
                mesh.RotationScale.UpAxis = mesh.RotationScale.UpAxis.RotateXY(value, 0, 0);
            }

            foreach (var hardpoint in mesh.Hardpoints)
            {
                hardpoint.Position = hardpoint.Position.RotateXY(value, centerX, centerY);
            }

            foreach (var engineGlow in mesh.EngineGlows)
            {
                engineGlow.Position = engineGlow.Position.RotateXY(value, centerX, centerY);
                engineGlow.LookAxis = engineGlow.LookAxis.RotateXY(value, 0, 0);
                engineGlow.UpAxis = engineGlow.UpAxis.RotateXY(value, 0, 0);
                engineGlow.RightAxis = engineGlow.RightAxis.RotateXY(value, 0, 0);
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
                mesh.RotationScale.RotationAxis = mesh.RotationScale.RotationAxis.RotateXZ(value, 0, 0);
                mesh.RotationScale.DirectionAxis = mesh.RotationScale.DirectionAxis.RotateXZ(value, 0, 0);
                mesh.RotationScale.UpAxis = mesh.RotationScale.UpAxis.RotateXZ(value, 0, 0);
            }

            foreach (var hardpoint in mesh.Hardpoints)
            {
                hardpoint.Position = hardpoint.Position.RotateXZ(value, centerX, centerZ);
            }

            foreach (var engineGlow in mesh.EngineGlows)
            {
                engineGlow.Position = engineGlow.Position.RotateXZ(value, centerX, centerZ);
                engineGlow.LookAxis = engineGlow.LookAxis.RotateXZ(value, 0, 0);
                engineGlow.UpAxis = engineGlow.UpAxis.RotateXZ(value, 0, 0);
                engineGlow.RightAxis = engineGlow.RightAxis.RotateXZ(value, 0, 0);
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
                mesh.RotationScale.RotationAxis = mesh.RotationScale.RotationAxis.RotateYZ(value, 0, 0);
                mesh.RotationScale.DirectionAxis = mesh.RotationScale.DirectionAxis.RotateYZ(value, 0, 0);
                mesh.RotationScale.UpAxis = mesh.RotationScale.UpAxis.RotateYZ(value, 0, 0);
            }

            foreach (var hardpoint in mesh.Hardpoints)
            {
                hardpoint.Position = hardpoint.Position.RotateYZ(value, centerY, centerZ);
            }

            foreach (var engineGlow in mesh.EngineGlows)
            {
                engineGlow.Position = engineGlow.Position.RotateYZ(value, centerY, centerZ);
                engineGlow.LookAxis = engineGlow.LookAxis.RotateYZ(value, 0, 0);
                engineGlow.UpAxis = engineGlow.UpAxis.RotateYZ(value, 0, 0);
                engineGlow.RightAxis = engineGlow.RightAxis.RotateYZ(value, 0, 0);
            }

            mesh.ComputeHitzone();
        }
    }
}
