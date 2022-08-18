using System.Collections;
using System.Collections.Generic;
using Dythervin.Core.Extensions;

namespace Dythervin.Collections
{
    public class DictionaryCross<TMain, T> : IDictionary<TMain, T>, IIndexer<TMain, T>
    {
        private readonly Dictionary<TMain, T> _dictionaryImplementation;
        private readonly Dictionary<T, TMain> _dictionaryImplementation1;

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

        public TMain this[T key]
        {
            get => _dictionaryImplementation1[key];
            set => _dictionaryImplementation1[key] = value;
        }

        public T this[TMain key]
        {
            get => _dictionaryImplementation[key];
            set => _dictionaryImplementation[key] = value;
        }

        public int Count => _dictionaryImplementation.Count;

        bool ICollection<KeyValuePair<TMain, T>>.IsReadOnly => ((ICollection<KeyValuePair<TMain, T>>)_dictionaryImplementation).IsReadOnly;

        public ICollection<TMain> Keys => _dictionaryImplementation.Keys;

        public ICollection<T> Values => _dictionaryImplementation.Values;

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
            if (!_dictionaryImplementation.TryPop(key, out T value))
                return false;

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

        public Dictionary<TMain, T>.Enumerator GetEnumerator()
        {
            return _dictionaryImplementation.GetEnumerator();
        }

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
            if (!_dictionaryImplementation1.TryPop(key, out TMain value))
                return false;

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
    }
}