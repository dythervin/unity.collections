using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Dythervin.Core.Extensions;

namespace Dythervin.Collections
{
    public class ListExt<T> : IList<T>, IList, IIndexer<int, T>, ICollectionExt<T>
    {
#region Fields

        public event Action<T> OnAdded;

        public event Action<T> OnRemoved;

        public event Action OnChanged;

        private readonly List<T> _list;

#endregion

#region Properties

        public int Count => _list.Count;

        public IReadOnlyList<T> InnerList => _list;

        public virtual T this[int index]
        {
            get => _list[index];
            set => ReplaceAt(index, value, out _);
        }

        bool IList.IsFixedSize => ((IList)_list).IsFixedSize;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        bool IList.IsReadOnly => ((IList)_list).IsReadOnly;

        bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

#endregion

#region Lifecycle

        public ListExt(IEnumerable<T> enumerable)
        {
            _list = new List<T>(enumerable);
        }

        public ListExt(int capacity = 16)
        {
            _list = new List<T>(capacity);
        }

        public ListExt()
        {
            _list = new List<T>();
        }

#endregion

        public bool ReplaceAt(int index, T value, out T popped)
        {
            popped = _list[index];
            if (EqualityComparer<T>.Default.Equals(popped, value))
            {
                popped = default;
                return false;
            }

            ReplaceAtInner(index, value, popped);
            OnRemoved?.Invoke(popped);
            OnAdded?.Invoke(value);
            OnChanged?.Invoke();
            return true;
        }

        public bool Add(T item)
        {
            if (!AddInner(item))
                return false;

            OnAdded?.Invoke(item);
            OnChanged?.Invoke();
            return true;
        }

        protected virtual bool AddInner(T item)
        {
            _list.Add(item);

            OnAdded?.Invoke(item);
            OnChanged?.Invoke();
            return true;
        }

        public virtual List<T>.Enumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        protected virtual void ReplaceAtInner(int index, T value, T popped)
        {
            _list[index] = value;
        }

        protected virtual void ClearInner()
        {
            _list.Clear();
        }

        protected virtual void InsertInner(int index, T item)
        {
            _list.Insert(index, item);
        }

        protected virtual void RemoveAtInner(int index, T item)
        {
            _list.RemoveAt(index);
        }

        public void Sort()
        {
            _list.Sort();
        }

        public void Sort(IComparer<T> comparer)
        {
            _list.Sort(comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            _list.Sort(comparison);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            _list.Sort(index, count, comparer);
        }

#region IList

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        int IList.Add(object value)
        {
            if (AddInner((T)value))
                return Count - 1;

            return -1;
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

#endregion

#region IList<T>

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            AddInner(item);
        }

        public virtual void Clear()
        {
            int count = Count;
            if (count == 0)
                return;

            var temp = ArrayPool<T>.Shared.Rent(_list.Count);
            try
            {
                for (int i = 0; i < count; i++)
                    temp[i] = _list[i];

                ClearInner();
                if (OnRemoved != null)
                    for (int i = 0; i < count; i++)
                        OnRemoved?.Invoke(temp[i]);

                OnChanged?.Invoke();
            }
            finally
            {
                ArrayPool<T>.Shared.Return(temp, true);
            }
        }

        public virtual bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public virtual int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            InsertInner(index, item);
            OnAdded?.Invoke(item);
            OnChanged?.Invoke();
        }

        public void RemoveAt(int index)
        {
            T item = _list[index];
            RemoveAtInner(index, item);
            OnRemoved?.Invoke(item);
            OnChanged?.Invoke();
        }

#endregion

        public virtual void EnsureCapacity(int capacity)
        {
            _list.EnsureCapacity(capacity);
        }
    }
}