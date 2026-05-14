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

        private Vector rotationAxis;

        private Vector directionAxis;

        private Vector upAxis;

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

        public Vector RotationAxis
        {
            get
            {
                return this.rotationAxis;
            }

            set
            {
                if (value != this.rotationAxis)
                {
                    this.rotationAxis = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(RotationLength));
                    this.OnPropertyChanged(nameof(RotationLengthFactor));
                    this.OnPropertyChanged(nameof(RealAngle));
                    this.OnPropertyChanged(nameof(RealAngleDegree));
                }
            }
        }

        public float RotationLength
        {
            get
            {
                return this.RotationAxis.Length();
            }
        }

        public float RotationLengthFactor
        {
            get
            {
                return this.RotationAxis.LengthFactor();
            }
        }

        public Vector DirectionAxis
        {
            get
            {
                return this.directionAxis;
            }

            set
            {
                if (value != this.directionAxis)
                {
                    this.directionAxis = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(DirectionLength));
                    this.OnPropertyChanged(nameof(DirectionLengthFactor));
                }
            }
        }

        public float DirectionLength
        {
            get
            {
                return this.DirectionAxis.Length();
            }
        }

        public float DirectionLengthFactor
        {
            get
            {
                return this.DirectionAxis.LengthFactor();
            }
        }

        public Vector UpAxis
        {
            get
            {
                return this.upAxis;
            }

            set
            {
                if (value != this.upAxis)
                {
                    this.upAxis = value;
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
                return this.UpAxis.Length();
            }
        }

        public float UpLengthFactor
        {
            get
            {
                return this.UpAxis.LengthFactor();
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
                float lookLength = this.RotationLengthFactor;
                return lookLength == 0 ? this.Angle : (this.Angle * lookLength);
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
