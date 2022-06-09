// <copyright file="MathUtils.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace RogueElements
{
    public static class MathUtils
    {
        private static IRandom rand = new ReRandom();
        private static INoise noise = new ReNoise();

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

        public static int IntPow(int num, int factor)
        {
            int result = 1;

            for (int ii = 0; ii < factor; ii++)
                result *= num;

            return result;
        }

        /// <summary>
        /// Division with round down.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="den"></param>
        /// <returns></returns>
        public static int DivDown(int num, int den)
        {
            if (num < 0 && den > 0)
                return ((num + 1) / den) - 1;
            else if (num > 0 && den < 0)
                return ((num - 1) / den) - 1;
            else
                return num / den;
        }

        /// <summary>
        /// Division with round up.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="den"></param>
        /// <returns></returns>
        public static int DivUp(int num, int den)
        {
            if (num > 0 && den > 0)
                return ((num - 1) / den) + 1;
            else if (num < 0 && den < 0)
                return ((num + 1) / den) + 1;
            else
                return num / den;
        }

        public static int Wrap(int num, int size)
        {
            return ((num % size) + size) % size;
        }
    }
}
