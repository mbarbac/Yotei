namespace Yotei.ORM.Generators.Invariant;

// ========================================================
internal interface IListTemplate<T>
{
    IListTemplate<T> Add(T item);
    IListTemplate<T> AddRange(IEnumerable<T> range);
    IListTemplate<T> Insert(int index, T item);
    IListTemplate<T> InsertRange(int index, IEnumerable<T> range);
    IListTemplate<T> GetRange(int index, int count);
    IListTemplate<T> Replace(int index, T item, Action<T>? removed = null);
    IListTemplate<T> Replace(int index, T item, out T removed);
    IListTemplate<T> RemoveAt(int index, Action<T>? removed = null);
    IListTemplate<T> RemoveAt(int index, out T removed);
    IListTemplate<T> RemoveRange(int index, int count, Action<T>? removed = null);
    IListTemplate<T> RemoveRange(int index, int count, out List<T> removed);
    IListTemplate<T> Remove(T item, Action<T>? removed = null);
    IListTemplate<T> Remove(T item, out List<T> removed);
    IListTemplate<T> RemoveLast(T item, Action<T>? removed = null);
    IListTemplate<T> RemoveLast(T item, out List<T> removed);
    IListTemplate<T> RemoveAll(T item, Action<T>? removed = null);
    IListTemplate<T> RemoveAll(T item, out List<T> removed);
    IListTemplate<T> Remove(Predicate<T> predicate, Action<T>? removed = null);
    IListTemplate<T> Remove(Predicate<T> predicate, out T removed);
    IListTemplate<T> RemoveLast(Predicate<T> predicate, Action<T>? removed = null);
    IListTemplate<T> RemoveLast(Predicate<T> predicate, out T removed);
    IListTemplate<T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null);
    IListTemplate<T> RemoveAll(Predicate<T> predicate, out List<T> removed);
    IListTemplate<T> Clear();
}