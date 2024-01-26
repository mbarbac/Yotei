using T = Yotei.ORM.IParameter;
using K = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// The ordered collection of parameters in a command.
/// </summary>
[Cloneable]
public partial interface IParameterList : IFrozenList<K, T>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Returns the next available parameter's name.
    /// </summary>
    /// <returns></returns>
    string NextName();

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, is added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList AddNew(object? value, out T? item);

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, is inserted into it at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList InsertNew(int index, object? value, out T? item);

    // ----------------------------------------------------

    new IParameterList GetRange(int index, int count);
    new IParameterList Replace(int index, T item);
    new IParameterList Add(T item);
    new IParameterList AddRange(IEnumerable<T> range);
    new IParameterList Insert(int index, T item);
    new IParameterList InsertRange(int index, IEnumerable<T> range);
    new IParameterList RemoveAt(int index);
    new IParameterList RemoveRange(int index, int count);
    new IParameterList Remove(K key);
    new IParameterList RemoveLast(K key);
    new IParameterList RemoveAll(K key);
    new IParameterList Remove(Predicate<T> predicate);
    new IParameterList RemoveLast(Predicate<T> predicate);
    new IParameterList RemoveAll(Predicate<T> predicate);
    new IParameterList Clear();
}