namespace Yotei.ORM.Generators.Invariant;

// ========================================================
internal interface IChainTemplate<T>
{
    IChainTemplate<T> GetRange(int index, int count);
    IChainTemplate<T> Replace(int index, T item);
    IChainTemplate<T> Add(T item);
    IChainTemplate<T> AddRange(IEnumerable<T> range);
    IChainTemplate<T> Insert(int index, T item);
    IChainTemplate<T> InsertRange(int index, IEnumerable<T> range);
    IChainTemplate<T> RemoveAt(int index);
    IChainTemplate<T> RemoveRange(int index, int count);
    IChainTemplate<T> Remove(T item);
    IChainTemplate<T> RemoveLast(T item);
    IChainTemplate<T> RemoveAll(T item);
    IChainTemplate<T> Remove(Predicate<T> predicate);
    IChainTemplate<T> RemoveLast(Predicate<T> predicate);
    IChainTemplate<T> RemoveAll(Predicate<T> predicate);
    IChainTemplate<T> Clear();
}