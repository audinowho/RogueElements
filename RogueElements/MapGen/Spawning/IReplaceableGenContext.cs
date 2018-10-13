using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface IReplaceableGenContext<T> : IViewPlaceableGenContext<T>
    {
        void SetItem(int index, T item);
        void RemoveItemAt(int index);
    }

}
