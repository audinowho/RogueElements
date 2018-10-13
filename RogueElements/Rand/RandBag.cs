using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RandBag<T> : IRandPicker<T>
    {
        public List<T> ToSpawn;
        public bool RemoveOnRoll;
        public bool ChangesState { get { return RemoveOnRoll; } }
        public bool CanPick { get { return ToSpawn.Count > 0; } }

        public RandBag() { ToSpawn = new List<T>(); }
        public RandBag(params T[] toSpawn) : this() { ToSpawn.AddRange(toSpawn); }
        public RandBag(List<T> toSpawn) { ToSpawn = toSpawn; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T element in ToSpawn)
                yield return element;
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public T Pick(IRandom rand)
        {
            int index = rand.Next(ToSpawn.Count);
            T choice = ToSpawn[index];
            if (RemoveOnRoll)
                ToSpawn.RemoveAt(index);
            return choice;
        }
    }
    
}
