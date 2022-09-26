using System.Collections.Generic;

namespace Dythervin.Collections
{
    public interface IReadOnlyHashList<T> : IReadOnlyList<T>
    {
        bool Contains(T item);
    }
}