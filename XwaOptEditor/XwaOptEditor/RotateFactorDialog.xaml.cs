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
    /// Logique d'interaction pour RotateFactorDialog.xaml
    /// </summary>
    public partial class RotateFactorDialog : Window, INotifyPropertyChanged
    {
        private float centerX;

        private float centerY;

        private float angle;

        public RotateFactorDialog(Window owner)
        {
            InitializeComponent();

            this.Owner = owner;

            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public float CenterX
        {
            get
            {
                return this.centerX;
            }

            set
            {
                this.centerX = value;

                this.NotifyPropertyChanged("CenterX");
                this.NotifyPropertyChanged("CenterXMeter");
            }
        }

        public float CenterXMeter
        {
            get
            {
                return this.centerX * OptFile.ScaleFactor;
            }

            set
            {
                this.centerX = value / OptFile.ScaleFactor;

                this.NotifyPropertyChanged("CenterX");
                this.NotifyPropertyChanged("CenterXMeter");
            }
        }

        public float CenterY
        {
            get
            {
                return this.centerY;
            }

            set
            {
                this.centerY = value;

                this.NotifyPropertyChanged("CenterY");
                this.NotifyPropertyChanged("CenterYMeter");
            }
        }

        public float CenterYMeter
        {
            get
            {
                return this.centerY * OptFile.ScaleFactor;
            }

            set
            {
                this.centerY = value / OptFile.ScaleFactor;

                this.NotifyPropertyChanged("CenterY");
                this.NotifyPropertyChanged("CenterYMeter");
            }
        }

        public float Angle
        {
            get
            {
                return this.angle;
            }

            set
            {
                this.angle = value;

                this.NotifyPropertyChanged("Angle");
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
