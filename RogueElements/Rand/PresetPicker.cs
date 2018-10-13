using System;
using System.Collections.Generic;
using System.Collections;

namespace RogueElements
{

    [Serializable]
    public class PresetPicker<T> : IRandPicker<T>
    {
        public T ToSpawn;
        public bool ChangesState { get { return false; } }
        public bool CanPick { get { return true; } }

        public PresetPicker() { }
        public PresetPicker(T toSpawn) { ToSpawn = toSpawn; }

        public IEnumerator<T> GetEnumerator() { yield return ToSpawn; }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public T Pick(IRandom rand) { return ToSpawn; }
    }
}
