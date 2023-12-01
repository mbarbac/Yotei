using THost = Yotei.ORM.Records.IParameterList;
using TItem = Yotei.ORM.Records.IParameter;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents the ordered collection of parameters in a command.
/// <br/> Duplicate elements are allowed as far as they are the same instance.
/// </summary>
public interface IParameterList : IEnumerable<TItem>
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
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains an element with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Returns the index of the first element in this collection with the given name, or -1
    /// if no one can be found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int IndexOf(string name);

    /// <summary>
    /// Returns the index of the last element in this collection with the given name, or -1
    /// if no one can be found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int LastIndexOf(string name);

    /// <summary>
    /// Returns the indexes of the elements in this collection with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    List<int> IndexesOf(string name);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or -1 if no one
    /// can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or -1 if no one
    /// can be found.
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
    /// An array with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// A list with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements from the original one, starting
    /// at the given index. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by 
    /// the new given one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original one. If no
    /// changes were detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Add(TItem item);

    /// <summary>
    /// Returns a new instance where a new element built from the given value and an automatic name
    /// has been added to the original one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost AddNew(object? value, out TItem item);

    /// <summary>
    /// Returns a new instance where the given range of elements have been added to the original
    /// one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted, at the given index, into
    /// the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance where a new element built from the given value and an automatic name
    /// has been inserted into the original one, at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost InsertNew(int index, object? value, out TItem item);

    /// <summary>
    /// Returns a new instance where the given range of elements have been inserted, starting at
    /// the given index, into the original one. If no changes were detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been removed from
    /// the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed from the original one. If no changes were detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element with the given name has been removed from
    /// the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    THost Remove(string name);

    /// <summary>
    /// Returns a new instance where the last element with the given name has been removed from
    /// the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    THost RemoveLast(string name);

    /// <summary>
    /// Returns a new instance where all the elements with the given name have been removed from
    /// the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    THost RemoveAll(string name);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed. If no changes
    /// were detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}