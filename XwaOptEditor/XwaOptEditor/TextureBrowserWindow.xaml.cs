using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using JeremyAnsel.Xwa.Opt;

namespace XwaOptEditor
{
    /// <summary>
    /// Logique d'interaction pour TextureBrowserWindow.xaml
    /// </summary>
    public partial class TextureBrowserWindow : Window
    {
        public TextureBrowserWindow(Window owner, OptFile optFile)
        {
            InitializeComponent();

            this.Owner = owner;
            this.DataContext = optFile;
        }

        public string TextureName { get; private set; }

        private void Textures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (KeyValuePair<string, Texture>)this.Textures.SelectedItem;
            this.TextureName = item.Value.Name;
            this.DialogResult = true;
            this.Close();
        }
    }
}
