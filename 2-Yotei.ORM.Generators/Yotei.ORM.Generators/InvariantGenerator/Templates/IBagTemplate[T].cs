namespace Yotei.ORM.Generators;

// ========================================================
internal interface IBagTemplate<T>
{
    IBagTemplate<T> Add(T value);
    IBagTemplate<T> AddRange(IEnumerable<T> range);
    IBagTemplate<T> Remove(T value);
    IBagTemplate<T> RemoveAll(T value);
    IBagTemplate<T> Remove(Predicate<T> predicate);
    IBagTemplate<T> RemoveAll(Predicate<T> predicate);
    IBagTemplate<T> Clear();
}