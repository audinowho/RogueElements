// <copyright file="Priority.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Represents a hierarchical priority value used to order generation steps in the pipeline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Priority"/> enables fine-grained ordering of <see cref="GenStep{T}"/> instances
    /// within a <see cref="MapGen{T}"/> pipeline. Unlike simple integer priorities, <see cref="Priority"/>
    /// supports multi-level hierarchical ordering (e.g., "3.1.2") for inserting steps between existing ones.
    /// </para>
    /// <para>
    /// Priority comparison follows lexicographic ordering of the integer components:
    /// <list type="bullet">
    /// <item><description><c>Priority(1)</c> comes before <c>Priority(2)</c></description></item>
    /// <item><description><c>Priority(1, 1)</c> comes after <c>Priority(1)</c> but before <c>Priority(2)</c></description></item>
    /// <item><description><c>Priority(1, 0)</c> is equivalent to <c>Priority(1)</c> (trailing zeros are normalized)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The hierarchical structure allows inserting new steps between existing priorities without
    /// renumbering. For example, to add a step between priorities 3 and 4, use <c>Priority(3, 1)</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Simple integer priorities
    /// var early = new Priority(1);
    /// var middle = new Priority(5);
    /// var late = new Priority(10);
    ///
    /// // Hierarchical priorities for fine-grained ordering
    /// var afterMiddle = new Priority(5, 1);      // Between 5 and 6
    /// var wayAfterMiddle = new Priority(5, 2);   // Between 5.1 and 6
    ///
    /// // Extending an existing priority
    /// var subStep = new Priority(middle, 1);     // Creates Priority(5, 1)
    ///
    /// // Use with MapGen
    /// layout.GenSteps.Add(early, new InitTilesStep());
    /// layout.GenSteps.Add(middle, new PlaceRoomsStep());
    /// layout.GenSteps.Add(afterMiddle, new AddDoorsStep());
    /// layout.GenSteps.Add(late, new FinalizeStep());
    /// </code>
    /// </example>
    /// <seealso cref="PriorityList{T}"/>
    /// <seealso cref="MapGen{T}"/>
    [Serializable]
    public struct Priority : IComparable<Priority>, IEquatable<Priority>
    {
        /// <summary>
        /// Represents an invalid priority that precedes all valid priorities in comparison.
        /// </summary>
        /// <remarks>
        /// Invalid priorities have a null internal array. When compared, invalid priorities
        /// precede all valid priorities and are equal to each other. Use this to represent
        /// uninitialized or undefined priority states.
        /// </remarks>
        public static Priority Invalid = new Priority(null);

        /// <summary>
        /// Represents a priority of zero, equivalent to <c>new Priority(0)</c>.
        /// </summary>
        public static Priority Zero = new Priority(0);

        private readonly int[] str;

        /// <summary>
        /// Initializes a new instance of the <see cref="Priority"/> struct with the specified priority levels.
        /// </summary>
        /// <param name="vals">
        /// The integer components of the priority, from most significant to least significant.
        /// Trailing zeros are automatically normalized (e.g., [1, 0, 0] becomes [1]).
        /// Pass <see langword="null"/> or an empty array to create an invalid priority.
        /// </param>
        /// <example>
        /// <code>
        /// var p1 = new Priority(5);           // Priority "5"
        /// var p2 = new Priority(5, 1);        // Priority "5.1"
        /// var p3 = new Priority(5, 1, 2);     // Priority "5.1.2"
        /// var p4 = new Priority(5, 0);        // Normalized to "5"
        /// </code>
        /// </example>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Priority"/> struct by extending an existing priority.
        /// </summary>
        /// <param name="other">The base priority to extend.</param>
        /// <param name="vals">
        /// Additional priority levels to append after the base priority.
        /// Trailing zeros in the combined result are normalized.
        /// </param>
        /// <remarks>
        /// This constructor is useful for creating sub-priorities that logically follow a parent priority.
        /// For example, <c>new Priority(existingPriority, 1)</c> creates a priority that comes after
        /// <c>existingPriority</c> but before any higher integer priority.
        /// </remarks>
        /// <example>
        /// <code>
        /// var basePriority = new Priority(5);
        /// var extended = new Priority(basePriority, 1, 2);  // Creates Priority(5, 1, 2)
        /// </code>
        /// </example>
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

        /// <summary>
        /// Gets the number of priority levels in this instance.
        /// </summary>
        /// <value>
        /// The count of integer components in this priority. Returns 0 for invalid priorities.
        /// </value>
        public int Length
        {
            get { return this.str == null ? 0 : this.str.Length; }
        }

        /// <summary>
        /// Gets the priority component at the specified level index.
        /// </summary>
        /// <param name="ii">The zero-based index of the priority level to retrieve.</param>
        /// <returns>The integer value at the specified priority level.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when <paramref name="ii"/> is less than 0 or greater than or equal to <see cref="Length"/>.
        /// </exception>
        public int this[int ii]
        {
            get { return this.str[ii]; }
        }

        /// <summary>
        /// Determines whether two <see cref="Priority"/> instances are equal.
        /// </summary>
        /// <param name="value1">The first priority to compare.</param>
        /// <param name="value2">The second priority to compare.</param>
        /// <returns><see langword="true"/> if the priorities are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(Priority value1, Priority value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Determines whether two <see cref="Priority"/> instances are not equal.
        /// </summary>
        /// <param name="value1">The first priority to compare.</param>
        /// <param name="value2">The second priority to compare.</param>
        /// <returns><see langword="true"/> if the priorities are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(Priority value1, Priority value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Determines whether one <see cref="Priority"/> is greater than another.
        /// </summary>
        /// <param name="value1">The first priority to compare.</param>
        /// <param name="value2">The second priority to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="value1"/> is greater than <paramref name="value2"/>;
        /// otherwise, <see langword="false"/>. Returns <see langword="false"/> if either priority is invalid.
        /// </returns>
        public static bool operator >(Priority value1, Priority value2)
        {
            // Special case for invalid.  It is not greater or less than anything.
            if (value1.Length == 0 || value2.Length == 0)
                return false;
            return value1.CompareTo(value2) > 0;
        }

        /// <summary>
        /// Determines whether one <see cref="Priority"/> is less than another.
        /// </summary>
        /// <param name="value1">The first priority to compare.</param>
        /// <param name="value2">The second priority to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="value1"/> is less than <paramref name="value2"/>;
        /// otherwise, <see langword="false"/>. Returns <see langword="false"/> if either priority is invalid.
        /// </returns>
        public static bool operator <(Priority value1, Priority value2)
        {
            // Special case for invalid.  It is not greater or less than anything.
            if (value1.Length == 0 || value2.Length == 0)
                return false;
            return value1.CompareTo(value2) < 0;
        }

        /// <summary>
        /// Determines whether one <see cref="Priority"/> is greater than or equal to another.
        /// </summary>
        /// <param name="value1">The first priority to compare.</param>
        /// <param name="value2">The second priority to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="value1"/> is greater than or equal to <paramref name="value2"/>;
        /// otherwise, <see langword="false"/>. Returns <see langword="false"/> if either priority is invalid.
        /// </returns>
        public static bool operator >=(Priority value1, Priority value2)
        {
            // Special case for invalid.  It is not greater or less than anything.
            if (value1.Length == 0 || value2.Length == 0)
                return false;
            return value1.CompareTo(value2) >= 0;
        }

        /// <summary>
        /// Determines whether one <see cref="Priority"/> is less than or equal to another.
        /// </summary>
        /// <param name="value1">The first priority to compare.</param>
        /// <param name="value2">The second priority to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="value1"/> is less than or equal to <paramref name="value2"/>;
        /// otherwise, <see langword="false"/>. Returns <see langword="false"/> if either priority is invalid.
        /// </returns>
        public static bool operator <=(Priority value1, Priority value2)
        {
            // Special case for invalid.  It is not greater or less than anything.
            if (value1.Length == 0 || value2.Length == 0)
                return false;
            return value1.CompareTo(value2) <= 0;
        }

        /// <summary>
        /// Returns a string representation of this priority.
        /// </summary>
        /// <returns>
        /// A dot-separated string of the priority levels (e.g., "5.1.2"), or "NULL" for invalid priorities.
        /// </returns>
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

        /// <summary>
        /// Compares this priority to another and returns their relative ordering.
        /// </summary>
        /// <param name="other">The priority to compare against.</param>
        /// <returns>
        /// A negative value if this priority comes before <paramref name="other"/>;
        /// zero if they are equal;
        /// a positive value if this priority comes after <paramref name="other"/>.
        /// Invalid priorities compare as preceding all valid priorities.
        /// </returns>
        /// <remarks>
        /// Comparison is performed lexicographically across priority levels.
        /// Missing levels are treated as zero (e.g., Priority(5) equals Priority(5, 0)).
        /// </remarks>
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

        /// <summary>
        /// Determines whether this priority is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this priority.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is a <see cref="Priority"/>
        /// and equals this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Priority))
                return false;

            return this.Equals((Priority)obj);
        }

        /// <summary>
        /// Determines whether this priority is equal to another priority.
        /// </summary>
        /// <param name="other">The priority to compare with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if the priorities have the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(Priority other)
        {
            return this.CompareTo(other) == 0;
        }

        /// <summary>
        /// Returns a hash code for this priority.
        /// </summary>
        /// <returns>
        /// A hash code computed from the priority levels, or 0 for invalid priorities.
        /// </returns>
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
