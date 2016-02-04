using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace XwaOptEditor
{
    /// <summary>
    /// Logique d'interaction pour MoveFactorDialog.xaml
    /// </summary>
    public partial class MoveFactorDialog : Window, INotifyPropertyChanged
    {
        private float moveX;

        private float moveY;

        private float moveZ;

        public MoveFactorDialog(Window owner)
        {
            InitializeComponent();

            this.Owner = owner;

            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public float MoveX
        {
            get
            {
                return this.moveX;
            }

            set
            {
                this.moveX = value;

                this.NotifyPropertyChanged("MoveX");
                this.NotifyPropertyChanged("MoveXMeter");
            }
        }

        public float MoveXMeter
        {
            get
            {
                return this.moveX * OptFile.ScaleFactor;
            }

            set
            {
                this.moveX = value / OptFile.ScaleFactor;

                this.NotifyPropertyChanged("MoveX");
                this.NotifyPropertyChanged("MoveXMeter");
            }
        }

        public float MoveY
        {
            get
            {
                return this.moveY;
            }

            set
            {
                this.moveY = value;

                this.NotifyPropertyChanged("MoveY");
                this.NotifyPropertyChanged("MoveYMeter");
            }
        }

        public float MoveYMeter
        {
            get
            {
                return this.moveY * OptFile.ScaleFactor;
            }

            set
            {
                this.moveY = value / OptFile.ScaleFactor;

                this.NotifyPropertyChanged("MoveY");
                this.NotifyPropertyChanged("MoveYMeter");
            }
        }

        public float MoveZ
        {
            get
            {
                return this.moveZ;
            }

            set
            {
                this.moveZ = value;

                this.NotifyPropertyChanged("MoveZ");
                this.NotifyPropertyChanged("MoveZMeter");
            }
        }

        public float MoveZMeter
        {
            get
            {
                return this.moveZ * OptFile.ScaleFactor;
            }

            set
            {
                this.moveZ = value / OptFile.ScaleFactor;

                this.NotifyPropertyChanged("MoveZ");
                this.NotifyPropertyChanged("MoveZMeter");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
