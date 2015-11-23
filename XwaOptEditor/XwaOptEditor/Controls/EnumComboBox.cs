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
    ///     <MyNamespace:EnumComboBox/>
    ///
    /// </summary>
    public class EnumComboBox : ComboBox
    {
        private Type enumType;

        static EnumComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumComboBox), new FrameworkPropertyMetadata(typeof(EnumComboBox)));
        }

        public EnumComboBox()
            : base()
        {
            this.IsEditable = true;
            this.IsReadOnly = true;
            this.AddHandler(TextBox.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(this.Button_Click));
            this.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.Button_Click));
        }

        public Type EnumType
        {
            get { return this.enumType; }

            set
            {
                this.enumType = value;

                if (this.enumType != null)
                {
                    this.ItemsSource = Enum.GetValues(this.enumType);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = true;
        }
    }
}
