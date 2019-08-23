// <copyright file="TestTile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements.Tests
{
    public class TestTile : ITile
    {
        public TestTile()
        {
        }

        public TestTile(int id)
        {
            this.ID = id;
        }

        protected TestTile(TestTile other)
        {
            this.ID = other.ID;
        }

        public int ID { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TestTile other))
                return false;
            return other.ID == this.ID;
        }

        public ITile Copy() => new TestTile(this);

        public bool TileEquivalent(ITile other) => this.Equals(other);

        public override int GetHashCode() => this.ID;
    }
}