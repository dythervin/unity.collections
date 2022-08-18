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
        [ShowInInspector] [ReadOnly]
#endif
        public readonly TContainer values = new TContainer();

        private bool _isLocked;

#if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
#endif
        private readonly HashSet<T> _toAdd = new HashSet<T>();

#if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
#endif
        private readonly HashSet<T> _toRemove = new HashSet<T>();

        public event Action OnChangesApplied;

        public bool IsLocked => _isLocked;


        public void Unlock() => Lock(false);

        public void Lock(bool value = true)
        {
            if (value == _isLocked)
                return;

            _isLocked = value;
            if (!value)
                ApplyChanges();
        }

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

        public int Count => values.Count + _toAdd.Count - _toRemove.Count;
        public bool IsReadOnly => values.IsReadOnly;

        public bool Contains(T value)
        {
            return !_toRemove.Contains(value)
                   && (values.Contains(value) || _toAdd.Contains(value));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)values).GetEnumerator();
        }

        public bool ToRemove(T value)
        {
            return _toRemove.Contains(value);
        }

        public void Add(T value)
        {
            if (_isLocked)
                AddBuffer(value);
            else
                values.Add(value);
        }

        public bool Remove(T value)
        {
            return _isLocked ? RemoveBuffer(value) : values.Remove(value);
        }

        public void Clear()
        {
            values.Clear();
            _toAdd.Clear();
            _toRemove.Clear();
        }

        private void ApplyChanges()
        {
            if (_toRemove.Count == 0 && _toAdd.Count == 0)
                return;

            if (_toRemove.Count > 0)
            {
                foreach (T prioritized in _toRemove)
                {
                    values.Remove(prioritized);
                }

                _toRemove.Clear();
            }

            if (_toAdd.Count > 0)
            {
                foreach (T prioritized in _toAdd)
                {
                    values.Add(prioritized);
                }

                _toAdd.Clear();
            }

            OnChangesApplied?.Invoke();
        }

        private void AddBuffer(T value)
        {
            if (_toRemove.Remove(value))
                return;

            _toAdd.Add(value);
        }

        private bool RemoveBuffer(T value)
        {
            return _toAdd.Remove(value) || _toRemove.Add(value);
        }
    }
}