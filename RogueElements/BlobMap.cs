using System;
using System.Collections.Generic;

namespace RogueElements
{

    public class BlobMap
    {
        public int[][] Map;
        public List<Rect> Blobs;
        public List<int> Sizes;

        public BlobMap(int width, int height)
        {
            Map = new int[width][];
            for (int ii = 0; ii < width; ii++)
                Map[ii] = new int[height];

            Blobs = new List<Rect>();
            Sizes = new List<int>();
        }
    }
}
