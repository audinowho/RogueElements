using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a list of items by repeatedly calling an IRandPicker
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class LoopedRand<T> : IMultiRandPicker<T>
    {
        public IRandPicker<T> Spawner;
        public IRandPicker<int> AmountSpawner;
        public bool ChangesState { get { return Spawner.ChangesState || AmountSpawner.ChangesState; } }
        public bool CanPick { get { return AmountSpawner.CanPick; } }

        public LoopedRand() { }
        public LoopedRand(IRandPicker<T> spawner, IRandPicker<int> amountSpawner)
        {
            Spawner = spawner;
            AmountSpawner = amountSpawner;
        }
        protected LoopedRand(LoopedRand<T> other)
        {
            Spawner = other.Spawner.CopyState();
            AmountSpawner = other.AmountSpawner.CopyState();
        }
        public IMultiRandPicker<T> CopyState() { return new LoopedRand<T>(this); }

        public List<T> Roll(IRandom rand)
        {
            List<T> result = new List<T>();
            int amount = AmountSpawner.Pick(rand);
            for (int ii = 0; ii < amount; ii++)
            {
                if (!Spawner.CanPick)
                    break;
                result.Add(Spawner.Pick(rand));
            }
            return result;
        }
    }
}
