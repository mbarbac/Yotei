namespace Yotei.ORM.Generators.Invariant;

// ========================================================
internal interface IListTemplate<K, T>
{
    IListTemplate<K, T> Add(T item);
    IListTemplate<K, T> AddRange(IEnumerable<T> range);
    IListTemplate<K, T> Insert(int index, T item);
    IListTemplate<K, T> InsertRange(int index, IEnumerable<T> range);
    IListTemplate<K, T> GetRange(int index, int count);
    IListTemplate<K, T> Replace(int index, T item, Action<T>? removed = null);
    IListTemplate<K, T> Replace(int index, T item, out T removed);
    IListTemplate<K, T> RemoveAt(int index, Action<T>? removed = null);
    IListTemplate<K, T> RemoveAt(int index, out T removed);
    IListTemplate<K, T> RemoveRange(int index, int count, Action<T>? removed = null);
    IListTemplate<K, T> RemoveRange(int index, int count, out List<T> removed);
    IListTemplate<K, T> Remove(K key, Action<T>? removed = null);
    IListTemplate<K, T> Remove(K key, out T removed);
    IListTemplate<K, T> RemoveLast(K key, Action<T>? removed = null);
    IListTemplate<K, T> RemoveLast(K key, out T removed);
    IListTemplate<K, T> RemoveAll(K key, Action<T>? removed = null);
    IListTemplate<K, T> RemoveAll(K key, out List<T> removed);
    IListTemplate<K, T> Remove(Predicate<T> predicate, Action<T>? removed = null);
    IListTemplate<K, T> Remove(Predicate<T> predicate, out T removed);
    IListTemplate<K, T> RemoveLast(Predicate<T> predicate, Action<T>? removed = null);
    IListTemplate<K, T> RemoveLast(Predicate<T> predicate, out T removed);
    IListTemplate<K, T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null);
    IListTemplate<K, T> RemoveAll(Predicate<T> predicate, out List<T> removed);
    IListTemplate<K, T> Clear();
}