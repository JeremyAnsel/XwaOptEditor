using JeremyAnsel.Xwa.Opt;
using System.Globalization;
using System.IO;

Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

Console.WriteLine("Xwa Opt Skins Splitter");

string optDirectory = GetOptDirectory();

if (optDirectory is null)
{
    return;
}

Console.WriteLine(optDirectory);

if (string.Equals(optDirectory, Path.Combine(Directory.GetCurrentDirectory(), "FlightModels"), StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("You have selected the output directory. Please select another directory.");
    return;
}

if (Directory.Exists("FlightModels"))
{
    Directory.Delete("FlightModels", true);
}

Directory.CreateDirectory(Path.Combine("FlightModels", "Skins"));

foreach (string optFileName in Directory.EnumerateFiles(optDirectory, "*.opt"))
{
    Console.WriteLine(Path.GetFileName(optFileName));

    try
    {
        OptFile opt = OptFile.FromFile(optFileName);

        ExportOptSkins(opt);
    }
    catch (Exception ex)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.ToString());
        Console.ResetColor();
    }
}

Console.WriteLine("Press any key to continue");
Console.ReadKey(true);

string GetOptDirectory()
{
    var dlg = new FolderBrowserForWPF.Dialog
    {
        Title = "Choose a directory containing OPTs"
    };

    if (dlg.ShowDialog() != true)
    {
        return null;
    }

    return dlg.FileName;
}

void ExportOptSkins(OptFile opt)
{
    if (opt == null)
    {
        throw new ArgumentNullException(nameof(opt));
    }

    if (string.IsNullOrEmpty(opt.FileName))
    {
        return;
    }

    string optName = Path.GetFileNameWithoutExtension(opt.FileName);
    int maxTextureVersion = opt.MaxTextureVersion;

    if (maxTextureVersion <= 1)
    {
        return;
    }

    opt = opt.Clone();

    opt.CompactBuffers();
    opt.RemoveUnusedTextures();

    string createMarkingsSkinDirectoryPath(int markings) => Path.Combine(
        "FlightModels",
        "Skins",
        optName,
        "Default_" + markings.ToString(CultureInfo.InvariantCulture));

    for (int markings = 0; markings < maxTextureVersion; markings++)
    {
        Directory.CreateDirectory(createMarkingsSkinDirectoryPath(markings));
    }

    var skinsTextures = new Dictionary<string, string>(opt.Textures.Count, StringComparer.OrdinalIgnoreCase);

    foreach (Mesh mesh in opt.Meshes)
    {
        foreach (var lod in mesh.Lods)
        {
            foreach (var faceGroup in lod.FaceGroups)
            {
                if (faceGroup.Textures.Count <= 1)
                {
                    continue;
                }

                string baseTextureName = faceGroup.Textures[0];

                for (int markings = 0; markings < faceGroup.Textures.Count; markings++)
                {
                    string skinTextureName = faceGroup.Textures[markings];

                    string key = Path.Combine(createMarkingsSkinDirectoryPath(markings), baseTextureName);
                    string value = skinTextureName;

                    if (skinsTextures.ContainsKey(key))
                    {
                        continue;
                    }

                    skinsTextures.Add(key, value);
                }

                faceGroup.Textures.Clear();
                faceGroup.Textures.Add(baseTextureName);
            }
        }
    }

    foreach (var skin in skinsTextures)
    {
        var texture = opt.Textures[skin.Value];
        texture.Save(skin.Key + ".png");
    }

    opt.RemoveUnusedTextures();
    opt.Save(Path.Combine("FlightModels", optName + ".opt"));
}
