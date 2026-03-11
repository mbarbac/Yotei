namespace Yotei.ORM.Generators;

// ========================================================
internal interface IListTemplate<T>
{
    IListTemplate<T> GetRange(int index, int count);
    IListTemplate<T> Replace(int index, T other);
    IListTemplate<T> Add(T value);
    IListTemplate<T> AddRange(IEnumerable<T> range);
    IListTemplate<T> Insert(int index, T value);
    IListTemplate<T> InsertRange(int index, IEnumerable<T> range);
    IListTemplate<T> RemoveAt(int index);
    IListTemplate<T> RemoveRange(int index, int count);
    IListTemplate<T> Remove(T value);
    IListTemplate<T> RemoveLast(T value);
    IListTemplate<T> RemoveAll(T value);
    IListTemplate<T> Remove(Predicate<T> predicate);
    IListTemplate<T> RemoveLast(Predicate<T> predicate);
    IListTemplate<T> RemoveAll(Predicate<T> predicate);
    IListTemplate<T> Clear();
}