// <copyright file="BlobMap.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public class BlobMap
    {
        public BlobMap(int width, int height)
        {
            this.Map = new int[width][];
            for (int xx = 0; xx < width; xx++)
            {
                this.Map[xx] = new int[height];
                for (int yy = 0; yy < height; yy++)
                    this.Map[xx][yy] = -1;
            }

            this.Blobs = new List<Blob>();
        }

        public int[][] Map { get; }

        public List<Blob> Blobs { get; }

        public struct Blob : IEquatable<Blob>
        {
            public Rect Bounds;
            public int Area;

            public Blob(Rect bounds, int area)
            {
                this.Bounds = bounds;
                this.Area = area;
            }

            public static bool operator ==(Blob value1, Blob value2)
            {
                return value1.Equals(value2);
            }

            public static bool operator !=(Blob value1, Blob value2)
            {
                return !(value1 == value2);
            }

            public override int GetHashCode()
            {
                return this.Bounds.GetHashCode() ^ this.Area.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return (obj is Blob) && this.Equals((Blob)obj);
            }

            public bool Equals(Blob other)
            {
                return this.Area == other.Area && this.Bounds == other.Bounds;
            }
        }
    }
}
