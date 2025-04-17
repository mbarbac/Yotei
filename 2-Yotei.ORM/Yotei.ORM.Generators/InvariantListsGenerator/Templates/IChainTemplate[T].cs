namespace Yotei.ORM.Generators;

// ========================================================
internal abstract class IChainTemplate<T>
{
    public abstract IChainTemplate<T> GetRange(int index, int count);
    public abstract IChainTemplate<T> Replace(int index, T item);
    public abstract IChainTemplate<T> Add(T item);
    public abstract IChainTemplate<T> AddRange(IEnumerable<T> range);
    public abstract IChainTemplate<T> Insert(int index, T item);
    public abstract IChainTemplate<T> InsertRange(int index, IEnumerable<T> range);
    public abstract IChainTemplate<T> RemoveAt(int index);
    public abstract IChainTemplate<T> RemoveRange(int index, int count);
    public abstract IChainTemplate<T> Remove(T item);
    public abstract IChainTemplate<T> RemoveLast(T item);
    public abstract IChainTemplate<T> RemoveAll(T item);
    public abstract IChainTemplate<T> Remove(Predicate<T> predicate);
    public abstract IChainTemplate<T> RemoveLast(Predicate<T> predicate);
    public abstract IChainTemplate<T> RemoveAll(Predicate<T> predicate);
    public abstract IChainTemplate<T> Clear();
}