using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XwaOptEditor.Messages;
using XwaOptEditor.Mvvm;

namespace XwaOptEditor.ViewModels
{
    class BusyIndicatorViewModel : ObservableObject
    {
        private bool isBusy;

        private string busyContent;

        public BusyIndicatorViewModel()
        {
            this.IsBusy = false;
            this.BusyContent = "Please Wait...";
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            private set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    this.RaisePropertyChangedEvent("IsBusy");
                }
            }
        }

        public string BusyContent
        {
            get
            {
                return this.busyContent;
            }

            private set
            {
                if (this.busyContent != value)
                {
                    this.busyContent = value;
                    this.RaisePropertyChangedEvent("BusyContent");
                }
            }
        }

        public void Update(BusyIndicatorMessage busy)
        {
            this.IsBusy = busy.IsBusy;
            this.BusyContent = string.IsNullOrEmpty(busy.BusyContent) ? "Please Wait..." : busy.BusyContent;
        }
    }
}
