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
    /// Logique d'interaction pour ScaleFactorDialog.xaml
    /// </summary>
    public partial class ScaleFactorDialog : Window, INotifyPropertyChanged
    {
        private float scaleX = 1.0f;

        private float scaleY = 1.0f;

        private float scaleZ = 1.0f;

        public ScaleFactorDialog(Window owner, float sizeX, float sizeY, float sizeZ)
        {
            InitializeComponent();

            this.SizeX = sizeX;
            this.SizeY = sizeY;
            this.SizeZ = sizeZ;

            this.Owner = owner;

            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public float SizeX { get; private set; }

        public float SizeY { get; private set; }

        public float SizeZ { get; private set; }

        public float ScaleX
        {
            get { return this.scaleX; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.scaleX = value;
                this.NotifyPropertyChanged("ScaleX");
                this.NotifyPropertyChanged("ScaleXDiv");
                this.NotifyPropertyChanged("ScaleXMeter");

                this.NotifyScaleFactorChanged();
            }
        }

        public float ScaleXDiv
        {
            get { return 1.0f / this.ScaleX; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleX = 1.0f / value;
            }
        }

        public float ScaleXMeter
        {
            get { return this.SizeX * this.ScaleX; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleX = value / this.SizeX;
            }
        }

        public float ScaleY
        {
            get { return this.scaleY; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.scaleY = value;
                this.NotifyPropertyChanged("ScaleY");
                this.NotifyPropertyChanged("ScaleYDiv");
                this.NotifyPropertyChanged("ScaleYMeter");

                this.NotifyScaleFactorChanged();
            }
        }

        public float ScaleYDiv
        {
            get { return 1.0f / this.ScaleY; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleY = 1.0f / value;
            }
        }

        public float ScaleYMeter
        {
            get { return this.SizeY * this.ScaleY; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleY = value / this.SizeY;
            }
        }

        public float ScaleZ
        {
            get { return this.scaleZ; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.scaleZ = value;
                this.NotifyPropertyChanged("ScaleZ");
                this.NotifyPropertyChanged("ScaleZDiv");
                this.NotifyPropertyChanged("ScaleZMeter");

                this.NotifyScaleFactorChanged();
            }
        }

        public float ScaleZDiv
        {
            get { return 1.0f / this.ScaleZ; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleZ = 1.0f / value;
            }
        }

        public float ScaleZMeter
        {
            get { return this.SizeZ * this.ScaleZ; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleZ = value / this.SizeZ;
            }
        }

        public string ScaleType
        {
            get
            {
                if (this.ScaleX == this.ScaleY && this.ScaleX == this.ScaleZ)
                {
                    return "Scaling";
                }
                else if (this.ScaleY == this.ScaleZ)
                {
                    return "Stretching X";
                }
                else if (this.ScaleX == this.ScaleZ)
                {
                    return "Stretching Y";
                }
                else if (this.ScaleX == this.ScaleY)
                {
                    return "Stretching Z";
                }
                else
                {
                    return "Stretching";
                }
            }
        }

        public float ScaleFactor
        {
            get
            {
                double length = this.ScaleX * this.ScaleY * this.ScaleZ;

                if (length < 0)
                {
                    return -(float)Math.Pow(-length, 1.0 / 3.0);
                }
                else
                {
                    return (float)Math.Pow(length, 1.0 / 3.0);
                }
            }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleX = value;
                this.ScaleY = value;
                this.ScaleZ = value;
            }
        }

        public float ScaleFactorDiv
        {
            get { return 1.0f / this.ScaleFactor; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleFactor = 1.0f / value;
            }
        }

        public float ScaleFactorMeterX
        {
            get { return this.SizeX * this.ScaleFactor; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleFactor = value / this.SizeX;
            }
        }

        public float ScaleFactorMeterY
        {
            get { return this.SizeY * this.ScaleFactor; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleFactor = value / this.SizeY;
            }
        }

        public float ScaleFactorMeterZ
        {
            get { return this.SizeZ * this.ScaleFactor; }

            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ScaleFactor = value / this.SizeZ;
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NotifyScaleFactorChanged()
        {
            this.NotifyPropertyChanged("ScaleType");
            this.NotifyPropertyChanged("ScaleFactor");
            this.NotifyPropertyChanged("ScaleFactorDiv");
            this.NotifyPropertyChanged("ScaleFactorMeterX");
            this.NotifyPropertyChanged("ScaleFactorMeterY");
            this.NotifyPropertyChanged("ScaleFactorMeterZ");
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
