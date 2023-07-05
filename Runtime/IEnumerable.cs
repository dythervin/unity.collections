using System.Collections.Generic;

namespace Dythervin.Collections
{
    public interface IEnumerable<out TEnumerator, out T> where TEnumerator : IEnumerator<T>
    {
        TEnumerator GetEnumerator();
    }
}