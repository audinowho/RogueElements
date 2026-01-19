// <copyright file="MathUtils.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides mathematical utility methods and global random number generation.
    /// </summary>
    public static class MathUtils
    {
        private static IRandom rand = new ReRandom();
        private static INoise noise = new ReNoise();

        /// <summary>
        /// Gets the global random number generator.
        /// </summary>
        public static IRandom Rand
        {
            get
            {
                return rand;
            }
        }

        /// <summary>
        /// Gets the global noise generator.
        /// </summary>
        public static INoise Noise
        {
            get
            {
                return noise;
            }
        }

        /// <summary>
        /// Re-seeds both the global random and noise generators.
        /// </summary>
        /// <param name="seed">The seed value.</param>
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

        /// <summary>
        /// Adds an amount to a dictionary entry, creating it if needed.
        /// </summary>
        /// <typeparam name="T">The key type.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="amt">The amount to add.</param>
        public static void AddToDictionary<T>(Dictionary<T, int> dict, T key, int amt)
        {
            dict.TryGetValue(key, out int currentCount);
            dict[key] = currentCount + amt;
        }

        /// <summary>
        /// Merges counts from one dictionary into another.
        /// </summary>
        /// <typeparam name="T">The key type.</typeparam>
        /// <param name="dict1">The destination dictionary.</param>
        /// <param name="dict2">The source dictionary.</param>
        public static void AddToDictionary<T>(Dictionary<T, int> dict1, Dictionary<T, int> dict2)
        {
            foreach (T key in dict2.Keys)
                AddToDictionary<T>(dict1, key, dict2[key]);
        }

        /// <summary>
        /// Performs bilinear interpolation between four corner values.
        /// </summary>
        /// <param name="topleft">Top-left corner value.</param>
        /// <param name="topright">Top-right corner value.</param>
        /// <param name="bottomleft">Bottom-left corner value.</param>
        /// <param name="bottomright">Bottom-right corner value.</param>
        /// <param name="degreeX">X position within the cell.</param>
        /// <param name="xTotal">Total X divisions.</param>
        /// <param name="degreeY">Y position within the cell.</param>
        /// <param name="yTotal">Total Y divisions.</param>
        /// <returns>The interpolated value.</returns>
        public static int BiInterpolate(int topleft, int topright, int bottomleft, int bottomright, int degreeX, int xTotal, int degreeY, int yTotal)
        {
            int bottom = ((topleft * (xTotal - degreeX)) + (topright * degreeX)) * (yTotal - degreeY) / xTotal;
            int top = ((bottomleft * (xTotal - degreeX)) + (bottomright * degreeX)) * degreeY / xTotal;
            return (bottom + top) / yTotal;
        }

        /// <summary>
        /// Performs linear interpolation between two values.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="degree">Position between a and b.</param>
        /// <param name="total">Total divisions.</param>
        /// <returns>The interpolated value.</returns>
        public static int Interpolate(int a, int b, int degree, int total)
        {
            return ((a * (total - degree)) + (b * degree)) / total;
        }

        /// <summary>
        /// Computes integer exponentiation.
        /// </summary>
        /// <param name="num">The base.</param>
        /// <param name="factor">The exponent.</param>
        /// <returns>num raised to the power of factor.</returns>
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

        /// <summary>
        /// Wraps a number to be within [0, size).
        /// </summary>
        /// <param name="num">The number to wrap.</param>
        /// <param name="size">The wrap boundary.</param>
        /// <returns>The wrapped value.</returns>
        public static int Wrap(int num, int size)
        {
            return ((num % size) + size) % size;
        }
    }
}
