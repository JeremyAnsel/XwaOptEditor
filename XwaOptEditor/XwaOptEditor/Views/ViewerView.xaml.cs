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
using XwaOptEditor.Models;
using XwaOptEditor.ViewModels;

namespace XwaOptEditor.Views
{
    /// <summary>
    /// Logique d'interaction pour ViewerView.xaml
    /// </summary>
    public partial class ViewerView : UserControl
    {
        public ViewerView()
        {
            InitializeComponent();
        }

        private ViewerViewModel ViewModel
        {
            get { return (ViewerViewModel)this.DataContext; }
        }

        public OptModel OptModel
        {
            get { return (OptModel)this.GetValue(ViewerView.OptModelProperty); }
            set { this.SetValue(ViewerView.OptModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OptModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptModelProperty =
            DependencyProperty.Register("OptModel", typeof(OptModel), typeof(ViewerView), new PropertyMetadata(new PropertyChangedCallback(ViewerView.OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var view = (ViewerView)sender;

            switch (e.Property.Name)
            {
                case "OptModel":
                    view.ViewModel.OptModel = (OptModel)e.NewValue;
                    break;
            }
        }
    }
}
