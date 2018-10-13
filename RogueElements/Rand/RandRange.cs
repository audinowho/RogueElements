using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public struct RandRange : IRandPicker<int>
    {
        public int Min;
        public int Max;
        public bool ChangesState { get { return false; } }
        public bool CanPick { get { return Min <= Max; } }

        public RandRange(int num) { Min = num; Max = num; }
        public RandRange(int min, int max) { Min = min; Max = max; }

        public IEnumerator<int> GetEnumerator()
        {
            yield return Min;
            for (int ii = Min + 1; ii < Max; ii++)
                yield return ii;
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public int Pick(IRandom rand) { return rand.Next(Min, Max); }
    }
}
