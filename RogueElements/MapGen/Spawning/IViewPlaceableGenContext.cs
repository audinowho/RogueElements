using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface IViewPlaceableGenContext<T> : IPlaceableGenContext<T>
    {
        int Count { get; }
        T GetItem(int index);
        Loc GetLoc(int index);
    }

}
