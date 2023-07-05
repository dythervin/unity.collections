using System.Collections.Generic;

namespace Dythervin.Collections
{
    public class LockableList<T> : LockableCollection<List<T>, T, List<T>.Enumerator>
    {
        protected override List<T>.Enumerator GetContainerEnumerator()
        {
            return container.GetEnumerator();
        }
    }
}