using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RandBinomial : IRandPicker<int>
    {
        public int Offset;
        public int Trials;
        public int Percent;
        public bool ChangesState { get { return false; } }
        public bool CanPick { get { return true; } }

        public RandBinomial(int trials, int percent) { Trials = trials; Percent = percent; }
        public RandBinomial(int trials, int percent, int offset) : this(trials, percent) { Offset = offset; }
        protected RandBinomial(RandBinomial other)
        {
            Offset = other.Offset;
            Trials = other.Trials;
            Percent = other.Percent;
        }
        public IRandPicker<int> CopyState() { return new RandBinomial(this); }

        public IEnumerator<int> GetEnumerator()
        {
            for(int ii = 0; ii < Trials; ii++)
                yield return ii;
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public int Pick(IRandom rand)
        {
            int total = 0;
            for (int ii = 0; ii < Trials; ii++)
            {
                if (rand.Next(100) < Percent)
                    total++;
            }
            return Offset + total;
        }
    }
}
