// <copyright file="Loc.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace RogueElements
{
    [Serializable]
    public struct Loc : IEquatable<Loc>
    {

        public int X;
        public int Y;

        public Loc(int n)
        {
            X = n;
            Y = n;
        }

        public Loc(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Loc(Loc loc)
        {
            X = loc.X;
            Y = loc.Y;
        }

        private static readonly Loc zero = new Loc(0);
        private static readonly Loc unit = new Loc(1);
        private static readonly Loc unitX = new Loc(1, 0);
        private static readonly Loc unitY = new Loc(0, 1);

        public static Loc Zero { get { return zero; } }
        public static Loc One { get { return unit; } }
        public static Loc UnitX { get { return unitX; } }
        public static Loc UnitY { get { return unitY; } }


        /// <summary>
        /// Gets the square of the total distance of the loc from (0,0), in Euclidean distance.
        /// </summary>
        /// <returns></returns>
        public int DistSquared()
        {
            return X * X + Y * Y;
        }

        /// <summary>
        /// Returns the vector length in integer units.
        /// </summary>
        /// <returns></returns>
        public int Length()
        {
            return (int)Math.Abs(Math.Round(Math.Sqrt(DistSquared())));
        }

        /// <summary>
        /// Gets the total distance of the loc from (0,0), in 8-Directional (Chess) distance.
        /// </summary>
        /// <returns></returns>
        public int Dist8()
        {
            return Math.Max(Math.Abs(X), Math.Abs(Y));
        }

        /// <summary>
        /// Gets the total distance of the loc from (0,0), in 4-Directional (Manhattan) distance.
        /// </summary>
        /// <returns></returns>
        public int Dist4()
        {
            return Math.Abs(X) + Math.Abs(Y);
        }

        public override string ToString()
        {
            return $"X:{X} Y:{Y}";
        }

        public override bool Equals(object obj)
        {
            return (obj is Loc) && Equals((Loc)obj);
        }

        public bool Equals(Loc other)
        {
            return (X == other.X && Y == other.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
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
            return new Loc(value1.X < value2.X ? value1.X : value2.X,
                               value1.Y < value2.Y ? value1.Y : value2.Y);
        }
        public static Loc Max(Loc value1, Loc value2)
        {
            return new Loc(value1.X > value2.X ? value1.X : value2.X,
                               value1.Y > value2.Y ? value1.Y : value2.Y);
        }

    }
}
