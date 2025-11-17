using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Mvvm
{
    public class SelectableCollection<T> : ObservableRangeCollection<SelectableItem<T>>
    {
        private T selectedItem;

        public event EventHandler SelectedItemChanged;

        public SelectableCollection()
        {
            this.SelectedItems = new ObservableRangeCollection<T>();
            this.CheckedItems = new ObservableRangeCollection<T>();
        }

        public IEnumerable<T> Source
        {
            get
            {
                return this.Select(t => t.Value);
            }
        }

        public ObservableRangeCollection<T> SelectedItems { get; private set; }

        public T SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            private set
            {
                if (!object.Equals(this.selectedItem, value))
                {
                    this.selectedItem = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));
                    this.OnPropertyChanged(new PropertyChangedEventArgs("HasSelectedItems"));

                    var handler = this.SelectedItemChanged;

                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool HasSelectedItems
        {
            get
            {
                return this.SelectedItem != null;
            }
        }

        internal void ChangeSelectedValue(T value, bool isSelected)
        {
            if (isSelected)
            {
                if (!this.SelectedItems.Contains(value))
                {
                    this.SelectedItems.Add(value);
                }
            }
            else
            {
                if (this.SelectedItems.Contains(value))
                {
                    this.SelectedItems.Remove(value);
                }
            }

            this.SelectedItem = this.SelectedItems.FirstOrDefault();
        }

        public ObservableRangeCollection<T> CheckedItems { get; private set; }

        public bool HasCheckedItems
        {
            get
            {
                return this.CheckedItems.Count != 0;
            }
        }

        internal void ChangeCheckedValue(T value, bool isChecked)
        {
            if (isChecked)
            {
                if (!this.CheckedItems.Contains(value))
                {
                    this.CheckedItems.Add(value);
                }
            }
            else
            {
                if (this.CheckedItems.Contains(value))
                {
                    this.CheckedItems.Remove(value);
                }
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs("HasCheckedItems"));
        }

        public void LoadItems(IEnumerable<T> items)
        {
            this.SelectedItem = default;
            this.SelectedItems.Clear();
            this.Clear();

            if (items == null)
            {
                return;
            }

            var selectableItems = new List<SelectableItem<T>>();

            foreach (var item in items)
            {
                selectableItems.Add(new SelectableItem<T>(this, item));
            }

            this.AddRange(selectableItems);
            this.CheckedItems.AddRange(items);

            if (this.Count > 0)
            {
                this.First().IsSelected = true;
            }
        }

        public void ClearSelection()
        {
            foreach (var item in this)
            {
                item.IsSelected = false;
            }
        }

        public void SetSelection(T value)
        {
            this.ClearSelection();
            this.AddToSelection(value);
        }

        public void SetSelection(IEnumerable<T> values)
        {
            this.ClearSelection();
            this.AddToSelection(values);
        }

        public void AddToSelection(T value)
        {
            if (value == null)
            {
                return;
            }

            this.Where(t => object.Equals(t.Value, value))
                .First()
                .IsSelected = true;
        }

        public void AddToSelection(IEnumerable<T> values)
        {
            if (values == null)
            {
                return;
            }

            foreach (var value in values)
            {
                this.AddToSelection(value);
            }
        }
    }
}
