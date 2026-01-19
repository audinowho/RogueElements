// <copyright file="BlobMap.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a 2D map that identifies connected regions (blobs) of tiles.
    /// </summary>
    public class BlobMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobMap"/> class with the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the map.</param>
        /// <param name="height">The height of the map.</param>
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

        /// <summary>
        /// Gets the 2D array mapping each tile to its blob index (-1 if not part of a blob).
        /// </summary>
        public int[][] Map { get; }

        /// <summary>
        /// Gets the list of identified blobs in the map.
        /// </summary>
        public List<Blob> Blobs { get; }

        /// <summary>
        /// Represents a connected region of tiles with its bounding rectangle and area.
        /// </summary>
        public struct Blob : IEquatable<Blob>
        {
            /// <summary>
            /// The bounding rectangle containing the blob.
            /// </summary>
            public Rect Bounds;

            /// <summary>
            /// The number of tiles in the blob.
            /// </summary>
            public int Area;

            /// <summary>
            /// Initializes a new instance of the <see cref="Blob"/> struct.
            /// </summary>
            /// <param name="bounds">The bounding rectangle.</param>
            /// <param name="area">The tile count.</param>
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

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return this.Bounds.GetHashCode() ^ this.Area.GetHashCode();
            }

            /// <inheritdoc/>
            public override bool Equals(object obj)
            {
                return (obj is Blob) && this.Equals((Blob)obj);
            }

            /// <inheritdoc/>
            public bool Equals(Blob other)
            {
                return this.Area == other.Area && this.Bounds == other.Bounds;
            }
        }
    }
}
