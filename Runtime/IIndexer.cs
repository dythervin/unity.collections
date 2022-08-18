namespace Dythervin.Collections
{
    public interface IIndexer<in TKey, TValue> : IIndexerGetter<TKey, TValue>, IIndexerSetter<TKey, TValue> { }

    public interface IIndexerGetter<in TKey, out TValue>
    {
        TValue this[TKey key] { get; }
    }

    public interface IIndexerSetter<in TKey, in TValue>
    {
        TValue this[TKey key] { set; }
    }
}