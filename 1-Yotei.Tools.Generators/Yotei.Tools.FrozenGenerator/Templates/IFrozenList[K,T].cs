namespace Yotei.Tools.FrozenGenerator.Templates;

// =========================================================
/// <summary>
/// Template for the <see cref="IFrozenList{K, T}"/> interface.
/// </summary>
internal partial interface IFrozenList<K, T>
{
    IFrozenList<K, T> GetRange(int index, int count);
    IFrozenList<K, T> Replace(int index, T item);
    IFrozenList<K, T> Add(T item);
    IFrozenList<K, T> AddRange(IEnumerable<T> range);
    IFrozenList<K, T> Insert(int index, T item);
    IFrozenList<K, T> InsertRange(int index, IEnumerable<T> range);
    IFrozenList<K, T> RemoveAt(int index);
    IFrozenList<K, T> RemoveRange(int index, int count);
    IFrozenList<K, T> Remove(K key);
    IFrozenList<K, T> RemoveLast(K key);
    IFrozenList<K, T> RemoveAll(K key);
    IFrozenList<K, T> Remove(Predicate<T> predicate);
    IFrozenList<K, T> RemoveLast(Predicate<T> predicate);
    IFrozenList<K, T> RemoveAll(Predicate<T> predicate);
    IFrozenList<K, T> Clear();
}