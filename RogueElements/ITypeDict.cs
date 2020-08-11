// <copyright file="ITypeDict.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    public interface ITypeDict<T>
    {
        void Clear();

        bool Contains(Type type);

        bool Contains<TK>()
            where TK : T;

        TK Get<TK>()
            where TK : T;

        void Remove<TK>()
            where TK : T;

        T Get(Type type);

        void Set(T item);

        void Remove(Type type);
    }

    public interface ITypeDict
    {
        void Clear();

        bool Contains(Type type);

        object Get(Type type);

        void Set(object item);

        void Remove(Type type);
    }
}
