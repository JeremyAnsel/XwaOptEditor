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
    /// Logique d'interaction pour MoveFactorSingleDialog.xaml
    /// </summary>
    public partial class MoveFactorSingleDialog : Window, INotifyPropertyChanged
    {
        private float centerX;

        private float centerY;

        private float centerZ;

        private float positionX;

        private float positionY;

        private float positionZ;

        private float moveX;

        private float moveY;

        private float moveZ;

        public MoveFactorSingleDialog(Window owner)
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
                if (value == this.centerX)
                {
                    return;
                }

                this.centerX = value;
                this.NotifyPropertyChanged("CenterX");
                this.NotifyPropertyChanged("CenterXMeter");

                this.PositionX = this.CenterX;
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
                if (value == this.centerY)
                {
                    return;
                }

                this.centerY = value;
                this.NotifyPropertyChanged("CenterY");
                this.NotifyPropertyChanged("CenterYMeter");

                this.PositionY = this.CenterY;
            }
        }

        public float CenterZ
        {
            get
            {
                return this.centerZ;
            }

            set
            {
                if (value == this.centerZ)
                {
                    return;
                }

                this.centerZ = value;
                this.NotifyPropertyChanged("CenterZ");
                this.NotifyPropertyChanged("CenterZMeter");

                this.PositionZ = this.CenterZ;
            }
        }

        public float CenterXMeter
        {
            get
            {
                return this.CenterX * OptFile.ScaleFactor;
            }

            set
            {
                this.CenterX = value / OptFile.ScaleFactor;
            }
        }

        public float CenterYMeter
        {
            get
            {
                return this.CenterY * OptFile.ScaleFactor;
            }

            set
            {
                this.CenterY = value / OptFile.ScaleFactor;
            }
        }

        public float CenterZMeter
        {
            get
            {
                return this.CenterZ * OptFile.ScaleFactor;
            }

            set
            {
                this.CenterZ = value / OptFile.ScaleFactor;
            }
        }

        public float PositionX
        {
            get
            {
                return this.positionX;
            }

            set
            {
                if (value == this.positionX)
                {
                    return;
                }

                this.positionX = value;
                this.NotifyPropertyChanged("PositionX");
                this.NotifyPropertyChanged("PositionXMeter");

                this.MoveX = this.PositionX - this.CenterX;
            }
        }

        public float PositionY
        {
            get
            {
                return this.positionY;
            }

            set
            {
                if (value == this.positionY)
                {
                    return;
                }

                this.positionY = value;
                this.NotifyPropertyChanged("PositionY");
                this.NotifyPropertyChanged("PositionYMeter");

                this.MoveY = this.PositionY - this.CenterY;
            }
        }

        public float PositionZ
        {
            get
            {
                return this.positionZ;
            }

            set
            {
                if (value == this.positionZ)
                {
                    return;
                }

                this.positionZ = value;
                this.NotifyPropertyChanged("PositionZ");
                this.NotifyPropertyChanged("PositionZMeter");

                this.MoveZ = this.PositionZ - this.CenterZ;
            }
        }

        public float PositionXMeter
        {
            get
            {
                return this.PositionX * OptFile.ScaleFactor;
            }

            set
            {
                this.PositionX = value / OptFile.ScaleFactor;
            }
        }

        public float PositionYMeter
        {
            get
            {
                return this.PositionY * OptFile.ScaleFactor;
            }

            set
            {
                this.PositionY = value / OptFile.ScaleFactor;
            }
        }

        public float PositionZMeter
        {
            get
            {
                return this.PositionZ * OptFile.ScaleFactor;
            }

            set
            {
                this.PositionZ = value / OptFile.ScaleFactor;
            }
        }

        public float MoveX
        {
            get
            {
                return this.moveX;
            }

            set
            {
                if (value == this.moveX)
                {
                    return;
                }

                this.moveX = value;
                this.NotifyPropertyChanged("MoveX");
                this.NotifyPropertyChanged("MoveXMeter");

                this.PositionX = this.CenterX + this.MoveX;
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
                if (value == this.moveY)
                {
                    return;
                }

                this.moveY = value;
                this.NotifyPropertyChanged("MoveY");
                this.NotifyPropertyChanged("MoveYMeter");

                this.PositionY = this.CenterY + this.MoveY;
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
                if (value == this.moveZ)
                {
                    return;
                }

                this.moveZ = value;
                this.NotifyPropertyChanged("MoveZ");
                this.NotifyPropertyChanged("MoveZMeter");

                this.PositionZ = this.CenterZ + this.MoveZ;
            }
        }

        public float MoveXMeter
        {
            get
            {
                return this.MoveX * OptFile.ScaleFactor;
            }

            set
            {
                this.MoveX = value / OptFile.ScaleFactor;
            }
        }

        public float MoveYMeter
        {
            get
            {
                return this.MoveY * OptFile.ScaleFactor;
            }

            set
            {
                this.MoveY = value / OptFile.ScaleFactor;
            }
        }

        public float MoveZMeter
        {
            get
            {
                return this.MoveZ * OptFile.ScaleFactor;
            }

            set
            {
                this.MoveZ = value / OptFile.ScaleFactor;
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
