using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JeremyAnsel.Xwa.Opt;

namespace XwaOptEditor.Helpers
{
    static class TextureHelpers
    {
        public const int DefaultPalette = 8;

        public static BitmapSource BuildOptTexture(Texture texture)
        {
            return TextureHelpers.BuildOptTexture(texture, TextureHelpers.DefaultPalette);
        }

        public static BitmapSource BuildOptTexture(Texture texture, int paletteIndex)
        {
            if (texture == null || paletteIndex < 0 || paletteIndex > 15)
            {
                return null;
            }

            int size = texture.Width * texture.Height;
            int bpp = texture.BitsPerPixel;

            if (bpp == 8)
            {
                var palette = new BitmapPalette(Enumerable.Range(0, 256)
                    .Select(i =>
                    {
                        ushort c = BitConverter.ToUInt16(texture.Palette, paletteIndex * 512 + i * 2);

                        byte r = (byte)((c & 0xF800) >> 11);
                        byte g = (byte)((c & 0x7E0) >> 5);
                        byte b = (byte)(c & 0x1F);

                        r = (byte)((r * (0xffU * 2) + 0x1fU) / (0x1fU * 2));
                        g = (byte)((g * (0xffU * 2) + 0x3fU) / (0x3fU * 2));
                        b = (byte)((b * (0xffU * 2) + 0x1fU) / (0x1fU * 2));

                        return Color.FromRgb(r, g, b);
                    })
                    .ToList());

                if (texture.AlphaData == null)
                {
                    byte[] imageData = new byte[size];
                    Array.Copy(texture.ImageData, imageData, size);

                    return BitmapSource.Create(texture.Width, texture.Height, 96, 96, PixelFormats.Indexed8, palette, imageData, texture.Width);
                }
                else
                {
                    byte[] imageData = new byte[size * 4];

                    for (int i = 0; i < size; i++)
                    {
                        int colorIndex = texture.ImageData[i];

                        imageData[i * 4 + 0] = palette.Colors[colorIndex].B;
                        imageData[i * 4 + 1] = palette.Colors[colorIndex].G;
                        imageData[i * 4 + 2] = palette.Colors[colorIndex].R;
                        imageData[i * 4 + 3] = texture.AlphaData[i];
                    }

                    return BitmapSource.Create(texture.Width, texture.Height, 96, 96, PixelFormats.Bgra32, null, imageData, texture.Width * 4);
                }
            }
            else if (bpp == 32)
            {
                byte[] imageData = new byte[size * 4];
                Array.Copy(texture.ImageData, imageData, size * 4);

                return BitmapSource.Create(texture.Width, texture.Height, 96, 96, PixelFormats.Bgra32, null, imageData, texture.Width * 4);
            }
            else
            {
                return null;
            }
        }

        public static BitmapSource BuildOptTextureAlpha(Texture texture)
        {
            if (texture == null || !texture.HasAlpha)
            {
                return null;
            }

            int size = texture.Width * texture.Height;

            byte[] imageData = new byte[size];
            Array.Copy(texture.AlphaData, imageData, size);

            return BitmapSource.Create(texture.Width, texture.Height, 96, 96, PixelFormats.Gray8, null, imageData, texture.Width);
        }
    }
}
