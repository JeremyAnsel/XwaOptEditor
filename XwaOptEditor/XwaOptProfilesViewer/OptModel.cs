using JeremyAnsel.Xwa.HooksConfig;
using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptProfilesViewer
{
    static class OptModel
    {
        public static string GetBaseOptFilename(string filename)
        {
            string baseFilename = Path.ChangeExtension(filename, null);

            if (baseFilename.EndsWith("exterior", StringComparison.OrdinalIgnoreCase))
            {
                baseFilename = baseFilename.Substring(0, baseFilename.Length - "exterior".Length);
            }
            else if (baseFilename.EndsWith("cockpit", StringComparison.OrdinalIgnoreCase))
            {
                baseFilename = baseFilename.Substring(0, baseFilename.Length - "cockpit".Length);
            }

            return baseFilename;
        }

        public static Dictionary<string, List<int>> GetObjectProfiles(string filename)
        {
            string shipPath = OptModel.GetBaseOptFilename(filename);

            var profiles = new Dictionary<string, List<int>>();
            profiles["Default"] = new List<int>();

            var lines = XwaHooksConfig.GetFileLines(shipPath + "ObjectProfiles.txt");

            if (lines.Count == 0)
            {
                lines = XwaHooksConfig.GetFileLines(shipPath + ".ini", "ObjectProfiles");
            }

            foreach (string line in lines)
            {
                int pos = line.IndexOf('=');

                if (pos == -1)
                {
                    continue;
                }

                string name = line.Substring(0, pos).Trim();

                if (name.Length == 0)
                {
                    continue;
                }

                var values = XwaHooksConfig.Tokennize(line.Substring(pos + 1).Trim());
                var indices = new List<int>();

                foreach (string value in values)
                {
                    int index = XwaHooksConfig.ToInt32(value);
                    indices.Add(index);
                }

                profiles[name] = indices;
            }

            return profiles;
        }

        public static List<string> GetSkins(string filename)
        {
            var skins = new List<string>();

            string optName = Path.GetFileNameWithoutExtension(filename);
            string directory = Path.Combine(Path.GetDirectoryName(filename), "Skins", optName);

            if (Directory.Exists(directory))
            {
                foreach (string path in Directory.EnumerateDirectories(directory))
                {
                    skins.Add(Path.GetFileName(path));
                }
            }

            return skins;
        }

        public static OptFile GetTransformedOpt(OptFile optFile, List<int> objectProfile, List<string> skins)
        {
            if (optFile == null || string.IsNullOrEmpty(optFile.FileName))
            {
                return null;
            }

            var opt = optFile.Clone();

            ApplyObjectProfile(opt, objectProfile);
            ApplySkins(opt, skins);

            opt.CompactBuffers();
            opt.CompactTextures();

            return opt;
        }

        private static void ApplyObjectProfile(OptFile opt, List<int> objectProfile)
        {
            foreach (int index in objectProfile)
            {
                if (index < 0 || index >= opt.Meshes.Count)
                {
                    continue;
                }

                var mesh = opt.Meshes[index];
                mesh.Lods.Clear();
                mesh.Hardpoints.Clear();
                mesh.EngineGlows.Clear();
            }
        }

        private static void ApplySkins(OptFile opt, List<string> skins)
        {
            string optName = Path.GetFileNameWithoutExtension(opt.FileName);
            string directory = Path.GetDirectoryName(opt.FileName);

            var baseDefaultSkinDirectory = new DirectoryInfo($"{directory}\\Skins\\{optName}\\Default");
            bool baseDefaultSkinExists = baseDefaultSkinDirectory.Exists && baseDefaultSkinDirectory.EnumerateFiles().Any();
            bool hasSkins = baseDefaultSkinExists || skins.Count != 0;

            if (hasSkins)
            {
                UpdateOptFile(optName, opt, skins);
            }
        }

        private static void UpdateOptFile(string optName, OptFile opt, IList<string> baseSkins)
        {
            List<List<string>> fgSkins = ReadFgSkins(optName, baseSkins);
            List<string> distinctSkins = fgSkins.SelectMany(t => t).Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
            ICollection<string> texturesExist = GetTexturesExist(optName, opt, distinctSkins);
            CreateSwitchTextures(opt, texturesExist, fgSkins);
            UpdateSkins(optName, opt, fgSkins);
        }

        private static List<List<string>> ReadFgSkins(string optName, IList<string> baseSkins)
        {
            var fgSkins = new List<List<string>>(1);

            var skins = new List<string>(baseSkins);

            if (skins.Count == 0)
            {
                skins.Add("Default");
            }

            fgSkins.Add(skins);

            return fgSkins;
        }

        private static ICollection<string> GetTexturesExist(string optName, OptFile opt, List<string> distinctSkins)
        {
            var texturesExist = new SortedSet<string>();
            string directory = Path.GetDirectoryName(opt.FileName);

            foreach (string skin in distinctSkins)
            {
                string skinDirectory = $"{directory}\\Skins\\{optName}\\{skin}";

                var filesEnum = Directory.EnumerateFiles(skinDirectory)
                    .Select(t => Path.GetFileName(t));

                var filesSet = new SortedSet<string>(filesEnum, StringComparer.InvariantCultureIgnoreCase);

                foreach (string textureName in opt.Textures.Keys)
                {
                    if (TextureExists(filesSet, textureName, skin) != null)
                    {
                        texturesExist.Add(textureName);
                    }
                }
            }

            return texturesExist;
        }

        private static void CreateSwitchTextures(OptFile opt, ICollection<string> texturesExist, List<List<string>> fgSkins)
        {
            int fgCount = fgSkins.Count;

            if (fgCount == 0)
            {
                return;
            }

            var newTextures = new List<Texture>();

            foreach (var texture in opt.Textures)
            {
                if (!texturesExist.Contains(texture.Key))
                {
                    continue;
                }

                texture.Value.Convert8To32();

                for (int i = 0; i < fgCount; i++)
                {
                    Texture newTexture = texture.Value.Clone();
                    newTexture.Name += "_fg_" + i.ToString(CultureInfo.InvariantCulture) + "_" + string.Join(",", fgSkins[i]);
                    newTextures.Add(newTexture);
                }
            }

            foreach (var newTexture in newTextures)
            {
                opt.Textures.Add(newTexture.Name, newTexture);
            }

            foreach (var mesh in opt.Meshes)
            {
                foreach (var lod in mesh.Lods)
                {
                    foreach (var faceGroup in lod.FaceGroups)
                    {
                        if (faceGroup.Textures.Count == 0)
                        {
                            continue;
                        }

                        string name = faceGroup.Textures[0];

                        if (!texturesExist.Contains(name))
                        {
                            continue;
                        }

                        faceGroup.Textures.Clear();

                        for (int i = 0; i < fgCount; i++)
                        {
                            faceGroup.Textures.Add(name + "_fg_" + i.ToString(CultureInfo.InvariantCulture) + "_" + string.Join(",", fgSkins[i]));
                        }
                    }
                }
            }
        }

        private static void UpdateSkins(string optName, OptFile opt, List<List<string>> fgSkins)
        {
            opt.Textures.AsParallel().ForAll(texture =>
            {
                int position = texture.Key.IndexOf("_fg_");

                if (position == -1)
                {
                    return;
                }

                string textureName = texture.Key.Substring(0, position);
                int fgIndex = int.Parse(texture.Key.Substring(position + 4, texture.Key.IndexOf('_', position + 4) - position - 4), CultureInfo.InvariantCulture);
                string directory = Path.GetDirectoryName(opt.FileName);

                foreach (string skin in fgSkins[fgIndex])
                {
                    string skinDirectory = $"{directory}\\Skins\\{optName}\\{skin}";

                    var filesEnum = Directory.EnumerateFiles(skinDirectory)
                        .Select(t => Path.GetFileName(t));

                    var filesSet = new SortedSet<string>(filesEnum, StringComparer.InvariantCultureIgnoreCase);

                    string filename = TextureExists(filesSet, textureName, skin);

                    if (filename == null)
                    {
                        continue;
                    }

                    CombineTextures(texture.Value, Path.Combine(skinDirectory, filename));
                }

                texture.Value.GenerateMipmaps();
            });
        }

        private static void CombineTextures(Texture baseTexture, string filename)
        {
            Texture newTexture = Texture.FromFile(filename);

            if (newTexture.Width != baseTexture.Width || newTexture.Height != baseTexture.Height)
            {
                return;
            }

            newTexture.Convert8To32();

            int size = baseTexture.Width * baseTexture.Height;
            byte[] src = newTexture.ImageData;
            byte[] dst = baseTexture.ImageData;

            for (int i = 0; i < size; i++)
            {
                int a = src[i * 4 + 3];

                dst[i * 4 + 0] = (byte)(dst[i * 4 + 0] * (255 - a) / 255 + src[i * 4 + 0] * a / 255);
                dst[i * 4 + 1] = (byte)(dst[i * 4 + 1] * (255 - a) / 255 + src[i * 4 + 1] * a / 255);
                dst[i * 4 + 2] = (byte)(dst[i * 4 + 2] * (255 - a) / 255 + src[i * 4 + 2] * a / 255);
            }
        }

        private static readonly string[] _textureExtensions = new string[] { ".bmp", ".png", ".jpg" };

        private static string TextureExists(ICollection<string> files, string baseFilename, string skin)
        {
            foreach (string ext in _textureExtensions)
            {
                string filename = baseFilename + "_" + skin + ext;

                if (files.Contains(filename))
                {
                    return filename;
                }
            }

            foreach (string ext in _textureExtensions)
            {
                string filename = baseFilename + ext;

                if (files.Contains(filename))
                {
                    return filename;
                }
            }

            return null;
        }
    }
}
