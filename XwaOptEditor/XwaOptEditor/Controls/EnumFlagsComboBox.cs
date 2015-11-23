using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XwaOptEditor.Controls
{
    /// <summary>
    /// Suivez les étapes 1a ou 1b puis 2 pour utiliser ce contrôle personnalisé dans un fichier XAML.
    ///
    /// Étape 1a) Utilisation de ce contrôle personnalisé dans un fichier XAML qui existe dans le projet actif.
    /// Ajoutez cet attribut XmlNamespace à l'élément racine du fichier de balisage où il doit 
    /// être utilisé :
    ///
    ///     xmlns:MyNamespace="clr-namespace:XwaOptEditor"
    ///
    ///
    /// Étape 1b) Utilisation de ce contrôle personnalisé dans un fichier XAML qui existe dans un autre projet.
    /// Ajoutez cet attribut XmlNamespace à l'élément racine du fichier de balisage où il doit 
    /// être utilisé :
    ///
    ///     xmlns:MyNamespace="clr-namespace:XwaOptEditor;assembly=XwaOptEditor"
    ///
    /// Vous devrez également ajouter une référence du projet contenant le fichier XAML
    /// à ce projet et régénérer pour éviter des erreurs de compilation :
    ///
    ///     Cliquez avec le bouton droit sur le projet cible dans l'Explorateur de solutions, puis sur
    ///     "Ajouter une référence"->"Projets"->[Recherchez et sélectionnez ce projet]
    ///
    ///
    /// Étape 2)
    /// Utilisez à présent votre contrôle dans le fichier XAML.
    ///
    ///     <MyNamespace:EnumFlagsComboBox/>
    ///
    /// </summary>
    public class EnumFlagsComboBox : ComboBox
    {
        private Type enumType;

        static EnumFlagsComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumFlagsComboBox), new FrameworkPropertyMetadata(typeof(EnumFlagsComboBox)));
        }

        public EnumFlagsComboBox()
            : base()
        {
            this.IsEditable = true;
            this.IsReadOnly = true;
            this.AddHandler(TextBox.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(this.Button_Click));
            this.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.Button_Click));
            this.DropDownOpened += this.EnumFlagsComboBox_DropDownOpened;
            this.AddHandler(CheckBox.ClickEvent, new RoutedEventHandler(this.ItemClick));
        }

        public Type EnumType
        {
            get { return this.enumType; }

            set
            {
                this.enumType = value;

                if (this.enumType != null)
                {
                    this.ItemsSource = from object v in Enum.GetValues(this.enumType)
                                       where (int)v != 0
                                       select new CheckBox()
                                       {
                                           Content = v
                                       };
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = true;
        }

        private void EnumFlagsComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (this.EnumType != null)
            {
                if (!string.IsNullOrEmpty(this.Text))
                {
                    int target = (int)Enum.Parse(this.EnumType, this.Text);

                    foreach (CheckBox item in this.Items)
                    {
                        int c = (int)item.Content;
                        item.IsChecked = ((target & c) == c);
                    }
                }
            }
        }

        private void ItemClick(object sender, RoutedEventArgs e)
        {
            if (this.EnumType != null)
            {
                int target = 0;

                foreach (CheckBox item in this.Items)
                {
                    if (item.IsChecked.GetValueOrDefault())
                    {
                        target |= (int)item.Content;
                    }
                }

                this.Text = Enum.ToObject(this.EnumType, target).ToString();
            }
        }
    }
}
