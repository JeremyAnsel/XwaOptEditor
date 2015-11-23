using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace WavefrontObj
{
    public struct ObjVector2 : IEquatable<ObjVector2>
    {
        private float u;

        private float v;

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "u")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "v")]
        public ObjVector2(float u, float v)
        {
            this.u = u;
            this.v = v;
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U")]
        public float U
        {
            get { return this.u; }
            set { this.u = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public float V
        {
            get { return this.v; }
            set { this.v = value; }
        }

        /// <summary>
        /// Compares two <see cref="ObjVector2"/> objects. The result specifies whether the values of the two objects are equal.
        /// </summary>
        /// <param name="left">The left <see cref="ObjVector2"/> to compare.</param>
        /// <param name="right">The right <see cref="ObjVector2"/> to compare.</param>
        /// <returns><value>true</value> if the values of left and right are equal; otherwise, <value>false</value>.</returns>
        public static bool operator ==(ObjVector2 left, ObjVector2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="ObjVector2"/> objects. The result specifies whether the values of the two objects are unequal.
        /// </summary>
        /// <param name="left">The left <see cref="ObjVector2"/> to compare.</param>
        /// <param name="right">The right <see cref="ObjVector2"/> to compare.</param>
        /// <returns><value>true</value> if the values of left and right differ; otherwise, <value>false</value>.</returns>
        public static bool operator !=(ObjVector2 left, ObjVector2 right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:R} ; {1:R}", this.U, this.V);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><value>true</value> if the specified object is equal to the current object; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ObjVector2))
            {
                return false;
            }

            return this.Equals((ObjVector2)obj);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><value>true</value> if the specified object is equal to the current object; otherwise, <value>false</value>.</returns>
        public bool Equals(ObjVector2 other)
        {
            return this.u == other.u
                && this.v == other.v;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return new
            {
                this.u,
                this.v
            }
            .GetHashCode();
        }
    }
}
