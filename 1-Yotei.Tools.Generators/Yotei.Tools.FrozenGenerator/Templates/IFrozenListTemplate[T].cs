namespace Yotei.Tools.FrozenGenerator.Internal;

// =========================================================
internal abstract class IFrozenListTemplate<T>
{
    public abstract IFrozenListTemplate<T> Reverse();
    public abstract IFrozenListTemplate<T> Sort(IComparer<T> comparer);
    public abstract IFrozenListTemplate<T> GetRange(int index, int count);
    public abstract IFrozenListTemplate<T> Replace(int index, T item);
    public abstract IFrozenListTemplate<T> Add(T item);
    public abstract IFrozenListTemplate<T> AddRange(IEnumerable<T> range);
    public abstract IFrozenListTemplate<T> Insert(int index, T item);
    public abstract IFrozenListTemplate<T> InsertRange(int index, IEnumerable<T> range);
    public abstract IFrozenListTemplate<T> RemoveAt(int index);
    public abstract IFrozenListTemplate<T> RemoveRange(int index, int count);
    public abstract IFrozenListTemplate<T> Remove(T item);
    public abstract IFrozenListTemplate<T> RemoveLast(T item);
    public abstract IFrozenListTemplate<T> RemoveAll(T item);
    public abstract IFrozenListTemplate<T> Remove(Predicate<T> predicate);
    public abstract IFrozenListTemplate<T> RemoveLast(Predicate<T> predicate);
    public abstract IFrozenListTemplate<T> RemoveAll(Predicate<T> predicate);
    public abstract IFrozenListTemplate<T> Clear();
}