using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Geenrates spawnables from a specifically defined IMultiRandPicker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public class PickerSpawner<T, E> : IStepSpawner<T, E> 
        where T : IGenContext
        where E : ISpawnable
    {
        public IMultiRandPicker<E> Picker;

        public PickerSpawner() { }

        public PickerSpawner(IMultiRandPicker<E> picker)
        {
            Picker = picker;
        }

        public List<E> GetSpawns(T map)
        {
            IMultiRandPicker<E> picker = Picker;
            if (picker.ChangesState)
                picker = picker.CopyState();
            List<E> results = picker.Roll(map.Rand);
            List<E> copyResults = new List<E>();
            foreach (E result in results)
                copyResults.Add((E)result.Copy());
            return copyResults;
        }
    }
}
