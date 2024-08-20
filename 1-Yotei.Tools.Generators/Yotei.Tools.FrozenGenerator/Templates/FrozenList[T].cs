namespace Yotei.Tools.FrozenGenerator.Templates;

// =========================================================
/// <summary>
/// Template for the <see cref="FrozenList{T}"/> class.
/// </summary>
internal class FrozenList<T>
{
    public FrozenList<T> GetRange(int index, int count) => throw new NotImplementedException();
    public FrozenList<T> Replace(int index, T item) => throw new NotImplementedException();
    public FrozenList<T> Add(T item) => throw new NotImplementedException();
    public FrozenList<T> AddRange(IEnumerable<T> range) => throw new NotImplementedException();
    public FrozenList<T> Insert(int index, T item) => throw new NotImplementedException();
    public FrozenList<T> InsertRange(int index, IEnumerable<T> range) => throw new NotImplementedException();
    public FrozenList<T> RemoveAt(int index) => throw new NotImplementedException();
    public FrozenList<T> RemoveRange(int index, int count) => throw new NotImplementedException();
    public FrozenList<T> Remove(T item) => throw new NotImplementedException();
    public FrozenList<T> RemoveLast(T item) => throw new NotImplementedException();
    public FrozenList<T> RemoveAll(T item) => throw new NotImplementedException();
    public FrozenList<T> Remove(Predicate<T> predicate) => throw new NotImplementedException();
    public FrozenList<T> RemoveLast(Predicate<T> predicate) => throw new NotImplementedException();
    public FrozenList<T> RemoveAll(Predicate<T> predicate) => throw new NotImplementedException();
    public FrozenList<T> Clear() => throw new NotImplementedException();
}