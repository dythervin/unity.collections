using System;
using System.Collections.Generic;
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
}