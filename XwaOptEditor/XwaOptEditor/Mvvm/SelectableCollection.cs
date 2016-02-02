using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Mvvm
{
    public class SelectableCollection<T> : ObservableCollection<SelectableItem<T>> where T : class
    {
        private T selectedItem;

        public event EventHandler SelectedItemChanged;

        public SelectableCollection()
        {
            this.SelectedItems = new ObservableCollection<T>();
        }

        public IEnumerable<T> Source
        {
            get
            {
                return this.Select(t => t.Value);
            }
        }

        public ObservableCollection<T> SelectedItems { get; private set; }

        public T SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            private set
            {
                if (this.selectedItem != value)
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

        public void LoadItems(IEnumerable<T> items)
        {
            this.SelectedItem = null;
            this.SelectedItems.Clear();
            this.Clear();

            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                this.Add(new SelectableItem<T>(this, item));
            }

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

            this.Where(t => t.Value == value)
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
