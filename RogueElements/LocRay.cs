// <copyright file="LocRay.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RogueElements
{
    /// <summary>
    /// Represents a ray originating from a location in one of eight directions.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName",
        MessageId = nameof(LocRay8),
        Justification = "Defines multiple LocRay structs with descriptive suffixes")]

    [Serializable]
    public struct LocRay8 : IEquatable<LocRay8>
    {
        /// <summary>
        /// The origin location of the ray.
        /// </summary>
        public Loc Loc;

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public Dir8 Dir;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay8"/> struct with a location and no direction.
        /// </summary>
        /// <param name="loc">The origin location.</param>
        public LocRay8(Loc loc)
        {
            this.Loc = loc;
            this.Dir = Dir8.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay8"/> struct with a direction and origin at zero.
        /// </summary>
        /// <param name="dir">The direction of the ray.</param>
        public LocRay8(Dir8 dir)
        {
            this.Loc = Loc.Zero;
            this.Dir = dir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay8"/> struct with specified location and direction.
        /// </summary>
        /// <param name="loc">The origin location.</param>
        /// <param name="dir">The direction of the ray.</param>
        public LocRay8(Loc loc, Dir8 dir)
        {
            this.Loc = loc;
            this.Dir = dir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay8"/> struct with specified coordinates and direction.
        /// </summary>
        /// <param name="x">The X coordinate of the origin.</param>
        /// <param name="y">The Y coordinate of the origin.</param>
        /// <param name="dir">The direction of the ray.</param>
        public LocRay8(int x, int y, Dir8 dir)
        {
            this.Loc = new Loc(x, y);
            this.Dir = dir;
        }

        public static bool operator ==(LocRay8 lhs, LocRay8 rhs) => lhs.Equals(rhs);

        public static bool operator !=(LocRay8 lhs, LocRay8 rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Computes the location after traversing a specified distance along the ray.
        /// </summary>
        /// <param name="dist">The distance to traverse.</param>
        /// <returns>The resulting location.</returns>
        public Loc Traverse(int dist)
        {
            return this.Loc + (this.Dir.GetLoc() * dist);
        }

        /// <inheritdoc/>
        public bool Equals(LocRay8 other) => this.Loc == other.Loc && this.Dir == other.Dir;

        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is LocRay8 ray) && this.Equals(ray);

        /// <inheritdoc/>
        public override int GetHashCode() => unchecked(971 + (this.Loc.GetHashCode() * 619) ^ (this.Dir.GetHashCode() * 491));

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}, {1}", this.Loc, this.Dir);
        }
    }

    /// <summary>
    /// Represents a ray originating from a location in one of four cardinal directions.
    /// </summary>
    [Serializable]
    public struct LocRay4 : IEquatable<LocRay4>
    {
        /// <summary>
        /// The origin location of the ray.
        /// </summary>
        public Loc Loc;

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public Dir4 Dir;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay4"/> struct by copying another.
        /// </summary>
        /// <param name="locRay4">The ray to copy.</param>
        public LocRay4(LocRay4 locRay4)
        {
            this.Loc = locRay4.Loc;
            this.Dir = locRay4.Dir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay4"/> struct with a location and no direction.
        /// </summary>
        /// <param name="loc">The origin location.</param>
        public LocRay4(Loc loc)
        {
            this.Loc = loc;
            this.Dir = Dir4.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay4"/> struct with a direction and origin at zero.
        /// </summary>
        /// <param name="dir">The direction of the ray.</param>
        public LocRay4(Dir4 dir)
        {
            this.Loc = Loc.Zero;
            this.Dir = dir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay4"/> struct with specified location and direction.
        /// </summary>
        /// <param name="loc">The origin location.</param>
        /// <param name="dir">The direction of the ray.</param>
        public LocRay4(Loc loc, Dir4 dir)
        {
            this.Loc = loc;
            this.Dir = dir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocRay4"/> struct with specified coordinates and direction.
        /// </summary>
        /// <param name="x">The X coordinate of the origin.</param>
        /// <param name="y">The Y coordinate of the origin.</param>
        /// <param name="dir">The direction of the ray.</param>
        public LocRay4(int x, int y, Dir4 dir)
        {
            this.Loc = new Loc(x, y);
            this.Dir = dir;
        }

        public static bool operator ==(LocRay4 lhs, LocRay4 rhs) => lhs.Equals(rhs);

        public static bool operator !=(LocRay4 lhs, LocRay4 rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Computes the location after traversing a specified distance along the ray.
        /// </summary>
        /// <param name="dist">The distance to traverse.</param>
        /// <returns>The resulting location.</returns>
        public Loc Traverse(int dist)
        {
            return this.Loc + (this.Dir.GetLoc() * dist);
        }

        /// <inheritdoc/>
        public bool Equals(LocRay4 other) => this.Loc == other.Loc && this.Dir == other.Dir;

        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is LocRay4) && this.Equals((LocRay4)obj);

        /// <inheritdoc/>
        public override int GetHashCode() => unchecked(571 + (this.Loc.GetHashCode() * 293) ^ (this.Dir.GetHashCode() * 827));

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}, {1}", this.Loc, this.Dir);
        }
    }
}
