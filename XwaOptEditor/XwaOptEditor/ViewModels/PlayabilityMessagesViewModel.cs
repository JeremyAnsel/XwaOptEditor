using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XwaOptEditor.Models;
using XwaOptEditor.Mvvm;

namespace XwaOptEditor.ViewModels
{
    class PlayabilityMessagesViewModel : ObservableObject
    {
        private OptModel optModel;

        public PlayabilityMessagesViewModel()
        {
        }

        public OptModel OptModel
        {
            get
            {
                return this.optModel;
            }

            set
            {
                if (this.optModel != value)
                {
                    this.optModel = value;
                    this.RaisePropertyChangedEvent("OptModel");
                }
            }
        }
    }
}
