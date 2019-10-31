using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Mvvm
{
    public class StringCollection : ObservableCollection<StringWrapper>
    {
        public IEnumerable<string> Source
        {
            get
            {
                return this.Select(t => t.Value);
            }
        }

        public void LoadItems(IEnumerable<string> items)
        {
            this.Clear();

            if (items == null)
            {
                return;
            }

            foreach (string item in items)
            {
                this.Add(new StringWrapper(item));
            }
        }

        public IList<int> GetSelectedIndexes(IList<StringWrapper> selectedItems)
        {
            var indexes = new List<int>();

            if (selectedItems != null && selectedItems.Count != 0)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (selectedItems.Contains(this[i]))
                    {
                        indexes.Add(i);
                    }
                }
            }

            return indexes;
        }
    }
}
