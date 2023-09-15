using System.Collections.Generic;
#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Dythervin.Collections
{
    public class LockableDictionary<TKey, TValue> : LockableCollectionBase<KeyValuePair<TKey, TValue>,
        Dictionary<TKey, TValue>.Enumerator>
    {
#region Fields

#if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector]
        [ReadOnly]
#endif
        public readonly Dictionary<TKey, TValue> container = new();

        private readonly Dictionary<TKey, (TValue value, bool toAdd)> _buffer = new();

#endregion

#region Properties

        public override int Count => container.Count;

#endregion

        public override void Clear()
        {
            _buffer.Clear();
            container.Clear();
        }

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Remove(TKey value)
        {
            Remove(new KeyValuePair<TKey, TValue>(value, default));
        }

        public override bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            return container.TryGetValue(pair.Key, out TValue value) &&
                   EqualityComparer<TValue>.Default.Equals(value, pair.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return container.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return container.TryGetValue(key, out value);
        }

        protected override void ContainerAdd(KeyValuePair<TKey, TValue> value)
        {
            container[value.Key] = value.Value;
        }

        protected override void ContainerRemove(KeyValuePair<TKey, TValue> value)
        {
            container.Remove(value.Key);
        }

        protected override void AddToBuffer(KeyValuePair<TKey, TValue> value, bool toAdd)
        {
            _buffer[value.Key] = (value.Value, toAdd);
        }

        protected override bool TryGetBufferValue(KeyValuePair<TKey, TValue> value, out bool toAdd)
        {
            if (_buffer.TryGetValue(value.Key, out (TValue value, bool toAdd) data))
            {
                toAdd = data.toAdd;
                return true;
            }

            toAdd = false;
            return false;
        }

        protected override bool TryApplyBuffer()
        {
            if (_buffer.Count == 0)
            {
                return false;
            }

            foreach (var pair in _buffer)
            {
                var data = new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.value);
                if (pair.Value.toAdd)
                {
                    ContainerAdd(data);
                }
                else
                {
                    ContainerRemove(data);
                }
            }

            _buffer.Clear();
            return true;
        }

        protected override Dictionary<TKey, TValue>.Enumerator GetContainerEnumerator()
        {
            return container.GetEnumerator();
        }
    }
}