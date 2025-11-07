namespace Yotei.ORM.Generators.Invariant;

// ========================================================
internal interface IChainTemplate<K, T>
{
    IChainTemplate<K, T> GetRange(int index, int count);
    IChainTemplate<K, T> Replace(int index, T item);
    IChainTemplate<K, T> Add(T item);
    IChainTemplate<K, T> AddRange(IEnumerable<T> range);
    IChainTemplate<K, T> Insert(int index, T item);
    IChainTemplate<K, T> InsertRange(int index, IEnumerable<T> range);
    IChainTemplate<K, T> RemoveAt(int index);
    IChainTemplate<K, T> RemoveRange(int index, int count);
    IChainTemplate<K, T> Remove(K key);
    IChainTemplate<K, T> RemoveLast(K key);
    IChainTemplate<K, T> RemoveAll(K key);
    IChainTemplate<K, T> Remove(Predicate<T> predicate);
    IChainTemplate<K, T> RemoveLast(Predicate<T> predicate);
    IChainTemplate<K, T> RemoveAll(Predicate<T> predicate);
    IChainTemplate<K, T> Clear();
}