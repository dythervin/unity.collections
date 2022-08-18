using System;
using System.Collections;
using System.Collections.Generic;

namespace Dythervin.Collections
{
    public class HashList<T> : ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IEnumerable, IList,
        IIndexer<int, T>
    {
        private readonly List<T> _list;
        private readonly HashSet<T> _set;

        public HashList(IEnumerable<T> enumerable)
        {
            _list = new List<T>(enumerable);
            _set = new HashSet<T>(_list);
        }

        public HashList(int capacity)
        {
            _list = new List<T>(capacity);
            _set = new HashSet<T>(
#if UNITY_2021_3_OR_NEWER
                    capacity
#endif
            );
        }

        private HashList()
        {
            _list = new List<T>();
            _set = new HashSet<T>();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public void Add(T item)
        {
            if (_set.Add(item))
                _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
            _set.Clear();
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (!_set.Remove(item))
                return false;

            _list.Remove(item);
            return true;
        }

        public int Count => _list.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        int IList.Add(object value)
        {
            return ((IList)_list).Add(value);
        }

        bool IList.Contains(object value)
        {
            return _set.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_list).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ((IList)_list).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            ((IList)_list).Remove(value);
        }

        object IList.this[int index]
        {
            get => ((IList)_list)[index];
            set => ((IList)_list)[index] = value;
        }

        bool IList.IsFixedSize => ((IList)_list).IsFixedSize;

        bool IList.IsReadOnly => ((IList)_list).IsReadOnly;

        public int IndexOf(T item)
        {
            if (!_set.Contains(item))
                return -1;
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (_set.Add(item))
                _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            T item = _list[index];
            _set.Remove(item);
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public List<T>.Enumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}