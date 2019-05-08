using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface IReplaceableGenContext<T> : IViewPlaceableGenContext<T>
        where T : ISpawnable
    {
        void SetItem(int index, T item);
        void RemoveItemAt(int index);
    }

}
