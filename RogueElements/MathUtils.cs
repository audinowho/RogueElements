using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    public class MathUtils
    {
        private static ReRandom rand = new ReRandom();
        public static ReRandom Rand
        {
            get
            {
                return rand;
            }
        }
        public static void ReSeedRand(ulong seed)
        {
            rand = new ReRandom(seed);
        }

        /// <summary>
        /// Choose a random member from a set.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="rand"></param>
        public static T ChooseFromHash<T>(HashSet<T> hash, IRandom rand)
        {
            T[] crossArray = new T[hash.Count];
            hash.CopyTo(crossArray);
            return crossArray[rand.Next(crossArray.Length)];
        }

        public static void AddToDictionary<T>(Dictionary<T, int> dict, T key, int amt)
        {
            int currentCount;
            dict.TryGetValue(key, out currentCount);
            dict[key] = currentCount + amt;
        }

        public static void AddToDictionary<T>(Dictionary<T, int> dict1, Dictionary<T, int> dict2)
        {
            foreach (T key in dict2.Keys)
                AddToDictionary<T>(dict1, key, dict2[key]);
        }

        public delegate int CompareFunction<T>(T a, T b);
        public static void AddToSortedList<T>(List<T> list, T element, CompareFunction<T> compareFunc)
        {
            //stable
            int min = 0;
            int max = list.Count - 1;
            int point = max;
            int compare = -1;
            //binary search
            while (min <= max)
            {
                point = (min + max) / 2;

                compare = compareFunc(list[point], element);

                if (compare > 0) //go down
                    max = point - 1;
                else if (compare < 0) //go up
                    min = point + 1;
                else
                {
                    //go past the last index of equal comparison
                    point++;
                    while (point < list.Count && compareFunc(list[point], element) == 0)
                        point++;
                    list.Insert(point, element);
                    return;
                }
            }
            //no place found
            if (compare > 0) //put this one under the current point
                list.Insert(point, element);
            else //put this one above the current point
                list.Insert(point + 1, element);
        }

        //public static List<int> RandomDivide(IRandom rand, int min, int max, int pieces)
        //{
        //    //divides a region [min,max] into pieces amount
        //    //division points include (min,max)
        //    if (pieces > max - min)
        //        throw new Exception("Not enough space to divide!");
        //    List<int> divides = new List<int>();
        //    if (pieces > 1)
        //    {
        //        divides.Add(rand.Next(min + 1, max));
        //        if (pieces > 2)
        //        {
        //            int newDiv = ChooseRandom(rand, min + 1, max, divides);
        //            for (int ii = 0; ii <= divides.Count; ii++)
        //            {
        //                if (ii == divides.Count)
        //                    divides.Add(newDiv);
        //                else if (divides[ii] > newDiv)
        //                    divides.Insert(ii, newDiv);
        //            }
        //        }
        //    }
        //    return divides;
        //}

        //public static int ChooseRandom(IRandom rand, int min, int max, List<int> banned)
        //{
        //    int val = rand.Next(min, max - banned.Count);
        //    for (int ii = 0; ii < banned.Count; ii++)
        //    {
        //        if (banned[ii] <= val)
        //            val++;
        //        else
        //            break;
        //    }
        //    return val;
        //}

    }
}
