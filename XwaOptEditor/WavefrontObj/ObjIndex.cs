using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace WavefrontObj
{
    public struct ObjIndex : IEquatable<ObjIndex>
    {
        private int a;

        private int b;

        private int c;

        private int d;

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        public ObjIndex(int a, int b, int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = -1;
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d")]
        public ObjIndex(int a, int b, int c, int d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A")]
        public int A
        {
            get { return this.a; }
            set { this.a = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "B")]
        public int B
        {
            get { return this.b; }
            set { this.b = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "C")]
        public int C
        {
            get { return this.c; }
            set { this.c = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "D")]
        public int D
        {
            get { return this.d; }
            set { this.d = value; }
        }

        /// <summary>
        /// Compares two <see cref="ObjIndex"/> objects. The result specifies whether the values of the two objects are equal.
        /// </summary>
        /// <param name="left">The left <see cref="ObjIndex"/> to compare.</param>
        /// <param name="right">The right <see cref="ObjIndex"/> to compare.</param>
        /// <returns><value>true</value> if the values of left and right are equal; otherwise, <value>false</value>.</returns>
        public static bool operator ==(ObjIndex left, ObjIndex right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="ObjIndex"/> objects. The result specifies whether the values of the two objects are unequal.
        /// </summary>
        /// <param name="left">The left <see cref="ObjIndex"/> to compare.</param>
        /// <param name="right">The right <see cref="ObjIndex"/> to compare.</param>
        /// <returns><value>true</value> if the values of left and right differ; otherwise, <value>false</value>.</returns>
        public static bool operator !=(ObjIndex left, ObjIndex right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ; {1} ; {2}; {3}", this.A, this.B, this.C, this.D);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><value>true</value> if the specified object is equal to the current object; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ObjIndex))
            {
                return false;
            }

            return this.Equals((ObjIndex)obj);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><value>true</value> if the specified object is equal to the current object; otherwise, <value>false</value>.</returns>
        public bool Equals(ObjIndex other)
        {
            return this.a == other.a
                && this.b == other.b
                && this.c == other.c
                && this.d == other.d;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return new
            {
                this.a,
                this.b,
                this.c,
                this.d
            }
            .GetHashCode();
        }
    }
}
