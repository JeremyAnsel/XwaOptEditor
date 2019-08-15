using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JeremyAnsel.Xwa.Opt;

namespace OptTextures
{
    internal static class TextureUtils
    {
        public const int DefaultPalette = 8;

        public static BitmapSource BuildOptTexture(Texture texture, int paletteIndex = TextureUtils.DefaultPalette, int level = 0)
        {
            if (texture == null || paletteIndex < 0 || paletteIndex > 15 || level < 0 || level >= texture.MipmapsCount)
                return null;

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

                int texWidth;
                int texHeight;
                byte[] texImageData = texture.GetMipmapImageData(level, out texWidth, out texHeight);

                int size = texWidth * texHeight;
                byte[] imageData = new byte[size * 4];

                for (int i = 0; i < size; i++)
                {
                    int colorIndex = texImageData[i];

                    imageData[i * 4 + 0] = palette.Colors[colorIndex].B;
                    imageData[i * 4 + 1] = palette.Colors[colorIndex].G;
                    imageData[i * 4 + 2] = palette.Colors[colorIndex].R;
                    imageData[i * 4 + 3] = texture.AlphaIllumData == null ? (byte)255 : texture.AlphaIllumData[i];
                }

                return BitmapSource.Create(texWidth, texHeight, 96, 96, PixelFormats.Bgra32, null, imageData, texWidth * 4);
            }
            else if (bpp == 32)
            {
                int texWidth;
                int texHeight;
                byte[] texImageData = texture.GetMipmapImageData(level, out texWidth, out texHeight);

                return BitmapSource.Create(texWidth, texHeight, 96, 96, PixelFormats.Bgra32, null, texImageData, texWidth * 4);
            }
            else
            {
                return null;
            }
        }

        public static BitmapSource BuildOptPalette(Texture texture)
        {
            if (texture == null)
                return null;

            const int width = 256;
            const int height = 16;

            byte[] imageData = new byte[width * height * 4];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ushort c = BitConverter.ToUInt16(texture.Palette, y * width * 2 + x * 2);

                    byte r = (byte)((c & 0xF800) >> 11);
                    byte g = (byte)((c & 0x7E0) >> 5);
                    byte b = (byte)(c & 0x1F);

                    r = (byte)((r * (0xffU * 2) + 0x1fU) / (0x1fU * 2));
                    g = (byte)((g * (0xffU * 2) + 0x3fU) / (0x3fU * 2));
                    b = (byte)((b * (0xffU * 2) + 0x1fU) / (0x1fU * 2));

                    int index = y * width * 4 + x * 4;
                    imageData[index + 0] = b;
                    imageData[index + 1] = g;
                    imageData[index + 2] = r;
                    imageData[index + 3] = 255;
                }
            }

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, imageData, width * 4);
        }
    }
}
