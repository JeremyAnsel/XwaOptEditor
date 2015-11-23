using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Mvvm
{
    public class SelectableItem<T> : ObservableObject where T : class
    {
        private SelectableCollection<T> collection;

        private bool isSelected;

        public SelectableItem(SelectableCollection<T> collection, T value)
        {
            this.collection = collection;
            this.Value = value;
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
    }
}
