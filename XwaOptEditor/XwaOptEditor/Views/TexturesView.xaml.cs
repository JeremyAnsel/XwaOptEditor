using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XwaOptEditor.Models;
using XwaOptEditor.ViewModels;

namespace XwaOptEditor.Views
{
    /// <summary>
    /// Logique d'interaction pour TexturesView.xaml
    /// </summary>
    public partial class TexturesView : UserControl
    {
        public TexturesView()
        {
            InitializeComponent();
        }

        private void zommbox_Loaded(object sender, RoutedEventArgs e)
        {
            var zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox)sender;
            zoombox.FitToBounds();
            zoombox.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderVisibilityProperty, Visibility.Visible);
        }

        private TexturesViewModel ViewModel
        {
            get { return (TexturesViewModel)this.DataContext; }
        }

        public OptModel OptModel
        {
            get { return (OptModel)this.GetValue(TexturesView.OptModelProperty); }
            set { this.SetValue(TexturesView.OptModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OptModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptModelProperty =
            DependencyProperty.Register("OptModel", typeof(OptModel), typeof(TexturesView), new PropertyMetadata(new PropertyChangedCallback(TexturesView.OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var view = (TexturesView)sender;

            switch (e.Property.Name)
            {
                case "OptModel":
                    view.ViewModel.OptModel = (OptModel)e.NewValue;
                    break;
            }
        }

        private void ImageIllumination_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = (Image)sender;

            if (image == null)
            {
                return;
            }

            var source = image.Source as BitmapSource;

            if (source == null)
            {
                return;
            }

            var position = e.GetPosition(image);

            int x = Math.Max(Math.Min((int)(position.X * source.PixelWidth / image.ActualWidth), source.PixelWidth - 1), 0);
            int y = Math.Max(Math.Min((int)(position.Y * source.PixelHeight / image.ActualHeight), source.PixelHeight - 1), 0);

            byte[] pixel = new byte[4];

            source.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, source.PixelWidth * 4, 0);

            var color = Color.FromRgb(pixel[2], pixel[1], pixel[0]);

            this.textureIlluminationColorKey.SelectedColor = color;
            this.textureIlluminationColorKey0.SelectedColor = color;
            this.textureIlluminationColorKey1.SelectedColor = color;
        }

        private void SetTextureIlluminationColorKey_Click(object sender, RoutedEventArgs e)
        {
            if (this.Textures.SelectedItem == null)
            {
                return;
            }

            if (this.textureIlluminationColorKey.SelectedColor == null)
            {
                return;
            }

            var texture = ((KeyValuePair<string, Texture>)this.Textures.SelectedItem).Value;

            Color color = this.textureIlluminationColorKey.SelectedColor.Value;
            byte tolerance = 0;
            byte.TryParse(this.textureIlluminationTolerance.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out tolerance);
            this.textureIlluminationTolerance.Text = tolerance.ToString(CultureInfo.InvariantCulture);

            Color color0 = Color.FromRgb((byte)Math.Max(color.R - tolerance, 0), (byte)Math.Max(color.G - tolerance, 0), (byte)Math.Max(color.B - tolerance, 0));
            Color color1 = Color.FromRgb((byte)Math.Min(color.R + tolerance, 255), (byte)Math.Min(color.G + tolerance, 255), (byte)Math.Min(color.B + tolerance, 255));

            texture.MakeColorIlluminated(color0.R, color0.G, color0.B, color1.R, color1.G, color1.B);

            this.OptModel.File = this.OptModel.File;
            this.OptModel.UndoStackPush("set illumination");
        }

        private void SetTextureIlluminationColorKeyRange_Click(object sender, RoutedEventArgs e)
        {
            if (this.Textures.SelectedItem == null)
            {
                return;
            }

            if (this.textureIlluminationColorKey0.SelectedColor == null || this.textureIlluminationColorKey1.SelectedColor == null)
            {
                return;
            }

            var texture = ((KeyValuePair<string, Texture>)this.Textures.SelectedItem).Value;

            Color color0 = this.textureIlluminationColorKey0.SelectedColor.Value;
            Color color1 = this.textureIlluminationColorKey1.SelectedColor.Value;

            texture.MakeColorIlluminated(color0.R, color0.G, color0.B, color1.R, color1.G, color1.B);

            this.OptModel.File = this.OptModel.File;
            this.OptModel.UndoStackPush("set illumination");
        }

        private void SetTextureIlluminationReset_Click(object sender, RoutedEventArgs e)
        {
            if (this.Textures.SelectedItem == null)
            {
                return;
            }

            foreach (KeyValuePair<string, Texture> texture in this.Textures.SelectedItems)
            {
                texture.Value.ResetPaletteColors();
            }

            this.OptModel.File = this.OptModel.File;
            this.OptModel.UndoStackPush("reset illumination");
        }

        private void SetTextureAlphaReset_Click(object sender, RoutedEventArgs e)
        {
            if (this.Textures.SelectedItem == null)
            {
                return;
            }

            foreach (KeyValuePair<string, Texture> texture in this.Textures.SelectedItems)
            {
                int bpp = texture.Value.BitsPerPixel;

                if (bpp == 8)
                {
                    texture.Value.AlphaIllumData = null;
                }
                else if (bpp == 32)
                {
                    byte[] imageData = texture.Value.ImageData;
                    int length = imageData.Length / 4;

                    for (int i = 0; i < length; i++)
                    {
                        imageData[i * 4 + 3] = 0xff;
                    }

                    texture.Value.Palette[2] = 0;
                }
            }

            this.OptModel.File = this.OptModel.File;
            this.OptModel.UndoStackPush("reset alpha");
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            var texture = (KeyValuePair<string, Texture>)row.DataContext;

            this.ViewModel.RenameTextureCommand.Execute(texture.Value);
        }
    }
}
