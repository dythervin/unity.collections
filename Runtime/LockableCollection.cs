using System;
using System.Collections;
using System.Collections.Generic;
using Dythervin.Core.Lockables;
#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Dythervin.Collections
{
    public interface IReadOnlyLockableCollection<T> : IReadOnlyCollection<T>, ILockable
    {
        bool Contains(T value);

        bool IsToRemove(T value);
    }

    public interface ILockableCollection<T> : IReadOnlyLockableCollection<T>, ILockableSimple
    {
        void Clear();

        void Add(T value);

        void Remove(T value);
    }

    public interface ILockableCollection<T, TEnumerator> : ILockableCollection<T>
        where TEnumerator : IEnumerator<T>
    {
        new LockableCollectionBase<T, TEnumerator>.Enumerator GetEnumerator();
    }

    public abstract class LockableCollectionBase<T, TEnumerator> : ILockableCollection<T, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
        [Serializable]
        public struct Enumerator : IEnumerator<T>
        {
            private LockableCollectionBase<T, TEnumerator> _collection;
            private TEnumerator _enumerator;
            private T _current;
            private bool _isCurrentValid;

            public T Current => _current;

            object IEnumerator.Current
            {
                get
                {
                    if (!_isCurrentValid)
                    {
                        throw new InvalidOperationException();
                    }

                    return Current;
                }
            }

            internal Enumerator(LockableCollectionBase<T, TEnumerator> collection)
            {
                _collection = collection;

                _current = default;
                _isCurrentValid = false;
                _enumerator = _collection.GetContainerEnumerator();
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    _current = _enumerator.Current;
                    if (_collection.IsToRemove(_current))
                    {
                        continue;
                    }

                    _isCurrentValid = true;
                    return true;
                }

                _current = default;
                _isCurrentValid = false;
                return false;
            }

            void IEnumerator.Reset()
            {
                _current = default;
                _isCurrentValid = false;
                _enumerator = _collection.GetContainerEnumerator();
            }
        }

        public event Action OnChangesApplied;

        public event Action OnLockChanged;

#if ODIN_INSPECTOR && UNITY_EDITOR

        [ShowInInspector]
        [ReadOnly]
#endif
        public bool IsLocked { get; private set; }

        public abstract int Count { get; }

        protected abstract void ContainerAdd(T value);

        protected abstract void ContainerRemove(T value);

        protected abstract void AddToBuffer(T value, bool toAdd);

        protected abstract bool TryGetBufferValue(T value, out bool toAdd);

        protected abstract bool TryApplyBuffer();

        protected abstract TEnumerator GetContainerEnumerator();

        private void ApplyBufferInner()
        {
            if (TryApplyBuffer())
            {
                OnChangesApplied?.Invoke();
            }
        }

        public abstract bool Contains(T value);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract void Clear();

        public void Add(T value)
        {
            if (IsLocked)
            {
                AddToBuffer(value, true);
            }
            else
            {
                ContainerAdd(value);
            }
        }

        public bool IsToRemove(T value)
        {
            return TryGetBufferValue(value, out bool add) && !add;
        }

        public ILockableSimple SetLock(bool isLocked)
        {
            if (isLocked == IsLocked)
            {
                return this;
            }

            IsLocked = isLocked;
            if (!isLocked)
            {
                ApplyBufferInner();
            }

            OnLockChanged?.Invoke();
            return this;
        }

        public void Remove(T value)
        {
            if (!IsLocked)
            {
                ContainerRemove(value);
                return;
            }

            AddToBuffer(value, false);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
    }

    public abstract class LockableCollection<T, TEnumerator> : LockableCollectionBase<T, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
#if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector]
        [ReadOnly]
#endif
        private readonly Dictionary<T, bool> _buffer = new();

        public override void Clear()
        {
            _buffer.Clear();
        }

        protected override bool TryGetBufferValue(T value, out bool toAdd)
        {
            return _buffer.TryGetValue(value, out toAdd);
        }

        protected override void AddToBuffer(T value, bool toAdd)
        {
            _buffer[value] = toAdd;
        }

        protected override bool TryApplyBuffer()
        {
            if (_buffer.Count == 0)
            {
                return false;
            }

            foreach (var pair in _buffer)
            {
                if (pair.Value)
                {
                    ContainerAdd(pair.Key);
                }
                else
                {
                    ContainerRemove(pair.Key);
                }
            }

            _buffer.Clear();
            return true;
        }
    }

    public abstract class LockableCollection<TContainer, T, TEnumerator> : LockableCollection<T, TEnumerator>, IReadOnlyCollection<T>
        where TContainer : ICollection<T>, new()
        where TEnumerator : IEnumerator<T>
    {
#if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector]
        [ReadOnly]
#endif
        public readonly TContainer container = new();

        public override int Count => container.Count;

        public override bool Contains(T value)
        {
            return container.Contains(value);
        }

        protected override void ContainerAdd(T value)
        {
            container.Add(value);
        }

        protected override void ContainerRemove(T value)
        {
            container.Remove(value);
        }
    }
}