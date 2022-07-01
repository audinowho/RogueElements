// <copyright file="Priority.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements
{
    [Serializable]
    public struct Priority : IComparable<Priority>, IEquatable<Priority>
    {
        public static Priority Invalid = new Priority(null);

        public static Priority Zero = new Priority(0);

        private readonly int[] str;

        public Priority(params int[] vals)
        {
            if (vals == null || vals.Length == 0)
            {
                this.str = null;
            }
            else
            {
                int lastIdx = vals.Length - 1;
                while (vals[lastIdx] == 0 && lastIdx > 0)
                    lastIdx--;
                this.str = new int[lastIdx + 1];
                Array.Copy(vals, 0, this.str, 0, lastIdx + 1);
            }
        }

        public Priority(Priority other, params int[] vals)
        {
            if (vals == null || vals.Length == 0)
            {
                this.str = other.str;
            }
            else
            {
                int lastIdx = vals.Length - 1;
                while (vals[lastIdx] == 0 && lastIdx > 0)
                    lastIdx--;
                this.str = new int[other.Length + lastIdx + 1];
                Array.Copy(other.str, 0, this.str, 0, other.str.Length);
                Array.Copy(vals, 0, this.str, other.Length, lastIdx + 1);
            }
        }

        public int Length
        {
            get { return this.str == null ? 0 : this.str.Length; }
        }

        public int this[int ii]
        {
            get { return this.str[ii]; }
        }

        public static bool operator ==(Priority value1, Priority value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(Priority value1, Priority value2)
        {
            return !(value1 == value2);
        }

        public static bool operator >(Priority value1, Priority value2)
        {
            // Special case for invalid.  It is not greater or less than anything.
            if (value1.Length == 0 || value2.Length == 0)
                return false;
            return value1.CompareTo(value2) > 0;
        }

        public static bool operator <(Priority value1, Priority value2)
        {
            // Special case for invalid.  It is not greater or less than anything.
            if (value1.Length == 0 || value2.Length == 0)
                return false;
            return value1.CompareTo(value2) < 0;
        }

        public static bool operator >=(Priority value1, Priority value2)
        {
            // Special case for invalid.  It is not greater or less than anything.
            if (value1.Length == 0 || value2.Length == 0)
                return false;
            return value1.CompareTo(value2) >= 0;
        }

        public static bool operator <=(Priority value1, Priority value2)
        {
            // Special case for invalid.  It is not greater or less than anything.
            if (value1.Length == 0 || value2.Length == 0)
                return false;
            return value1.CompareTo(value2) <= 0;
        }

        public override string ToString()
        {
            if (this.str == null)
                return "NULL";

            StringBuilder s = new StringBuilder();
            for (int ii = 0; ii < this.str.Length; ii++)
            {
                if (ii > 0)
                    s.Append(".");
                s.Append(this.str[ii].ToString());
            }

            return s.ToString();
        }

        public int CompareTo(Priority other)
        {
            // Invalid precedes everything else
            if (this.str == null)
                return (other.str == null) ? 0 : -1;
            else if (other.str == null)
                return 1;

            int ii = 0;
            while (true)
            {
                if (ii >= this.str.Length && ii >= other.str.Length)
                    return 0;

                int thisDigit = 0;
                int otherDigit = 0;

                if (ii < this.str.Length)
                    thisDigit = this.str[ii];
                if (ii < other.str.Length)
                    otherDigit = other.str[ii];

                if (thisDigit < otherDigit)
                    return -1;
                else if (thisDigit > otherDigit)
                    return 1;
                else
                    ii++;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Priority))
                return false;

            return this.Equals((Priority)obj);
        }

        public bool Equals(Priority other)
        {
            return this.CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (this.str == null)
                return hash;

            foreach (int member in this.str)
                hash ^= member.GetHashCode();

            return hash;
        }
    }
}
