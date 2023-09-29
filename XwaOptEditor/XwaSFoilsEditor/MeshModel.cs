using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XwaSFoilsEditor
{
    public sealed class MeshModel : INotifyPropertyChanged
    {
        private bool isVisible = true;

        private string name;

        private int meshIndex;

        private Vector pivot;

        private Vector look;

        private Vector up;

        private Vector right;

        private int angle;

        private int closingSpeed;

        private int openingSpeed;

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if (value != this.isVisible)
                {
                    this.isVisible = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int MeshIndex
        {
            get
            {
                return this.meshIndex;
            }

            set
            {
                if (value != this.meshIndex)
                {
                    this.meshIndex = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Vector Pivot
        {
            get
            {
                return this.pivot;
            }

            set
            {
                if (value != this.pivot)
                {
                    this.pivot = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Vector Look
        {
            get
            {
                return this.look;
            }

            set
            {
                if (value != this.look)
                {
                    this.look = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(LookLength));
                    this.OnPropertyChanged(nameof(LookLengthFactor));
                    this.OnPropertyChanged(nameof(RealAngle));
                    this.OnPropertyChanged(nameof(RealAngleDegree));
                }
            }
        }

        public float LookLength
        {
            get
            {
                return this.Look.Length();
            }
        }

        public float LookLengthFactor
        {
            get
            {
                return this.Look.LengthFactor();
            }
        }

        public Vector Up
        {
            get
            {
                return this.up;
            }

            set
            {
                if (value != this.up)
                {
                    this.up = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(UpLength));
                    this.OnPropertyChanged(nameof(UpLengthFactor));
                }
            }
        }

        public float UpLength
        {
            get
            {
                return this.Up.Length();
            }
        }

        public float UpLengthFactor
        {
            get
            {
                return this.Up.LengthFactor();
            }
        }

        public Vector Right
        {
            get
            {
                return this.right;
            }

            set
            {
                if (value != this.right)
                {
                    this.right = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(RightLength));
                    this.OnPropertyChanged(nameof(RightLengthFactor));
                }
            }
        }

        public float RightLength
        {
            get
            {
                return this.Right.Length();
            }
        }

        public float RightLengthFactor
        {
            get
            {
                return this.Right.LengthFactor();
            }
        }

        public int Angle
        {
            get
            {
                return this.angle;
            }

            set
            {
                if (value != this.angle)
                {
                    this.angle = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(RealAngle));
                    this.OnPropertyChanged(nameof(RealAngleDegree));
                }
            }
        }

        public float RealAngle
        {
            get
            {
                return this.Angle * this.LookLengthFactor;
            }
        }

        public float RealAngleDegree
        {
            get
            {
                return this.RealAngle * 360.0f / 255;
            }
        }

        public int ClosingSpeed
        {
            get
            {
                return this.closingSpeed;
            }

            set
            {
                if (value != this.closingSpeed)
                {
                    this.closingSpeed = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int OpeningSpeed
        {
            get
            {
                return this.openingSpeed;
            }

            set
            {
                if (value != this.openingSpeed)
                {
                    this.openingSpeed = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return this.MeshIndex + "-" + this.Name;
        }
    }
}
