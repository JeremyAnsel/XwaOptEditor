using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Extensions
{
    static class VectorExtensions
    {
        public static Vector RotateXY(this Vector vertex, float value, float centerX, float centerY)
        {
            double angle = (Math.PI / 180) * value;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            float x = vertex.X - centerX;
            float y = vertex.Y - centerY;
            float xnew = x * cos - y * sin;
            float ynew = x * sin + y * cos;
            x = xnew + centerX;
            y = ynew + centerY;

            return new Vector(x, y, vertex.Z);
        }

        public static Vector RotateXZ(this Vector vertex, float value, float centerX, float centerZ)
        {
            double angle = (Math.PI / 180) * value;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            float x = vertex.X - centerX;
            float z = vertex.Z - centerZ;
            float xnew = x * cos - z * sin;
            float znew = x * sin + z * cos;
            x = xnew + centerX;
            z = znew + centerZ;

            return new Vector(x, vertex.Y, z);
        }

        public static Vector RotateYZ(this Vector vertex, float value, float centerY, float centerZ)
        {
            double angle = (Math.PI / 180) * value;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            float y = vertex.Y - centerY;
            float z = vertex.Z - centerZ;
            float ynew = y * cos - z * sin;
            float znew = y * sin + z * cos;
            y = ynew + centerY;
            z = znew + centerZ;

            return new Vector(vertex.X, y, z);
        }
    }
}
