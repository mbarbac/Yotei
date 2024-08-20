namespace Yotei.Tools.FrozenGenerator.Templates;

// =========================================================
/// <summary>
/// Template for the <see cref="FrozenList{K, T}"/> class.
/// </summary>
internal class FrozenList<K, T>
{
    public virtual FrozenList<K, T> GetRange(int index, int count) => throw new NotImplementedException();
    public virtual FrozenList<K, T> Replace(int index, T item) => throw new NotImplementedException();
    public virtual FrozenList<K, T> Add(T item) => throw new NotImplementedException();
    public virtual FrozenList<K, T> AddRange(IEnumerable<T> range) => throw new NotImplementedException();
    public virtual FrozenList<K, T> Insert(int index, T item) => throw new NotImplementedException();
    public virtual FrozenList<K, T> InsertRange(int index, IEnumerable<T> range) => throw new NotImplementedException();
    public virtual FrozenList<K, T> RemoveAt(int index) => throw new NotImplementedException();
    public virtual FrozenList<K, T> RemoveRange(int index, int count) => throw new NotImplementedException();
    public virtual FrozenList<K, T> Remove(K key) => throw new NotImplementedException();
    public virtual FrozenList<K, T> RemoveLast(K key) => throw new NotImplementedException();
    public virtual FrozenList<K, T> RemoveAll(K key) => throw new NotImplementedException();
    public virtual FrozenList<K, T> Remove(Predicate<T> predicate) => throw new NotImplementedException();
    public virtual FrozenList<K, T> RemoveLast(Predicate<T> predicate) => throw new NotImplementedException();
    public virtual FrozenList<K, T> RemoveAll(Predicate<T> predicate) => throw new NotImplementedException();
    public virtual FrozenList<K, T> Clear() => throw new NotImplementedException();
}