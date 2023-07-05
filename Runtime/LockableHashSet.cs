using System.Collections.Generic;

namespace Dythervin.Collections
{
    public class LockableHashSet<T> : LockableCollection<HashSet<T>, T,  HashSet<T>.Enumerator>
    {
        protected override HashSet<T>.Enumerator GetContainerEnumerator()
        {
            return container.GetEnumerator();
        }
    }
}