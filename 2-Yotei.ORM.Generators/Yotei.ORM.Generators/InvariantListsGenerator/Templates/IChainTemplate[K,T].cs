namespace Yotei.Tools.Generators;

// ========================================================
internal abstract class IChainTemplate<K, T>
{
    public abstract IChainTemplate<K, T> GetRange(int index, int count);
    public abstract IChainTemplate<K, T> Replace(int index, T item);
    public abstract IChainTemplate<K, T> Add(T item);
    public abstract IChainTemplate<K, T> AddRange(IEnumerable<T> range);
    public abstract IChainTemplate<K, T> Insert(int index, T item);
    public abstract IChainTemplate<K, T> InsertRange(int index, IEnumerable<T> range);
    public abstract IChainTemplate<K, T> RemoveAt(int index);
    public abstract IChainTemplate<K, T> RemoveRange(int index, int count);
    public abstract IChainTemplate<K, T> Remove(K key);
    public abstract IChainTemplate<K, T> RemoveLast(K key);
    public abstract IChainTemplate<K, T> RemoveAll(K key);
    public abstract IChainTemplate<K, T> Remove(Predicate<T> predicate);
    public abstract IChainTemplate<K, T> RemoveLast(Predicate<T> predicate);
    public abstract IChainTemplate<K, T> RemoveAll(Predicate<T> predicate);
    public abstract IChainTemplate<K, T> Clear();
}