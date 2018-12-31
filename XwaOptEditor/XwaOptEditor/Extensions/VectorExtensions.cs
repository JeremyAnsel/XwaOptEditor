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
    }
}
