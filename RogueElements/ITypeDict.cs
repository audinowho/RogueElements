// <copyright file="ITypeDict.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    public interface ITypeDict<T> : IEnumerable<T>, ICollection<T>
    {
        bool Contains(Type type);

        bool Contains<TK>()
            where TK : T;

        TK Get<TK>()
            where TK : T;

        bool Remove<TK>()
            where TK : T;

        T Get(Type type);

        void Set(T item);

        bool Remove(Type type);
    }

    public interface ITypeDict : IEnumerable
    {
        void Clear();

        bool Contains(Type type);

        object Get(Type type);

        void Set(object item);

        bool Remove(Type type);
    }
}
