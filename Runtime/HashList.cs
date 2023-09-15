using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Dythervin.Collections
{
    public class HashList<T> : ListExt<T>, IReadOnlyHashList<T>
    {
#region Fields

        private readonly HashSet<T> _set;

#endregion

#region Lifecycle

        public HashList(IEnumerable<T> enumerable) : base(enumerable)
        {
            _set = new HashSet<T>(InnerList);
            Assert.IsTrue(_set.Count == InnerList.Count);
        }

        public HashList(int capacity = 16) : base(capacity)
        {
            _set = new HashSet<T>(
#if UNITY_2021_1_OR_NEWER
                capacity
#endif
            );
        }

        public HashList()
        {
            _set = new HashSet<T>();
        }

#endregion

        public override bool Remove(T item)
        {
            return _set.Remove(item) && base.Remove(item);
        }

        public override int IndexOf(T item)
        {
            if (!_set.Contains(item))
            {
                return -1;
            }

            return base.IndexOf(item);
        }

        public override void Insert(int index, T item)
        {
            if (_set.Add(item))
            {
                base.Insert(index, item);
            }
        }

        protected override bool AddInner(T item)
        {
            return _set.Add(item) && base.AddInner(item);
        }

        protected override void ReplaceAtInner(int index, T value, T popped)
        {
            base.ReplaceAtInner(index, value, popped);
            _set.Remove(popped);
            _set.Add(value);
        }

        protected override void ClearInner()
        {
            base.ClearInner();
            _set.Clear();
        }

        protected override void RemoveAtInner(int index, T item)
        {
            _set.Remove(item);
            base.RemoveAtInner(index, item);
        }

#region IReadOnlyHashList<T>

        public override bool Contains(T item)
        {
            return _set.Contains(item);
        }

#endregion

        public override void EnsureCapacity(int capacity)
        {
            base.EnsureCapacity(capacity);
            _set.EnsureCapacity(capacity);
        }
    }
}