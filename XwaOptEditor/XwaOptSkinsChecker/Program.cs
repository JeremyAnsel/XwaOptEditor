using JeremyAnsel.Xwa.Opt;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

Console.WriteLine("Xwa Opt Skins Checker");

string optDirectory = GetOptDirectory();

if (optDirectory is null)
{
    return;
}

Console.WriteLine(optDirectory);

string skinsDirectory = System.IO.Path.Combine(optDirectory, "Skins");

if (!System.IO.Directory.Exists(skinsDirectory))
{
    return;
}

try
{
    var sb = new StringBuilder();

    foreach (string optSkinDirectory in System.IO.Directory.EnumerateDirectories(skinsDirectory))
    {
        string optSkin = System.IO.Path.GetFileName(optSkinDirectory);
        Console.WriteLine(optSkin);

        string optFilename = System.IO.Path.Combine(optDirectory, optSkin + ".opt");

        if (!System.IO.File.Exists(optFilename))
        {
            continue;
        }

        OptFile opt = OptFile.FromFile(optFilename);

        foreach (string skinDirectory in System.IO.Directory.EnumerateDirectories(optSkinDirectory))
        {
            string skinName = System.IO.Path.GetFileName(skinDirectory);

            foreach (string imageFilename in System.IO.Directory.EnumerateFiles(skinDirectory))
            {
                BitmapFrame bitmapFrame = null;

                try
                {
                    using var fs = new System.IO.FileStream(imageFilename, System.IO.FileMode.Open);
                    bitmapFrame = BitmapFrame.Create(fs);
                }
                catch
                {
                    continue;
                }

                if (bitmapFrame is null)
                {
                    continue;
                }

                int imageWidth = bitmapFrame.PixelWidth;
                int imageHeight = bitmapFrame.PixelHeight;

                string imageName = System.IO.Path.GetFileNameWithoutExtension(imageFilename);
                Texture texture = opt.Textures
                    .Values
                    .FirstOrDefault(t => t.Name.Equals(imageName, StringComparison.OrdinalIgnoreCase));

                if (texture is null)
                {
                    continue;
                }

                if (texture.Width != imageWidth || texture.Height != imageHeight)
                {
                    sb.AppendLine($"OPT {optSkin} SKIN {skinName} contains mismatch texture {imageName}");
                }
            }
        }
    }

    Console.WriteLine("Log is written to XwaOptSkinsChecker.txt");
    System.IO.File.WriteAllText("XwaOptSkinsChecker.txt", sb.ToString());
}
catch (Exception ex)
{
    MessageBox.Show(ex.ToString(), "Xwa Opt Skins Checker error", MessageBoxButton.OK, MessageBoxImage.Error);
}

string GetOptDirectory()
{
    var dialog = new Microsoft.Win32.OpenFileDialog
    {
        Title = "Open OPT file",
        CheckFileExists = true,
        AddExtension = true,
        DefaultExt = ".opt",
        Filter = "OPT files|*.opt"
    };

    if (dialog.ShowDialog() != true)
    {
        return null;
    }

    string directory = System.IO.Path.GetDirectoryName(dialog.FileName);
    return directory;
}
