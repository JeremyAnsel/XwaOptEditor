using JeremyAnsel.IO.Locator;
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

                foreach (string path in Directory.EnumerateFiles(directory, "*.zip"))
                {
                    skins.Add(Path.GetFileNameWithoutExtension(path));
                }

                foreach (string path in Directory.EnumerateFiles(directory, "*.7z"))
                {
                    skins.Add(Path.GetFileNameWithoutExtension(path));
                }
            }

            return skins;
        }

        public static OptFile GetTransformedOpt(OptFile optFile, int version, List<int> objectProfile, List<string> skins)
        {
            if (optFile == null || string.IsNullOrEmpty(optFile.FileName))
            {
                return null;
            }

            var opt = optFile.Clone();

            SelectOptVersion(opt, version);
            ApplyObjectProfile(opt, objectProfile);
            ApplySkins(opt, skins);

            opt.CompactBuffers();
            opt.CompactTextures();

            return opt;
        }

        private static void SelectOptVersion(OptFile opt, int version)
        {
            var facegroups = opt.Meshes
                .SelectMany(t => t.Lods)
                .SelectMany(t => t.FaceGroups);

            foreach (var facegroup in facegroups)
            {
                if (facegroup.Textures.Count <= 1)
                {
                    continue;
                }

                int currentVersion = version;

                if (version < 0 || version >= facegroup.Textures.Count)
                {
                    currentVersion = facegroup.Textures.Count - 1;
                }

                string texture = facegroup.Textures[currentVersion];
                facegroup.Textures.Clear();
                facegroup.Textures.Add(texture);
            }

            opt.CompactTextures();
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

        private static string GetSkinDirectoryLocatorPath(string directory, string optName, string skinName)
        {
            string path = $"{directory}\\Skins\\{optName}\\{skinName}";

            var baseDirectoryInfo = new DirectoryInfo(path);
            bool baseDirectoryExists = baseDirectoryInfo.Exists && baseDirectoryInfo.EnumerateFiles().Any();

            if (baseDirectoryExists)
            {
                return path;
            }

            if (File.Exists(path + ".zip"))
            {
                return path + ".zip";
            }

            if (File.Exists(path + ".7z"))
            {
                return path + ".7z";
            }

            return null;
        }

        private static void ApplySkins(OptFile opt, List<string> skins)
        {
            string optName = Path.GetFileNameWithoutExtension(opt.FileName);
            string directory = Path.GetDirectoryName(opt.FileName);

            bool hasDefaultSkin = GetSkinDirectoryLocatorPath(directory, optName, "Default") != null;
            bool hasSkins = hasDefaultSkin || skins.Count != 0;

            if (hasSkins)
            {
                UpdateOptFile(optName, opt, skins);
            }
        }

        private static void UpdateOptFile(string optName, OptFile opt, IList<string> baseSkins)
        {
            List<List<string>> fgSkins = ReadFgSkins(optName, baseSkins);
            List<string> distinctSkins = fgSkins.SelectMany(t => t).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            ICollection<string> texturesExist = GetTexturesExist(optName, opt, distinctSkins);
            CreateSwitchTextures(opt, texturesExist, fgSkins);
            UpdateSkins(optName, opt, distinctSkins, fgSkins);
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
                string path = GetSkinDirectoryLocatorPath(directory, optName, skin);

                if (path == null)
                {
                    continue;
                }

                SortedSet<string> filesSet;

                using (IFileLocator locator = FileLocatorFactory.Create(path))
                {
                    if (locator == null)
                    {
                        continue;
                    }

                    var filesEnum = locator.EnumerateFiles()
                        .Select(t => Path.GetFileName(t));

                    filesSet = new SortedSet<string>(filesEnum, StringComparer.OrdinalIgnoreCase);
                }

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

        private static void UpdateSkins(string optName, OptFile opt, List<string> distinctSkins, List<List<string>> fgSkins)
        {
            var locatorsPath = new Dictionary<string, string>(distinctSkins.Count, StringComparer.OrdinalIgnoreCase);
            var filesSets = new Dictionary<string, SortedSet<string>>(distinctSkins.Count, StringComparer.OrdinalIgnoreCase);
            string directory = Path.GetDirectoryName(opt.FileName);

            foreach (string skin in distinctSkins)
            {
                string path = GetSkinDirectoryLocatorPath(directory, optName, skin);
                locatorsPath.Add(skin, path);

                SortedSet<string> filesSet = null;

                if (path != null)
                {
                    using (IFileLocator locator = FileLocatorFactory.Create(path))
                    {
                        if (locator != null)
                        {
                            var filesEnum = locator.EnumerateFiles()
                                .Select(t => Path.GetFileName(t));

                            filesSet = new SortedSet<string>(filesEnum, StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }

                filesSets.Add(skin, filesSet ?? new SortedSet<string>());
            }

            opt.Textures.AsParallel().ForAll(texture =>
            {
                int position = texture.Key.IndexOf("_fg_");

                if (position == -1)
                {
                    return;
                }

                string textureName = texture.Key.Substring(0, position);
                int fgIndex = int.Parse(texture.Key.Substring(position + 4, texture.Key.IndexOf('_', position + 4) - position - 4), CultureInfo.InvariantCulture);

                foreach (string skin in fgSkins[fgIndex])
                {
                    string path = locatorsPath[skin];

                    if (path == null)
                    {
                        continue;
                    }

                    string filename = TextureExists(filesSets[skin], textureName, skin);

                    if (filename == null)
                    {
                        continue;
                    }

                    using (IFileLocator locator = FileLocatorFactory.Create(path))
                    {
                        if (locator == null)
                        {
                            continue;
                        }

                        CombineTextures(texture.Value, locator, filename);
                    }
                }

                texture.Value.GenerateMipmaps();
            });
        }

        private static void CombineTextures(Texture baseTexture, IFileLocator locator, string filename)
        {
            Texture newTexture;

            using (Stream file = locator.Open(filename))
            {
                newTexture = Texture.FromStream(file);
                newTexture.Name = Path.GetFileNameWithoutExtension(filename);
            }

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
