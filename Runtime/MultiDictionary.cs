using System.Collections.Generic;
using Dythervin.Core.Extensions;
using Dythervin.ObjectPool;
using UnityEngine.Assertions;

namespace Dythervin.Collections
{
    public class MultiDictionary<TKey, TValue, TCollection> : Dictionary<TKey, TCollection>,
        IMultiDictionary<TKey, TValue>
        where TCollection : class, ICollection<TValue>, new()
    {
        private static IObjectPool<TCollection> Pool => SharedPool<TCollection>.Instance;

        public bool Add(TKey key, TValue value, bool allowMultiple = true)
        {
            if (TryGetValue(key, out TCollection list))
            {
                Assert.IsTrue(allowMultiple || list.Count == 0);
                return list.AddB(value);
            }

            TCollection collection = Pool.Get();
            collection.Clear();
            if (collection.AddB(value))
            {
                Add(key, collection);
                return true;
            }

            Pool.Release(ref collection);
            return false;
        }

        public bool Remove(TKey key, TValue value, bool allowMultiple = true)
        {
            if (!TryGetValue(key, out TCollection collection))
                return false;

            Assert.IsTrue(allowMultiple || collection.Count == 1);

            if (collection.Remove(value))
            {
                if (collection.Count == 0)
                {
                    Pool.Release(ref collection);
                    Remove(key);
                }

                return true;
            }

            return false;
        }
    }

    public interface IMultiDictionary<in TKey, in TValue>
    {
        bool Add(TKey key, TValue value, bool allowMultiple = true);

        bool Remove(TKey key, TValue value, bool allowMultiple = true);
    }

    public interface IMultiDictionary<TKey, in TValue, TCollection> : IDictionary<TKey, TCollection>,
        IMultiDictionary<TKey, TCollection>
        where TCollection : class, ICollection<TValue>, new()
    {
    }
}