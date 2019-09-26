// <copyright file="LocRay.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RogueElements
{
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName",
        MessageId = nameof(LocRay8),
        Justification = "Defines multiple LocRay structs with descriptive suffixes")]

    [Serializable]
    public struct LocRay8 : IEquatable<LocRay8>
    {
        public Loc Loc;
        public Dir8 Dir;

        public LocRay8(Loc loc)
        {
            this.Loc = loc;
            this.Dir = Dir8.None;
        }

        public LocRay8(Dir8 dir)
        {
            this.Loc = Loc.Zero;
            this.Dir = dir;
        }

        public LocRay8(Loc loc, Dir8 dir)
        {
            this.Loc = loc;
            this.Dir = dir;
        }

        public LocRay8(int x, int y, Dir8 dir)
        {
            this.Loc = new Loc(x, y);
            this.Dir = dir;
        }

        public static bool operator ==(LocRay8 lhs, LocRay8 rhs) => lhs.Equals(rhs);

        public static bool operator !=(LocRay8 lhs, LocRay8 rhs) => !lhs.Equals(rhs);

        public Loc Traverse(int dist)
        {
            return this.Loc + (this.Dir.GetLoc() * dist);
        }

        public bool Equals(LocRay8 other) => this.Loc == other.Loc && this.Dir == other.Dir;

        public override bool Equals(object obj) => (obj is LocRay8 ray) && this.Equals(ray);

        public override int GetHashCode() => unchecked(971 + (this.Loc.GetHashCode() * 619) ^ (this.Dir.GetHashCode() * 491));
    }

    [Serializable]
    public struct LocRay4 : IEquatable<LocRay4>
    {
        public Loc Loc;
        public Dir4 Dir;

        public LocRay4(Loc loc)
        {
            this.Loc = loc;
            this.Dir = Dir4.None;
        }

        public LocRay4(Dir4 dir)
        {
            this.Loc = Loc.Zero;
            this.Dir = dir;
        }

        public LocRay4(Loc loc, Dir4 dir)
        {
            this.Loc = loc;
            this.Dir = dir;
        }

        public LocRay4(int x, int y, Dir4 dir)
        {
            this.Loc = new Loc(x, y);
            this.Dir = dir;
        }

        public static bool operator ==(LocRay4 lhs, LocRay4 rhs) => lhs.Equals(rhs);

        public static bool operator !=(LocRay4 lhs, LocRay4 rhs) => !lhs.Equals(rhs);

        public Loc Traverse(int dist)
        {
            return this.Loc + (this.Dir.GetLoc() * dist);
        }

        public bool Equals(LocRay4 other) => this.Loc == other.Loc && this.Dir == other.Dir;

        public override bool Equals(object obj) => (obj is LocRay4) && this.Equals((LocRay4)obj);

        public override int GetHashCode() => unchecked(571 + (this.Loc.GetHashCode() * 293) ^ (this.Dir.GetHashCode() * 827));
    }
}
