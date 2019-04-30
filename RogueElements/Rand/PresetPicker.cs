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
        protected PresetPicker(PresetPicker<T> other)
        {
            ToSpawn = other.ToSpawn;
        }
        public IRandPicker<T> CopyState() { return new PresetPicker<T>(this); }

        public IEnumerator<T> GetEnumerator() { yield return ToSpawn; }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public T Pick(IRandom rand) { return ToSpawn; }
    }
}
