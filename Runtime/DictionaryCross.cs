using System;
using System.Collections;
using System.Collections.Generic;
using Dythervin.Core.Extensions;

namespace Dythervin.Collections
{
    public class DictionaryCross<TMain, T> : IDictionary<TMain, T>,
        IIndexer<TMain, T>,
        IReadOnlyDictionary<TMain, T>,
        IEnumerable<Dictionary<TMain, T>.Enumerator, KeyValuePair<TMain, T>>,
        ICollection
    {
#region Fields

        private readonly Dictionary<TMain, T> _dictionaryImplementation;
        private readonly Dictionary<T, TMain> _dictionaryImplementation1;

#endregion

#region Properties

        public Dictionary<TMain, T>.KeyCollection Keys => _dictionaryImplementation.Keys;

        public Dictionary<TMain, T>.ValueCollection Values => _dictionaryImplementation.Values;

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_dictionaryImplementation).CopyTo(array, index);
        }

        public int Count => _dictionaryImplementation.Count;

        bool ICollection.IsSynchronized => ((ICollection)_dictionaryImplementation).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)_dictionaryImplementation).SyncRoot;

        public T this[TMain key]
        {
            get => _dictionaryImplementation[key];
            set
            {
                _dictionaryImplementation[key] = value;
                _dictionaryImplementation1[value] = key;
            }
        }

        public TMain this[T key]
        {
            get => _dictionaryImplementation1[key];
            set
            {
                _dictionaryImplementation1[key] = value;
                _dictionaryImplementation[value] = key;
            }
        }

        bool ICollection<KeyValuePair<TMain, T>>.IsReadOnly =>
            ((ICollection<KeyValuePair<TMain, T>>)_dictionaryImplementation).IsReadOnly;

        ICollection<T> IDictionary<TMain, T>.Values => _dictionaryImplementation.Values;

        ICollection<TMain> IDictionary<TMain, T>.Keys => _dictionaryImplementation.Keys;

        IEnumerable<T> IReadOnlyDictionary<TMain, T>.Values => Values;

        IEnumerable<TMain> IReadOnlyDictionary<TMain, T>.Keys => Keys;

#endregion

#region Lifecycle

        public DictionaryCross(int capacity)
        {
            _dictionaryImplementation = new Dictionary<TMain, T>(capacity);
            _dictionaryImplementation1 = new Dictionary<T, TMain>(capacity);
        }

        public DictionaryCross()
        {
            _dictionaryImplementation = new Dictionary<TMain, T>();
            _dictionaryImplementation1 = new Dictionary<T, TMain>();
        }

#endregion

        public void Add(KeyValuePair<T, TMain> item)
        {
            _dictionaryImplementation.Add(item.Value, item.Key);
            _dictionaryImplementation1.Add(item.Key, item.Value);
        }

        public void Add(T key, TMain value)
        {
            _dictionaryImplementation.Add(value, key);
            _dictionaryImplementation1.Add(key, value);
        }

        public void CopyTo(KeyValuePair<T, TMain>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<T, TMain>>)_dictionaryImplementation1).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<T, TMain> item)
        {
            _dictionaryImplementation.Remove(item.Value);
            return _dictionaryImplementation1.Remove(item.Key);
        }

        public bool Remove(T key)
        {
            if (!CollectionsExtensions.Remove(_dictionaryImplementation1, key, out TMain value))
            {
                return false;
            }

            _dictionaryImplementation.Remove(value);
            return _dictionaryImplementation1.Remove(key);
        }

        public bool ContainsKey(T key)
        {
            return _dictionaryImplementation1.ContainsKey(key);
        }

        public bool TryGetValue(T key, out TMain value)
        {
            return _dictionaryImplementation1.TryGetValue(key, out value);
        }

#region IDictionary<TMain,T>

        IEnumerator<KeyValuePair<TMain, T>> IEnumerable<KeyValuePair<TMain, T>>.GetEnumerator()
        {
            return _dictionaryImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionaryImplementation).GetEnumerator();
        }

        public void Add(KeyValuePair<TMain, T> item)
        {
            _dictionaryImplementation.Add(item.Key, item.Value);
            _dictionaryImplementation1.Add(item.Value, item.Key);
        }

        public void Add(TMain key, T value)
        {
            _dictionaryImplementation.Add(key, value);
            _dictionaryImplementation1.Add(value, key);
        }

        public void Clear()
        {
            _dictionaryImplementation.Clear();
            _dictionaryImplementation1.Clear();
        }

        public bool Contains(KeyValuePair<TMain, T> item)
        {
            return ((ICollection<KeyValuePair<TMain, T>>)_dictionaryImplementation).Contains(item);
        }

        public void CopyTo(KeyValuePair<TMain, T>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TMain, T>>)_dictionaryImplementation).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TMain, T> item)
        {
            _dictionaryImplementation1.Remove(item.Value);
            return _dictionaryImplementation.Remove(item.Key);
        }

        public bool Remove(TMain key)
        {
            if (!CollectionsExtensions.Remove(_dictionaryImplementation, key, out T value))
            {
                return false;
            }

            _dictionaryImplementation1.Remove(value);
            return true;
        }

        public bool ContainsKey(TMain key)
        {
            return _dictionaryImplementation.ContainsKey(key);
        }

        public bool TryGetValue(TMain key, out T value)
        {
            return _dictionaryImplementation.TryGetValue(key, out value);
        }

#endregion

#region IEnumerable<Dictionary<TMain,T>.Enumerator,KeyValuePair<TMain,T>>

        public Dictionary<TMain, T>.Enumerator GetEnumerator()
        {
            return _dictionaryImplementation.GetEnumerator();
        }

#endregion

        public void EnsureCapacity(int capacity)
        {
            _dictionaryImplementation.EnsureCapacity(capacity);
            _dictionaryImplementation1.EnsureCapacity(capacity);
        }
    }
}