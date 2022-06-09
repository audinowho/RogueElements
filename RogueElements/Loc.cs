// <copyright file="Loc.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    [Serializable]
    public struct Loc : IEquatable<Loc>
    {
        public static readonly Loc Zero = new Loc(0, 0);
        public static readonly Loc One = new Loc(1, 1);
        public static readonly Loc UnitX = new Loc(1, 0);
        public static readonly Loc UnitY = new Loc(0, 1);

        public int X;
        public int Y;

        public Loc(int n)
        {
            this.X = n;
            this.Y = n;
        }

        public Loc(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

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

        public static int Dot(Loc value1, Loc value2)
        {
            return (value1.X * value2.X) + (value1.Y * value2.Y);
        }

        public static Loc Min(Loc value1, Loc value2)
        {
            return new Loc(
                value1.X < value2.X ? value1.X : value2.X,
                value1.Y < value2.Y ? value1.Y : value2.Y);
        }

        public static Loc Max(Loc value1, Loc value2)
        {
            return new Loc(
                value1.X > value2.X ? value1.X : value2.X,
                value1.Y > value2.Y ? value1.Y : value2.Y);
        }

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

        public override string ToString()
        {
            return $"X:{this.X} Y:{this.Y}";
        }

        public override bool Equals(object obj)
        {
            return (obj is Loc) && this.Equals((Loc)obj);
        }

        public bool Equals(Loc other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        public Loc Negate() => -this;

        public Loc Add(Loc other) => this + other;

        public Loc Subtract(Loc other) => this - other;

        public Loc Multiply(Loc other) => this * other;

        public Loc Multiply(int scaleFactor) => this * scaleFactor;

        public Loc Divide(Loc other) => this / other;

        public Loc Divide(int scaleFactor) => this / scaleFactor;

        public Loc Mod(Loc other) => this % other;

        public Loc Mod(int scaleFactor) => this % scaleFactor;
    }
}
