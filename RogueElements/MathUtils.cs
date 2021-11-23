// <copyright file="MathUtils.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    public static class MathUtils
    {
        private static IRandom rand = new ReRandom();
        private static INoise noise = new ReNoise();

        public delegate int CompareFunction<T>(T a, T b);

        public static IRandom Rand
        {
            get
            {
                return rand;
            }
        }

        public static INoise Noise
        {
            get
            {
                return noise;
            }
        }

        public static void ReSeedRand(ulong seed)
        {
            rand = new ReRandom(seed);
            noise = new ReNoise(seed);
        }

        /// <summary>
        /// Choose a random member from a set.
        /// </summary>
        /// <typeparam name="T">Type of the input <see cref="HashSet{T}"/></typeparam>
        /// <param name="hash"></param>
        /// <param name="rand"></param>
        /// <returns></returns>
        public static T ChooseFromHash<T>(HashSet<T> hash, IRandom rand)
        {
            T[] crossArray = new T[hash.Count];
            hash.CopyTo(crossArray);
            return crossArray[rand.Next(crossArray.Length)];
        }

        public static void AddToDictionary<T>(Dictionary<T, int> dict, T key, int amt)
        {
            dict.TryGetValue(key, out int currentCount);
            dict[key] = currentCount + amt;
        }

        public static void AddToDictionary<T>(Dictionary<T, int> dict1, Dictionary<T, int> dict2)
        {
            foreach (T key in dict2.Keys)
                AddToDictionary<T>(dict1, key, dict2[key]);
        }

        public static void AddToSortedList<T>(List<T> list, T element, CompareFunction<T> compareFunc)
        {
            if (compareFunc == null)
                throw new ArgumentNullException(nameof(compareFunc));

            // stable
            int min = 0;
            int max = list.Count - 1;
            int point = max;
            int compare = -1;

            // binary search
            while (min <= max)
            {
                point = (min + max) / 2;

                compare = compareFunc(list[point], element);

                if (compare > 0)
                {
                    // go down
                    max = point - 1;
                }
                else if (compare < 0)
                {
                    // go up
                    min = point + 1;
                }
                else
                {
                    // go past the last index of equal comparison
                    point++;
                    while (point < list.Count && compareFunc(list[point], element) == 0)
                        point++;
                    list.Insert(point, element);
                    return;
                }
            }

            // no place found
            list.Insert(point + (compare > 0 ? 0 : 1), element);
        }

        public static int BiInterpolate(int topleft, int topright, int bottomleft, int bottomright, int degreeX, int xTotal, int degreeY, int yTotal)
        {
            int bottom = ((topleft * (xTotal - degreeX)) + (topright * degreeX)) * (yTotal - degreeY) / xTotal;
            int top = ((bottomleft * (xTotal - degreeX)) + (bottomright * degreeX)) * degreeY / xTotal;
            return (bottom + top) / yTotal;
        }

        public static int Interpolate(int a, int b, int degree, int total)
        {
            return ((a * (total - degree)) + (b * degree)) / total;
        }
    }
}
