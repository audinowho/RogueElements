using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class PresetMultiRand<T> : IMultiRandPicker<T>
    {
        public List<T> ToSpawn;
        public bool ChangesState { get { return false; } }
        public bool CanPick { get { return ToSpawn != null; } }

        public PresetMultiRand() { ToSpawn = new List<T>(); }
        public PresetMultiRand(params T[] toSpawn) : this() { ToSpawn.AddRange(toSpawn); }
        public PresetMultiRand(List<T> toSpawn) { ToSpawn = toSpawn; }

        public List<T> Roll(IRandom rand)
        {
            List<T> result = new List<T>();
            result.AddRange(ToSpawn);
            return result;
        }
    }
    
}
