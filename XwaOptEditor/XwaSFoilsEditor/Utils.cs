using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace XwaSFoilsEditor
{
    internal static class Utils
    {
        public static Transform3D GetSFoilTransform(MeshModel sfoil, double showSFoilsOpened, bool usePivot)
        {
            double angle = sfoil.Angle * 360.0 / 255;
            angle *= showSFoilsOpened;

            Transform3D transform;
            Point3D pivot = usePivot ? sfoil.Pivot.ToPoint3D() : new Point3D();

            if (sfoil.Look.Length() == 0)
            {
                double a = angle * Math.PI / 180.0;
                double cosA = Math.Cos(a);
                transform = new ScaleTransform3D(new Vector3D(cosA, cosA, cosA), pivot);
            }
            else
            {
                double a = angle * sfoil.Look.LengthFactor();
                transform = new RotateTransform3D(new AxisAngleRotation3D(sfoil.Look.ToVector3D(), a), pivot);
            }

            transform.Freeze();
            return transform;
        }
    }
}
