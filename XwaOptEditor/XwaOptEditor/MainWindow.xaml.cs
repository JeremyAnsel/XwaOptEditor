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
using Microsoft.Win32;
using XwaOptEditor.Messages;
using XwaOptEditor.Mvvm;
using XwaOptEditor.ViewModels;

namespace XwaOptEditor
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += (sender, args) =>
            {
                Messenger.Instance.Register<BusyIndicatorMessage>(this, this.OnBusyIndicatorMessage);
                Messenger.Instance.Register<MessageBoxMessage>(this, this.OnMessageBoxMessage);
                Messenger.Instance.Register<OpenFileDialogMessage>(this, this.OnOpenFileDialogMessage);
                Messenger.Instance.Register<SaveFileDialogMessage>(this, this.OnSaveFileDialogMessage);
                Messenger.Instance.Register<MainViewSelectorMessage>(this, this.OnMainViewSelectorMessage);
                Messenger.Instance.Register<TextureBrowserMessage>(this, this.OnTextureBrowserMessage);
                Messenger.Instance.Register<ScaleFactorMessage>(this, this.OnScaleFactorMessage);
                Messenger.Instance.Register<ChangeAxesMessage>(this, this.OnChangeAxesMessage);
                Messenger.Instance.Register<MoveFactorMessage>(this, this.OnMoveFactorMessage);
                Messenger.Instance.Register<RotateFactorMessage>(this, this.OnRotateFactorMessage);
            };

            this.Unloaded += (sender, args) =>
            {
                Messenger.Instance.Unregister(this);
            };
        }

        private MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)this.DataContext; }
        }

        private void OnBusyIndicatorMessage(BusyIndicatorMessage busy)
        {
            Action action = () =>
            {
                var viewModel = (MainWindowViewModel)this.DataContext;

                viewModel.Busy.Update(busy);
            };

            this.Dispatcher.Invoke(action);
        }

        private void OnMessageBoxMessage(MessageBoxMessage message)
        {
            Action action = () =>
            {
                message.Result = Xceed.Wpf.Toolkit.MessageBox.Show(this, message.Text, message.Caption ?? this.Title, message.Button, message.Icon);
            };

            this.Dispatcher.Invoke(action);
        }

        private void OnOpenFileDialogMessage(OpenFileDialogMessage message)
        {
            Action action = () =>
            {
                var dialog = new OpenFileDialog();
                dialog.CheckFileExists = true;
                dialog.AddExtension = true;
                dialog.DefaultExt = message.DefaultExtension;
                dialog.Filter = message.Filter;
                dialog.FileName = message.FileName;

                if (dialog.ShowDialog(this) == true)
                {
                    message.FileName = dialog.FileName;
                }
                else
                {
                    message.FileName = null;
                }
            };

            this.Dispatcher.Invoke(action);
        }

        private void OnSaveFileDialogMessage(SaveFileDialogMessage message)
        {
            Action action = () =>
            {
                var dialog = new SaveFileDialog();
                dialog.AddExtension = true;
                dialog.DefaultExt = message.DefaultExtension;
                dialog.Filter = message.Filter;
                dialog.FileName = message.FileName;

                if (dialog.ShowDialog(this) == true)
                {
                    message.FileName = dialog.FileName;
                }
                else
                {
                    message.FileName = null;
                }

            };

            this.Dispatcher.Invoke(action);
        }

        private void OnMainViewSelectorMessage(MainViewSelectorMessage message)
        {
            Action action = () =>
            {
                switch (message.View)
                {
                    case "Help":
                        new HelpWindow(this).ShowDialog();
                        break;

                    case "Editor":
                        this.Tabs.SelectedItem = this.EditorTab;
                        break;

                    case "Textures":
                        this.Tabs.SelectedItem = this.TexturesTab;
                        break;

                    case "Viewer":
                        this.Tabs.SelectedItem = this.ViewerTab;
                        break;

                    case "PlayabilityMessages":
                        this.Tabs.SelectedItem = this.PlayabilityMessagesTab;
                        break;
                }
            };

            this.Dispatcher.Invoke(action);
        }

        private void OnTextureBrowserMessage(TextureBrowserMessage message)
        {
            Action action = () =>
            {
                var dialog = new TextureBrowserWindow(this, message.OptFile);

                if (dialog.ShowDialog() == true)
                {
                    message.TextureName = dialog.TextureName;
                }
                else
                {
                    message.TextureName = null;
                }
            };

            this.Dispatcher.Invoke(action);
        }

        private void OnScaleFactorMessage(ScaleFactorMessage message)
        {
            Action action = () =>
            {
                var dialog = new ScaleFactorDialog(this, message.SizeX, message.SizeY, message.SizeZ);

                if (dialog.ShowDialog() == true)
                {
                    message.ScaleX = dialog.ScaleX;
                    message.ScaleY = dialog.ScaleY;
                    message.ScaleZ = dialog.ScaleZ;
                    message.ScaleType = dialog.ScaleType;
                    message.Changed = true;
                }
                else
                {
                    message.Changed = false;
                }
            };

            this.Dispatcher.Invoke(action);
        }

        private void OnChangeAxesMessage(ChangeAxesMessage message)
        {
            Action action = () =>
            {
                var dialog = new ChangeAxesDialog(this);

                if (dialog.ShowDialog() == true)
                {
                    message.AxisX = dialog.AxisX;
                    message.AxisY = dialog.AxisY;
                    message.AxisZ = dialog.AxisZ;
                    message.Changed = true;
                }
                else
                {
                    message.Changed = false;
                }
            };

            this.Dispatcher.Invoke(action);
        }

        private void OnMoveFactorMessage(MoveFactorMessage message)
        {
            Action action = () =>
            {
                var dialog = new MoveFactorDialog(this);

                if (dialog.ShowDialog() == true)
                {
                    message.MoveX = dialog.MoveX;
                    message.MoveY = dialog.MoveY;
                    message.MoveZ = dialog.MoveZ;
                    message.Changed = true;
                }
                else
                {
                    message.Changed = false;
                }
            };

            this.Dispatcher.Invoke(action);
        }

        private void OnRotateFactorMessage(RotateFactorMessage message)
        {
            Action action = () =>
            {
                var dialog = new RotateFactorDialog(this);
                dialog.CenterX = message.CenterX;
                dialog.CenterY = message.CenterY;
                dialog.Angle = message.Angle;

                if (dialog.ShowDialog() == true)
                {
                    message.CenterX = dialog.CenterX;
                    message.CenterY = dialog.CenterY;
                    message.Angle = dialog.Angle;
                    message.Changed = true;
                }
                else
                {
                    message.Changed = false;
                }
            };

            this.Dispatcher.Invoke(action);
        }

        private void PushUndoStackButton_Click(object sender, RoutedEventArgs e)
        {
            Action action = () =>
            {
                this.ViewModel.OptModel.UndoStackPush("no label");
            };

            this.Dispatcher.Invoke(action);
        }

        private void RestoreUndoStackButton_Click(object sender, RoutedEventArgs e)
        {
            Action action = () =>
            {
                this.ViewModel.OptModel.UndoStackRestore(this.UndoStackListBox.SelectedIndex);
            };

            this.Dispatcher.Invoke(action);
        }

        private void ClearUndoStackButton_Click(object sender, RoutedEventArgs e)
        {
            Action action = () =>
            {
                this.ViewModel.OptModel.UndoStack.Clear();
            };

            this.Dispatcher.Invoke(action);
        }
    }
}
