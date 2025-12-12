namespace Yotei.ORM.Generators.Invariant;

// ========================================================
internal interface IBagTemplate<T>
{
    IBagTemplate<T> Add(T item);
    IBagTemplate<T> AddRange(IEnumerable<T> range);
    IBagTemplate<T> Remove(T item, Action<T>? removed = null);
    IBagTemplate<T> Remove(T item, out List<T> removed);
    IBagTemplate<T> RemoveAll(T item, Action<T>? removed = null);
    IBagTemplate<T> RemoveAll(T item, out List<T> removed);
    IBagTemplate<T> Remove(Predicate<T> predicate, Action<T>? removed = null);
    IBagTemplate<T> Remove(Predicate<T> predicate, out T removed);
    IBagTemplate<T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null);
    IBagTemplate<T> RemoveAll(Predicate<T> predicate, out List<T> removed);
    IBagTemplate<T> Clear();
}