using System;

namespace Dythervin.Collections
{
    public interface ICollectionExt
    {
        event Action OnChanged;
    }

    public interface ICollectionExt<T> : ICollectionExt
    {
        event Action<T> OnAdded;

        event Action<T> OnRemoved;
    }
}