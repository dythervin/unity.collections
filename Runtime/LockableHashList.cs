using System.Collections.Generic;


namespace Dythervin.Collections
{
    public class LockableHashList<T> : LockableCollection<HashList<T>, T, List<T>.Enumerator>
    {
        protected override List<T>.Enumerator GetContainerEnumerator()
        {
            return container.GetEnumerator();
        }
    }
}