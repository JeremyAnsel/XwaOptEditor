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
    /// Logique d'interaction pour ChangeAxesDialog.xaml
    /// </summary>
    public partial class ChangeAxesDialog : Window, INotifyPropertyChanged
    {
        private int axisX;

        private int axisY;

        private int axisZ;

        public ChangeAxesDialog(Window owner)
        {
            InitializeComponent();

            this.axisX = 1;
            this.axisY = 2;
            this.axisZ = 3;

            this.Owner = owner;

            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int AxisX
        {
            get { return this.axisX; }

            private set
            {
                this.axisX = value;
                this.NotifyPropertyChanged("AxisX");
                this.NotifyPropertyChanged("AxisXName");
            }
        }

        public int AxisY
        {
            get { return this.axisY; }

            private set
            {
                this.axisY = value;
                this.NotifyPropertyChanged("AxisY");
                this.NotifyPropertyChanged("AxisYName");
            }
        }

        public int AxisZ
        {
            get { return this.axisZ; }

            private set
            {
                this.axisZ = value;
                this.NotifyPropertyChanged("AxisZ");
                this.NotifyPropertyChanged("AxisZName");
            }
        }

        public string AxisXName { get { return this.GetAxisName(this.axisX); } }

        public string AxisYName { get { return this.GetAxisName(this.axisY); } }

        public string AxisZName { get { return this.GetAxisName(this.axisZ); } }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string GetAxisName(int axis)
        {
            switch (axis)
            {
                case 1:
                    return "+X";

                case 2:
                    return "+Y";

                case 3:
                    return "+Z";

                case -1:
                    return "-X";

                case -2:
                    return "-Y";

                case -3:
                    return "-Z";
            }

            return string.Empty;
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

        private void NegX_Click(object sender, RoutedEventArgs e)
        {
            this.AxisX = -this.AxisX;
        }

        private void NegY_Click(object sender, RoutedEventArgs e)
        {
            this.AxisY = -this.AxisY;
        }

        private void NegZ_Click(object sender, RoutedEventArgs e)
        {
            this.AxisZ = -this.AxisZ;
        }

        private void ExchangeXY_Click(object sender, RoutedEventArgs e)
        {
            int x = this.AxisX;
            int y = this.AxisY;

            this.AxisX = y;
            this.AxisY = x;
        }

        private void ExchangeYZ_Click(object sender, RoutedEventArgs e)
        {
            int y = this.AxisY;
            int z = this.AxisZ;

            this.AxisY = z;
            this.AxisZ = y;
        }

        private void ExchangeXZ_Click(object sender, RoutedEventArgs e)
        {
            int x = this.AxisX;
            int z = this.AxisZ;

            this.AxisX = z;
            this.AxisZ = x;
        }
    }
}
