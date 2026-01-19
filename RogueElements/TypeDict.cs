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
    /// <summary>
    /// A dictionary that stores items keyed by their runtime type, allowing only one instance per type.
    /// </summary>
    /// <typeparam name="T">The base type of items that can be stored.</typeparam>
    [Serializable]
    public class TypeDict<T> : ITypeDict<T>, ITypeDict
    {
        [NonSerialized]
        private Dictionary<string, T> pointers;

        private List<T> serializationObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDict{T}"/> class.
        /// </summary>
        public TypeDict()
        {
            this.pointers = new Dictionary<string, T>();
        }

        /// <summary>
        /// Gets the number of items in the dictionary.
        /// </summary>
        public int Count => this.pointers.Count;

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        public void Clear()
        {
            this.pointers.Clear();
        }

        /// <summary>
        /// Determines whether the dictionary contains an item of the specified type.
        /// </summary>
        /// <typeparam name="TK">The type to check.</typeparam>
        /// <returns><c>true</c> if an item of that type exists; otherwise <c>false</c>.</returns>
        public bool Contains<TK>()
            where TK : T
        {
            Type type = typeof(TK);
            return this.Contains(type);
        }

        /// <summary>
        /// Determines whether the dictionary contains an item of the specified type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if an item of that type exists; otherwise <c>false</c>.</returns>
        public bool Contains(Type type)
        {
            return this.pointers.ContainsKey(type.AssemblyQualifiedName);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int idx)
        {
            foreach (T element in this.pointers.Values)
            {
                array[idx] = element;
                idx++;
            }
        }

        /// <inheritdoc/>
        bool ICollection<T>.Contains(T element)
        {
            return this.Contains(element.GetType());
        }

        /// <summary>
        /// Gets the item of the specified type.
        /// </summary>
        /// <typeparam name="TK">The type of item to get.</typeparam>
        /// <returns>The item of that type.</returns>
        public TK Get<TK>()
            where TK : T
        {
            Type type = typeof(TK);
            return (TK)this.pointers[type.AssemblyQualifiedName];
        }

        /// <summary>
        /// Gets the item of the specified type.
        /// </summary>
        /// <param name="type">The type of item to get.</param>
        /// <returns>The item of that type.</returns>
        public T Get(Type type)
        {
            return this.pointers[type.AssemblyQualifiedName];
        }

        /// <summary>
        /// Attempts to get an item of the specified type.
        /// </summary>
        /// <typeparam name="TK">The type of item to get.</typeparam>
        /// <param name="item">The item if found.</param>
        /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
        public bool TryGet<TK>(out TK item)
            where TK : T
        {
            Type type = typeof(TK);
            T val;
            bool success = this.pointers.TryGetValue(type.AssemblyQualifiedName, out val);
            item = (TK)val;
            return success;
        }

        /// <summary>
        /// Attempts to get an item of the specified type.
        /// </summary>
        /// <param name="type">The type of item to get.</param>
        /// <param name="item">The item if found.</param>
        /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
        public bool TryGet(Type type, out T item)
        {
            return this.pointers.TryGetValue(type.AssemblyQualifiedName, out item);
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            this.pointers[item.GetType().AssemblyQualifiedName] = item;
        }

        /// <summary>
        /// Sets an item in the dictionary, keyed by its runtime type.
        /// </summary>
        /// <param name="item">The item to set.</param>
        public void Set(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            this.pointers[item.GetType().AssemblyQualifiedName] = item;
        }

        /// <inheritdoc/>
        object ITypeDict.Get(Type type)
        {
            return this.Get(type);
        }

        /// <inheritdoc/>
        void ITypeDict.Set(object item)
        {
            this.Set((T)item);
        }

        /// <summary>
        /// Removes the item of the specified type.
        /// </summary>
        /// <typeparam name="TK">The type to remove.</typeparam>
        /// <returns><c>true</c> if removed; otherwise <c>false</c>.</returns>
        public bool Remove<TK>()
            where TK : T
        {
            Type type = typeof(TK);
            return this.Remove(type);
        }

        /// <summary>
        /// Removes the item of the specified type.
        /// </summary>
        /// <param name="type">The type to remove.</param>
        /// <returns><c>true</c> if removed; otherwise <c>false</c>.</returns>
        public bool Remove(Type type)
        {
            return this.pointers.Remove(type.AssemblyQualifiedName);
        }

        /// <inheritdoc/>
        bool ICollection<T>.Remove(T element)
        {
            return this.Remove(element.GetType());
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return this.pointers.Values.GetEnumerator();
        }

        /// <inheritdoc/>
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
            if (this.pointers == null)
                this.pointers = new Dictionary<string, T>();
            if (this.serializationObjects == null)
                this.serializationObjects = new List<T>();
            for (int ii = 0; ii < this.serializationObjects.Count; ii++)
                this.pointers[this.serializationObjects[ii].GetType().AssemblyQualifiedName] = this.serializationObjects[ii];
        }
    }
}
