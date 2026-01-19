// <copyright file="Loc.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Represents a 2D integer coordinate or vector.
    /// </summary>
    [Serializable]
    public struct Loc : IEquatable<Loc>
    {
        /// <summary>
        /// A location with both X and Y set to 0.
        /// </summary>
        public static readonly Loc Zero = new Loc(0, 0);

        /// <summary>
        /// A location with both X and Y set to 1.
        /// </summary>
        public static readonly Loc One = new Loc(1, 1);

        /// <summary>
        /// A unit vector pointing in the positive X direction (1, 0).
        /// </summary>
        public static readonly Loc UnitX = new Loc(1, 0);

        /// <summary>
        /// A unit vector pointing in the positive Y direction (0, 1).
        /// </summary>
        public static readonly Loc UnitY = new Loc(0, 1);

        /// <summary>
        /// The X component of this location.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of this location.
        /// </summary>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Loc"/> struct with both X and Y set to the same value.
        /// </summary>
        /// <param name="n">The value for both X and Y.</param>
        public Loc(int n)
        {
            this.X = n;
            this.Y = n;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loc"/> struct with specified X and Y values.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        public Loc(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loc"/> struct by copying another location.
        /// </summary>
        /// <param name="loc">The location to copy.</param>
        public Loc(Loc loc)
        {
            this.X = loc.X;
            this.Y = loc.Y;
        }

        public static bool operator ==(Loc value1, Loc value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(Loc value1, Loc value2)
        {
            return !(value1 == value2);
        }

        public static Loc operator -(Loc value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }

        public static Loc operator +(Loc value1, Loc value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static Loc operator -(Loc value1, Loc value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static Loc operator *(Loc value1, Loc value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static Loc operator *(Loc value1, int scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            return value1;
        }

        public static Loc operator *(int scaleFactor, Loc value1)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            return value1;
        }

        public static Loc operator /(Loc value1, Loc value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        public static Loc operator /(Loc value1, int scaleFactor)
        {
            value1.X /= scaleFactor;
            value1.Y /= scaleFactor;
            return value1;
        }

        public static Loc operator %(Loc value1, Loc value2)
        {
            value1.X %= value2.X;
            value1.Y %= value2.Y;
            return value1;
        }

        public static Loc operator %(Loc value1, int modFactor)
        {
            value1.X %= modFactor;
            value1.Y %= modFactor;
            return value1;
        }

        /// <summary>
        /// Computes the dot product of two locations.
        /// </summary>
        /// <param name="value1">The first location.</param>
        /// <param name="value2">The second location.</param>
        /// <returns>The dot product of the two locations.</returns>
        public static int Dot(Loc value1, Loc value2)
        {
            return (value1.X * value2.X) + (value1.Y * value2.Y);
        }

        /// <summary>
        /// Returns a location with the minimum X and Y components from two locations.
        /// </summary>
        /// <param name="value1">The first location.</param>
        /// <param name="value2">The second location.</param>
        /// <returns>A location with the component-wise minimum values.</returns>
        public static Loc Min(Loc value1, Loc value2)
        {
            return new Loc(
                value1.X < value2.X ? value1.X : value2.X,
                value1.Y < value2.Y ? value1.Y : value2.Y);
        }

        /// <summary>
        /// Returns a location with the maximum X and Y components from two locations.
        /// </summary>
        /// <param name="value1">The first location.</param>
        /// <param name="value2">The second location.</param>
        /// <returns>A location with the component-wise maximum values.</returns>
        public static Loc Max(Loc value1, Loc value2)
        {
            return new Loc(
                value1.X > value2.X ? value1.X : value2.X,
                value1.Y > value2.Y ? value1.Y : value2.Y);
        }

        /// <summary>
        /// Wraps a location within the specified size boundaries.
        /// </summary>
        /// <param name="value">The location to wrap.</param>
        /// <param name="size">The size boundaries for wrapping.</param>
        /// <returns>The wrapped location.</returns>
        public static Loc Wrap(Loc value, Loc size)
        {
            return ((value % size) + size) % size;
        }

        /// <summary>
        /// Gets the square of the total distance of the loc from (0,0), in Euclidean distance.
        /// </summary>
        /// <returns></returns>
        public int DistSquared()
        {
            return (this.X * this.X) + (this.Y * this.Y);
        }

        /// <summary>
        /// Returns the vector length in integer units.
        /// </summary>
        /// <returns></returns>
        public int Length()
        {
            return (int)Math.Abs(Math.Round(Math.Sqrt(this.DistSquared())));
        }

        /// <summary>
        /// Gets the total distance of the loc from (0,0), in 8-Directional (Chess) distance.
        /// </summary>
        /// <returns></returns>
        public int Dist8()
        {
            return Math.Max(Math.Abs(this.X), Math.Abs(this.Y));
        }

        /// <summary>
        /// Gets the total distance of the loc from (0,0), in 4-Directional (Manhattan) distance.
        /// </summary>
        /// <returns></returns>
        public int Dist4()
        {
            return Math.Abs(this.X) + Math.Abs(this.Y);
        }

        /// <summary>
        /// Returns the transposed coordinates.
        /// </summary>
        /// <returns></returns>
        public Loc Transpose()
        {
            return new Loc(this.Y, this.X);
        }

        /// <summary>
        /// Returns a string representation of this location.
        /// </summary>
        /// <returns>A string in the format "X:{X} Y:{Y}".</returns>
        public override string ToString()
        {
            return $"X:{this.X} Y:{this.Y}";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return (obj is Loc) && this.Equals((Loc)obj);
        }

        /// <inheritdoc/>
        public bool Equals(Loc other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        /// <summary>
        /// Returns the negation of this location.
        /// </summary>
        /// <returns>A location with negated X and Y components.</returns>
        public Loc Negate() => -this;

        /// <summary>
        /// Adds another location to this one.
        /// </summary>
        /// <param name="other">The location to add.</param>
        /// <returns>The sum of the two locations.</returns>
        public Loc Add(Loc other) => this + other;

        /// <summary>
        /// Subtracts another location from this one.
        /// </summary>
        /// <param name="other">The location to subtract.</param>
        /// <returns>The difference of the two locations.</returns>
        public Loc Subtract(Loc other) => this - other;

        /// <summary>
        /// Multiplies this location by another location component-wise.
        /// </summary>
        /// <param name="other">The location to multiply by.</param>
        /// <returns>The component-wise product.</returns>
        public Loc Multiply(Loc other) => this * other;

        /// <summary>
        /// Multiplies this location by a scalar.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>The scaled location.</returns>
        public Loc Multiply(int scaleFactor) => this * scaleFactor;

        /// <summary>
        /// Divides this location by another location component-wise.
        /// </summary>
        /// <param name="other">The location to divide by.</param>
        /// <returns>The component-wise quotient.</returns>
        public Loc Divide(Loc other) => this / other;

        /// <summary>
        /// Divides this location by a scalar.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>The scaled location.</returns>
        public Loc Divide(int scaleFactor) => this / scaleFactor;

        /// <summary>
        /// Computes the component-wise modulus with another location.
        /// </summary>
        /// <param name="other">The location to mod by.</param>
        /// <returns>The component-wise modulus.</returns>
        public Loc Mod(Loc other) => this % other;

        /// <summary>
        /// Computes the modulus of both components with a scalar.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>The modulus result.</returns>
        public Loc Mod(int scaleFactor) => this % scaleFactor;
    }
}
