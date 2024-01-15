using TItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements, identified by their respective
/// keys, with customizable behavior.
/// </summary>
[Cloneable]
public partial interface IParameterList : IEnumerable<TItem>
{
    /// <summary>
    /// Obtains an appropriate builder for this type.
    /// </summary>
    /// <param name="engine"></param>
    /// <returns></returns>
    static abstract Tools.ICoreList<TKey, TItem> CreateBuilder(IEngine engine);

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(TKey key);

    /// <summary>
    /// Returns the index of the first ocurrence of an element with the given key, or -1 if not
    /// found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(TKey key);

    /// <summary>
    /// Returns the index of the last ocurrence of an element with the given key, or -1 if not
    /// found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(TKey key);

    /// <summary>
    /// Returns the indexes of the ocurrences of elements with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOf(TKey key);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    /// <summary>
    /// Returns a builder of the appropriate type with the elements of this instance.
    /// </summary>
    /// <returns></returns>
    Tools.ICoreList<TKey, TItem> ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that contains the given number of elements starting from the given
    /// index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IParameterList GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the new given one.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance with the given element added to it. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList Add(TItem item);

    /// <summary>
    /// Returns a new instance with the elements from the given range add to it. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IParameterList AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance with the given element inserted into it at the given index. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance with the elements from the given range inserted into it, starting
    /// at the given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IParameterList InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance with the element at the given index removed from it. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IParameterList RemoveAt(int index);

    /// <summary>
    /// Returns a new instance with the given number of elements, starting from the given index,
    /// removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IParameterList RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the first ocurrence of an element with the given key removed
    /// from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IParameterList Remove(TKey key);

    /// <summary>
    /// Returns a new instance with the last ocurrence of an element with the given key removed
    /// from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IParameterList RemoveLast(TKey key);

    /// <summary>
    /// Returns a new instance with all ocurrences of elements with the givne key removed from
    /// it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IParameterList RemoveAll(TKey key);

    /// <summary>
    /// Returns a new instance with the first ocurrence of an element that matches the given
    /// predicate removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IParameterList Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with the last ocurrence of an element that matches the given
    /// predicate removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IParameterList RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with all the ocurrences of elements that match the given predicate
    /// removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IParameterList RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with all the original elements removed. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <returns></returns>
    IParameterList Clear();
}

// ========================================================
public static class IParameterListEx
{
    /// <summary>
    /// Returns the next available parameter name in the collection.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string NextName(this IParameterList source)
    {
        ArgumentNullException.ThrowIfNull(source);

        for (int i = source.Count; i < int.MaxValue; i++)
        {
            var name = $"{source.Engine.ParameterPrefix}{i}";
            var index = source.IndexOf(name);
            if (index < 0) return name;
        }
        throw new UnExpectedException("Range of integers exhausted.");
    }

    /// <summary>
    /// Returns a new instance where the new element built using the given value, and the next
    /// available parameter name in the collection, has been added to it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static IParameterList AddNew(
        this IParameterList source, object? value, out IParameter item)
    {
        ArgumentNullException.ThrowIfNull(source);

        var name = source.NextName();
        item = new Code.Parameter(name, value);
        return source.Add(item);
    }
}