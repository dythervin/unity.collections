using System;
using System.Collections;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Dythervin.Collections
{
    public class LockableCollection<TContainer, T> : ICollection<T>, IReadOnlyCollection<T>
        where TContainer : ICollection<T>, new()
    {
#if ODIN_INSPECTOR
        [ShowInInspector]
        [ReadOnly]
#endif
        private readonly Dictionary<T, bool> _buffer = new Dictionary<T, bool>();
#if ODIN_INSPECTOR
        [ShowInInspector]
        [ReadOnly]
#endif
        public readonly TContainer values = new TContainer();

        public bool IsLocked { get; private set; }

        void ICollection<T>.Add(T value)
        {
            Add(value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            values.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T value)
        {
            return Remove(value);
        }

        public int Count => values.Count;
        public bool IsReadOnly => values.IsReadOnly;

        public bool Contains(T value)
        {
            return values.Contains(value);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)values).GetEnumerator();
        }

        public void Clear()
        {
            values.Clear();
            _buffer.Clear();
        }

        public void Add(T value)
        {
            if (IsLocked)
                _buffer[value] = true;
            else
                values.Add(value);
        }

        public bool IsToRemove(T value)
        {
            return _buffer.TryGetValue(value, out bool data) && data;
        }

        public void Lock(bool value = true)
        {
            if (value == IsLocked)
                return;

            IsLocked = value;
            if (!value)
                ApplyChanges();
        }

        public event Action OnChangesApplied;

        public bool Remove(T value)
        {
            if (!IsLocked)
                return values.Remove(value);

            _buffer[value] = false;
            return false;
        }

        public void Unlock()
        {
            Lock(false);
        }

        private void ApplyChanges()
        {
            if (_buffer.Count == 0)
                return;

            foreach (var pair in _buffer)
            {
                if (pair.Value)
                    values.Add(pair.Key);
                else
                    values.Remove(pair.Key);
            }

            _buffer.Clear();
            OnChangesApplied?.Invoke();
        }
    }
}