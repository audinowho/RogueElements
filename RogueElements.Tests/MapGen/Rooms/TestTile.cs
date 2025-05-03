// <copyright file="TestTile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements.Tests
{
    public struct TestTile : ITile<TestTile>, IEquatable<TestTile>
    {
        public int ID { get; set; }

        public TestTile()
        {
        }

        public TestTile(int id)
        {
            this.ID = id;
        }

        // protected TestTile(TestTile other)
        // {
        //     this.ID = other.ID;
        // }

        public bool Equals(TestTile other)
        {
            return this.ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            return obj is TestTile other && this.Equals(other);
        }

        public bool TileEquivalent(TestTile other)
        {
            throw new System.NotImplementedException();
        }

        public TestTile Copy()
        {
            return this;
        }

        ITile ITile.Copy() => this.Copy();

        bool ITile.TileEquivalent(ITile other) => this.Equals(other);

        public override int GetHashCode() => this.ID;
    }
}