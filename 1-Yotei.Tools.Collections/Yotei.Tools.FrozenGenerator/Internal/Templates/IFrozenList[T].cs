namespace Yotei.Tools.FrozenGenerator.Templates;

// =========================================================
/// <summary>
/// Template for the <see cref="IFrozenList{T}"/> interface.
/// </summary>
internal partial interface IFrozenList<T>
{
    IFrozenList<T> Reduce(int index, int count);
    IFrozenList<T> Replace(int index, T item);
    IFrozenList<T> Add(T item);
    IFrozenList<T> AddRange(IEnumerable<T> range);
    IFrozenList<T> Insert(int index, T item);
    IFrozenList<T> InsertRange(int index, IEnumerable<T> range);
    IFrozenList<T> RemoveAt(int index);
    IFrozenList<T> RemoveRange(int index, int count);
    IFrozenList<T> Remove(T item);
    IFrozenList<T> RemoveLast(T item);
    IFrozenList<T> RemoveAll(T item);
    IFrozenList<T> Remove(Predicate<T> predicate);
    IFrozenList<T> RemoveLast(Predicate<T> predicate);
    IFrozenList<T> RemoveAll(Predicate<T> predicate);
    IFrozenList<T> Clear();
}