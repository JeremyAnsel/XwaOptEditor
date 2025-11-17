using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Mvvm
{
    public class SelectableItem<T> : ObservableObject
    {
        private SelectableCollection<T> collection;

        private bool isSelected;

        private bool isChecked;

        public SelectableItem(SelectableCollection<T> collection, T value)
        {
            this.collection = collection;
            this.Value = value;

            this.isChecked = true;
            //this.collection.CheckedItems.Add(value);
        }

        public T Value { get; private set; }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.RaisePropertyChangedEvent("IsSelected");

                    this.collection.ChangeSelectedValue(this.Value, this.IsSelected);
                }
            }
        }

        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.RaisePropertyChangedEvent("IsChecked");

                    this.collection.ChangeCheckedValue(this.Value, this.IsChecked);
                }
            }
        }
    }
}
