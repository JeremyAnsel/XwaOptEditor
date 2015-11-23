using JeremyAnsel.Xwa.Opt;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OptTextures
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OptFile OptFile
        {
            get
            {
                return (OptFile)this.DataContext;
            }
            set
            {
                this.DataContext = null;
                this.DataContext = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Execute_Open(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Xwa OPT files|*.opt";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    this.OptFile = OptFile.FromFile(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
