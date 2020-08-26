// <copyright file="TypeDict.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RogueElements
{
    [Serializable]
    public class TypeDict<T> : ITypeDict<T>, ITypeDict
    {
        [NonSerialized]
        private Dictionary<string, T> pointers;

        private List<T> serializationObjects;

        public TypeDict()
        {
            this.pointers = new Dictionary<string, T>();
        }

        public int Count => this.pointers.Count;

        public void Clear()
        {
            this.pointers.Clear();
        }

        public bool Contains<TK>()
            where TK : T
        {
            Type type = typeof(TK);
            return this.Contains(type);
        }

        public bool Contains(Type type)
        {
            return this.pointers.ContainsKey(type.AssemblyQualifiedName);
        }

        public TK Get<TK>()
            where TK : T
        {
            Type type = typeof(TK);
            return (TK)this.pointers[type.AssemblyQualifiedName];
        }

        public T Get(Type type)
        {
            return this.pointers[type.AssemblyQualifiedName];
        }

        public bool TryGet<TK>(out TK item)
            where TK : T
        {
            Type type = typeof(TK);
            T val;
            bool success = this.pointers.TryGetValue(type.AssemblyQualifiedName, out val);
            item = (TK)val;
            return success;
        }

        public bool TryGet(Type type, T item)
        {
            return this.pointers.TryGetValue(type.AssemblyQualifiedName, out item);
        }

        public void Set(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            this.pointers[item.GetType().AssemblyQualifiedName] = item;
        }

        object ITypeDict.Get(Type type)
        {
            return this.Get(type);
        }

        void ITypeDict.Set(object item)
        {
            this.Set((T)item);
        }

        public void Remove<TK>()
            where TK : T
        {
            Type type = typeof(TK);
            this.Remove(type);
        }

        public void Remove(Type type)
        {
            this.pointers.Remove(type.AssemblyQualifiedName);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.pointers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.pointers.Values.GetEnumerator();
        }

        [OnSerializing]
#pragma warning disable CC0057 // Unused parameters
        internal void OnSerializingMethod(StreamingContext context)
#pragma warning restore CC0057 // Unused parameters
        {
            this.serializationObjects = new List<T>();
            foreach (string key in this.pointers.Keys)
                this.serializationObjects.Add(this.pointers[key]);
        }

        [OnDeserialized]
#pragma warning disable CC0057 // Unused parameters
        internal void OnDeserializedMethod(StreamingContext context)
#pragma warning restore CC0057 // Unused parameters
        {
            this.pointers = new Dictionary<string, T>();
            for (int ii = 0; ii < this.serializationObjects.Count; ii++)
                this.pointers[this.serializationObjects[ii].GetType().AssemblyQualifiedName] = this.serializationObjects[ii];
        }
    }
}
