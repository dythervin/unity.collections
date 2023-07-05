using System.Collections.Generic;

namespace Dythervin.Collections
{
    public interface IReadOnlyHashList<T> : IReadOnlyList<T>
    {
        IReadOnlyList<T> InnerList { get; }

        bool Contains(T item);

        new List<T>.Enumerator GetEnumerator();
    }
}