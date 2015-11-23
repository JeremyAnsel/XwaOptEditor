using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace WavefrontObj
{
    public struct ObjVector3 : IEquatable<ObjVector3>
    {
        private float x;

        private float y;

        private float z;

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "z")]
        public ObjVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
        public float X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
        public float Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Z")]
        public float Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        /// <summary>
        /// Compares two <see cref="ObjVector3"/> objects. The result specifies whether the values of the two objects are equal.
        /// </summary>
        /// <param name="left">The left <see cref="ObjVector3"/> to compare.</param>
        /// <param name="right">The right <see cref="ObjVector3"/> to compare.</param>
        /// <returns><value>true</value> if the values of left and right are equal; otherwise, <value>false</value>.</returns>
        public static bool operator ==(ObjVector3 left, ObjVector3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="ObjVector3"/> objects. The result specifies whether the values of the two objects are unequal.
        /// </summary>
        /// <param name="left">The left <see cref="ObjVector3"/> to compare.</param>
        /// <param name="right">The right <see cref="ObjVector3"/> to compare.</param>
        /// <returns><value>true</value> if the values of left and right differ; otherwise, <value>false</value>.</returns>
        public static bool operator !=(ObjVector3 left, ObjVector3 right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:R} ; {1:R}; {2:R}", this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><value>true</value> if the specified object is equal to the current object; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ObjVector3))
            {
                return false;
            }

            return this.Equals((ObjVector3)obj);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><value>true</value> if the specified object is equal to the current object; otherwise, <value>false</value>.</returns>
        public bool Equals(ObjVector3 other)
        {
            return this.x == other.x
                && this.y == other.y
                && this.z == other.z;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return new
            {
                this.x,
                this.y,
                this.z
            }
            .GetHashCode();
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c", Justification = "Reviewed")]
        public static ObjVector3 Normal(ObjVector3 a, ObjVector3 b, ObjVector3 c)
        {
            ObjVector3 ba = new ObjVector3(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
            ObjVector3 ca = new ObjVector3(c.X - a.X, c.Y - a.Y, c.Z - a.Z);

            ObjVector3 n = new ObjVector3(
                ba.Z * ca.Y - ba.Y * ca.Z,
                ba.X * ca.Z - ba.Z * ca.X,
                ba.Y * ca.X - ba.X * ca.Y);

            float length = (float)Math.Sqrt(n.X * n.X + n.Y * n.Y + n.Z * n.Z);

            if (length > 0.0f)
            {
                n = new ObjVector3(n.X / length, n.Y / length, n.Z / length);
            }

            return n;
        }
    }
}
