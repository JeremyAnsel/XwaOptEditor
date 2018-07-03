using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XwaOptEditor.Models;
using XwaOptEditor.Mvvm;

namespace XwaOptEditor.ViewModels
{
    class ViewerViewModel : ObservableObject
    {
        private OptModel optModel;

        private int modelVersion = 0;

        private bool modelShowSolid = true;

        private bool modelShowWireframe = false;

        private float modelDistance = 0.001f;

        public ViewerViewModel()
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

        public int ModelVersion
        {
            get
            {
                return this.modelVersion;
            }

            set
            {
                int max = this.OptModel.File.MaxTextureVersion;

                if (value < 0)
                {
                    value = 0;
                }
                else if (value > max)
                {
                    value = max;
                }

                if (this.modelVersion != value)
                {
                    this.modelVersion = value;
                    this.RaisePropertyChangedEvent("ModelVersion");
                }
            }
        }

        public bool ModelShowSolid
        {
            get
            {
                return this.modelShowSolid;
            }

            set
            {
                if (this.modelShowSolid != value)
                {
                    this.modelShowSolid = value;
                    this.RaisePropertyChangedEvent("ModelShowSolid");
                }
            }
        }

        public bool ModelShowWireframe
        {
            get
            {
                return this.modelShowWireframe;
            }

            set
            {
                if (this.modelShowWireframe != value)
                {
                    this.modelShowWireframe = value;
                    this.RaisePropertyChangedEvent("ModelShowWireframe");
                }
            }
        }

        public float ModelDistance
        {
            get
            {
                return this.modelDistance;
            }

            set
            {
                if (this.modelDistance != value)
                {
                    this.modelDistance = value;
                    this.RaisePropertyChangedEvent("ModelDistance");
                }
            }
        }
    }
}
