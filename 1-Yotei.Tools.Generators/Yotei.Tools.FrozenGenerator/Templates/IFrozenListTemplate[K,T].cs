namespace Yotei.Tools.FrozenGenerator.Internal;

// =========================================================
internal abstract class IFrozenListTemplate<K, T>
{
    public abstract IFrozenListTemplate<K, T> Reverse();
    public abstract IFrozenListTemplate<K, T> Sort(IComparer<K> comparer);
    public abstract IFrozenListTemplate<K, T> GetRange(int index, int count);
    public abstract IFrozenListTemplate<K, T> Replace(int index, T item);
    public abstract IFrozenListTemplate<K, T> Add(T item);
    public abstract IFrozenListTemplate<K, T> AddRange(IEnumerable<T> range);
    public abstract IFrozenListTemplate<K, T> Insert(int index, T item);
    public abstract IFrozenListTemplate<K, T> InsertRange(int index, IEnumerable<T> range);
    public abstract IFrozenListTemplate<K, T> RemoveAt(int index);
    public abstract IFrozenListTemplate<K, T> RemoveRange(int index, int count);
    public abstract IFrozenListTemplate<K, T> Remove(K key);
    public abstract IFrozenListTemplate<K, T> RemoveLast(K key);
    public abstract IFrozenListTemplate<K, T> RemoveAll(K key);
    public abstract IFrozenListTemplate<K, T> Remove(Predicate<T> predicate);
    public abstract IFrozenListTemplate<K, T> RemoveLast(Predicate<T> predicate);
    public abstract IFrozenListTemplate<K, T> RemoveAll(Predicate<T> predicate);
    public abstract IFrozenListTemplate<K, T> Clear();
}