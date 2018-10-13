using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class PickerSpawner<T, E> : IStepSpawner<T, E> 
        where T : IGenContext
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
                picker = picker.Copy();
            return picker.Roll(map.Rand);
        }
    }
}
