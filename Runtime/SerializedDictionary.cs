using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dythervin.Collections
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>,
        ISerializationCallbackReceiver,
        IIndexer<TKey, TValue>,
        IEnumerable<Dictionary<TKey, TValue>.Enumerator, KeyValuePair<TKey, TValue>>
    {
        [SerializeField]
        [HideInInspector]
        protected TKey[] mKeys = Array.Empty<TKey>();

        [SerializeField]
        [HideInInspector]
        protected TValue[] mValues = Array.Empty<TValue>();

        public void OnBeforeSerialize()
        {
            mKeys = new TKey[Count];
            mValues = new TValue[Count];

            int i = 0;
            foreach (var pair in this)
            {
                mKeys[i] = pair.Key;
                mValues[i++] = pair.Value;
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
#if UNITY_2021_3_OR_NEWER
            EnsureCapacity(mKeys.Length);
#endif
            for (int i = 0; i < mKeys.Length; i++)
            {
                Add(mKeys[i], mValues[i]);
            }

            mKeys = null;
            mValues = null;
        }
    }

    [Serializable]
    public class SerializedDictionaryWrapped<TKey, TValue> : ISerializationCallbackReceiver,
        IDictionary<TKey, TValue>,
        IIndexer<TKey, TValue>,
        IEnumerable<Dictionary<TKey, TValue>.Enumerator, KeyValuePair<TKey, TValue>>
    {
        public readonly Dictionary<TKey, TValue> dictionary = new();

        private IDictionary<TKey, TValue> Dictionary => dictionary;

        [SerializeField]
        [ReadOnly]
        protected TKey[] mKeys = Array.Empty<TKey>();

        [SerializeField]
        [ReadOnly]
        protected TValue[] mValues = Array.Empty<TValue>();

        public void OnBeforeSerialize()
        {
            mKeys = new TKey[dictionary.Count];
            mValues = new TValue[dictionary.Count];

            int i = 0;
            foreach (var pair in dictionary)
            {
                mKeys[i] = pair.Key;
                mValues[i++] = pair.Value;
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();
#if UNITY_2021_3_OR_NEWER
            dictionary.EnsureCapacity(mKeys.Length);
#endif
            for (int i = 0; i < mKeys.Length; i++)
            {
                dictionary.Add(mKeys[i], mValues[i]);
            }

            mKeys = null;
            mValues = null;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dictionary).GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Dictionary.Add(item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Remove(item);
        }

        public int Count => dictionary.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => Dictionary.IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => dictionary.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => dictionary.Values;

        public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;

        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;
    }
}