using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XwaOptProfilesViewer
{
    static class ColorHelpers
    {
        public static Color FromUint(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color);

            return Color.FromArgb(a, r, g, b);
        }

        public static uint ToUint(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B));
        }
    }
}
