using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JeremyAnsel.Xwa.Opt;
using JeremyAnsel.Xwa.WpfOpt;
using XwaOptEditor.Mvvm;

namespace XwaOptEditor.Models
{
    public class OptModel : ObservableObject
    {
        private OptFile file;

        private OptCache cache;

        private bool isPlayable;

        public OptModel()
        {
            this.PlayabilityMessages = new ObservableCollection<PlayabilityMessage>();

            this.file = new OptFile();
        }

        public OptFile File
        {
            get
            {
                return this.file;
            }

            set
            {
                this.file = null;
                this.cache = null;
                this.RaisePropertyChangedEvent("File");

                this.file = value;
                this.RaisePropertyChangedEvent("File");

                this.Cache = new OptCache(this.file);

                this.CheckPlayability();
            }
        }

        public OptCache Cache
        {
            get
            {
                return this.cache;
            }

            private set
            {
                this.cache = value;
                this.RaisePropertyChangedEvent("Cache");
            }
        }

        public bool IsPlayable
        {
            get
            {
                return this.isPlayable;
            }

            set
            {
                if (this.isPlayable != value)
                {
                    this.isPlayable = value;
                    this.RaisePropertyChangedEvent("IsPlayable");
                }
            }
        }

        public ObservableCollection<PlayabilityMessage> PlayabilityMessages { get; private set; }

        private void CheckPlayability()
        {
            this.PlayabilityMessages.Clear();
            this.IsPlayable = true;

            if (this.File == null)
            {
                return;
            }

            var messages = this.File.CheckPlayability();

            this.IsPlayable = !messages.Any(t => t.Level == PlayabilityMessageLevel.Error);

            foreach (var message in messages)
            {
                this.PlayabilityMessages.Add(message);
            }
        }
    }
}
