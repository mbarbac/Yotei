namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents the ordered collection of parameters given to a command.
/// </summary>
public interface IParameterList : IEnumerable<IParameter>
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
    IParameter this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains an element with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Returns the index of the element in this collection with the given name, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int IndexOf(string name);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IParameter> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IParameter> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IParameter> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<IParameter> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    IParameter[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<IParameter> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements starting from the given index.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IParameterList GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// given one. The element is not replaced if it is the same as the existing one. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList Replace(int index, IParameter item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList Add(IParameter item);

    /// <summary>
    /// Returns a new instance where a new element, built using the given value and a name that
    /// is automatically generated, has been added to the collection.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList AddNew(object? value, out IParameter item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IParameterList AddRange(IEnumerable<IParameter> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the collection at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList Insert(int index, IParameter item);

    /// <summary>
    /// Returns a new instance where a new element, built using the given value and a name that
    /// is automatically generated, has been inserted into the collection at the given index.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList InsertNew(int index, object? value, out IParameter item);

    /// <summary>
    /// Returns a new instance the elements from the given range have been inserted into the
    /// collection, starting at the given index. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IParameterList InsertRange(int index, IEnumerable<IParameter> range);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed from the
    /// original collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IParameterList RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting from the given index,
    /// have been removed from the collection. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IParameterList RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element with the given name has been removed. If
    /// no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IParameterList Remove(string name);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IParameterList Remove(Predicate<IParameter> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IParameterList RemoveLast(Predicate<IParameter> predicate);

    /// <summary>
    /// Returns a new instance where all elements that match the given predicate has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IParameterList RemoveAll(Predicate<IParameter> predicate);

    /// <summary>
    /// Returns a new instance where all the elements have been removed. If no changes are
    /// detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    IParameterList Clear();
}