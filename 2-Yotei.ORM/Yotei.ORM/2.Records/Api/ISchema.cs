namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents the collection of metadata that describes the contents
/// and structure of a given record.
/// <br/> Elements with duplicated identifiers are only allowed if they are the same instance.
/// </summary>
public interface ISchema : IEnumerable<ISchemaEntry>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    ISchemaEntry this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains an element with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(IIdentifier identifier);

    /// <summary>
    /// Determines if this collection contains an element with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string? identifier);

    /// <summary>
    /// Returns the index of the first element with the given identifier, or -1 if not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(IIdentifier identifier);

    /// <summary>
    /// Returns the index of the first element with the given identifier, or -1 if not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string? identifier);

    /// <summary>
    /// Returns the index of the last element with the given identifier, or -1 if not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(IIdentifier identifier);

    /// <summary>
    /// Returns the index of the last element with the given identifier, or -1 if not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(string? identifier);

    /// <summary>
    /// Returns the indexes of the elements with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(IIdentifier identifier);

    /// <summary>
    /// Returns the indexes of the elements with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? identifier);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<ISchemaEntry> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<ISchemaEntry> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<ISchemaEntry> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<ISchemaEntry> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<ISchemaEntry> ToList();

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given specs.
    /// Matching is performed by comparing parts from left to right, where any null or empty
    /// specification is excluded from the comparison and considered an implicit match. If
    /// the returned collection contains just one element, the unique entry and its index
    /// are placed in the out arguments.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    List<int> Match(string? specs, out ISchemaEntry? unique, out int index);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements starting from the given index.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    ISchema GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// given one, if not equal to the existing one. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchema Replace(int index, ISchemaEntry item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchema Add(ISchemaEntry item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchema AddRange(IEnumerable<ISchemaEntry> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the collection at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchema Insert(int index, ISchemaEntry item);

    /// <summary>
    /// Returns a new instance the elements from the given range have been inserted into the
    /// collection, starting at the given index. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchema InsertRange(int index, IEnumerable<ISchemaEntry> range);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed from the
    /// original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    ISchema RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting from the given index,
    /// have been removed from the collection. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    ISchema RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element with the given identifier has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema Remove(IIdentifier identifier);

    /// <summary>
    /// Returns a new instance where the first element with the given identifier has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema Remove(string? identifier);

    /// <summary>
    /// Returns a new instance where the last element with the given identifier has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema RemoveLast(IIdentifier identifier);

    /// <summary>
    /// Returns a new instance where the last element with the given identifier has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema RemoveLast(string? identifier);

    /// <summary>
    /// Returns a new instance where all the elements with the given identifier have been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema RemoveAll(IIdentifier identifier);

    /// <summary>
    /// Returns a new instance where all the elements with the given identifier have been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema RemoveAll(string? identifier);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchema Remove(Predicate<ISchemaEntry> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchema RemoveLast(Predicate<ISchemaEntry> predicate);

    /// <summary>
    /// Returns a new instance where all elements that match the given predicate has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchema RemoveAll(Predicate<ISchemaEntry> predicate);

    /// <summary>
    /// Returns a new instance where all the elements have been removed. If no changes are
    /// detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    ISchema Clear();
}