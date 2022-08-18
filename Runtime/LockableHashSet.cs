using System.Collections.Generic;

namespace Dythervin.Collections
{
    public class LockableHashSet<T> : LockableCollection<HashSet<T>, T>
    {
        public HashSet<T>.Enumerator GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}