using System;
using System.Collections.Generic;
using System.Collections;

namespace RogueElements
{
    public interface IRandPicker<T> : IEnumerable<T>, IEnumerable
    {
        bool ChangesState { get; }
        bool CanPick { get; }
        T Pick(IRandom rand);
    }
    
}
