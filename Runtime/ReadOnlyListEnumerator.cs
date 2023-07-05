using System;
using System.Collections;
using System.Collections.Generic;

namespace Dythervin.Collections
{
    public static class ReadOnlyListEnumeratorExtension
    {
        public static ReadOnlyListEnumeratorWrapper<T> ToEnumerable<T>(this IReadOnlyList<T> list)
        {
            return new ReadOnlyListEnumeratorWrapper<T>(list);
        }
    }
    
    
    public readonly struct ReadOnlyListEnumeratorWrapper<T>
    {
        private readonly IReadOnlyList<T> _list;

        public ReadOnlyListEnumeratorWrapper(IReadOnlyList<T> list)
        {
            _list = list;
        }

        public ReadOnlyListEnumerator<T> GetEnumerator()
        {
            return new ReadOnlyListEnumerator<T>(_list);
        }
    }

    public struct ReadOnlyListEnumerator<T> : IEnumerator<T>
    {
        private IReadOnlyList<T> _list;
        private int _index;


        public ReadOnlyListEnumerator(IReadOnlyList<T> list)
        {
            _list = list;
            _index = -1;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _list.Count;
        }

        public void Reset()
        {
            _index = -1;
        }

        public T Current
        {
            get
            {
                if (_index < 0 || _index > _list.Count)
                    throw new ArgumentOutOfRangeException(nameof(_index));

                return _list[_index];
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _list = default;
        }
    }
}