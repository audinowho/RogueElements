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
        public int[][] Map;
        public List<MapBlob> Blobs;

        public BlobMap(int width, int height)
        {
            Map = new int[width][];
            for (int xx = 0; xx < width; xx++)
            {
                Map[xx] = new int[height];
                for (int yy = 0; yy < height; yy++)
                    Map[xx][yy] = -1;
            }

            Blobs = new List<MapBlob>();
        }
    }

    public struct MapBlob : IEquatable<MapBlob>
    {
        public Rect Bounds;
        public int Area;

        public MapBlob(Rect bounds, int area)
        {
            Bounds = bounds;
            Area = area;
        }


        public override bool Equals(object obj)
        {
            return (obj is MapBlob) && Equals((MapBlob)obj);
        }

        public bool Equals(MapBlob other)
        {
            return (Area == other.Area && Bounds == other.Bounds);
        }


        public static bool operator ==(MapBlob value1, MapBlob value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(MapBlob value1, MapBlob value2)
        {
            return !(value1 == value2);
        }

        public override int GetHashCode()
        {
            return Bounds.GetHashCode() ^ Area.GetHashCode();
        }
    }
}
