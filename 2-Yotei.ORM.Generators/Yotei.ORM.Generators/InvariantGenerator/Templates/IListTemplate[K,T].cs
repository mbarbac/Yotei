namespace Yotei.ORM.Generators;

// ========================================================
internal interface IListTemplate<K, T>
{
    IListTemplate<K, T> GetRange(int index, int count);
    IListTemplate<K, T> Replace(int index, T value);
    IListTemplate<K, T> Add(T value);
    IListTemplate<K, T> AddRange(IEnumerable<T> range);
    IListTemplate<K, T> Insert(int index, T value);
    IListTemplate<K, T> InsertRange(int index, IEnumerable<T> range);
    IListTemplate<K, T> RemoveAt(int index);
    IListTemplate<K, T> RemoveRange(int index, int count);
    IListTemplate<K, T> Remove(K key);
    IListTemplate<K, T> RemoveLast(K key);
    IListTemplate<K, T> RemoveAll(K key);
    IListTemplate<K, T> Remove(Predicate<T> predicate);
    IListTemplate<K, T> RemoveLast(Predicate<T> predicate);
    IListTemplate<K, T> RemoveAll(Predicate<T> predicate);
    IListTemplate<K, T> Clear();
}