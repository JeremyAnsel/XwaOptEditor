using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace XwaHangarMapEditor
{
    internal static class VectorExtensions
    {
        public static float Length(this Vector v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }

        public static Vector Scale(this Vector v, float scale)
        {
            return v.Scale(scale, scale, scale);
        }

        public static Vector Normalize(this Vector v)
        {
            float length = v.Length();
            if (length == 0)
            {
                return v;
            }

            return v.Scale(1.0f / length);
        }

        public static Point3D ToPoint3D(this Vector v)
        {
            return new Point3D(-v.Y, -v.X, v.Z);
        }

        public static Vector3D ToVector3D(this Vector v)
        {
            return new Vector3D(-v.Y, -v.X, v.Z);
        }

        public static float LengthFactor(this Vector v)
        {
            float length = v.Length();
            return length / 32768;
        }

        public static Vector Tranform(this Vector v, Transform3D t)
        {
            Point3D p = v.ToPoint3D();
            p = t.Transform(p);
            return p.ToVector();
        }

        public static Vector ToVector(this Point3D v)
        {
            return new Vector((float)-v.Y, (float)-v.X, (float)v.Z);
        }
    }
}
