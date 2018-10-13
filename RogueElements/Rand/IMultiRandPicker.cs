using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IMultiRandPicker<T>
    {
        bool ChangesState { get; }
        bool CanPick { get; }
        List<T> Roll(IRandom rand);
    }
    
}
