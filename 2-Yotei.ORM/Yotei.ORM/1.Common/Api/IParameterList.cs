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

    // ----------------------------------------------------

    /// <summary>
    /// Returns the next available parameter's name.
    /// </summary>
    /// <returns></returns>
    string NextName();

    /// <inheritdoc cref="IFrozenList{K, T}.GetRange(int, int)"/>
    new IParameterList GetRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Replace(int, T)"/>
    new IParameterList Replace(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.Add(T)"/>
    new IParameterList Add(T item);

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, is added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList AddNew(object? value, out T item);

    /// <inheritdoc cref="IFrozenList{K, T}.AddRange(IEnumerable{T})"/>
    new IParameterList AddRange(IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.Insert(int, T)"/>
    new IParameterList Insert(int index, T item);

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, is inserted into it at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList InsertNew(int index, object? value, out T item);

    /// <inheritdoc cref="IFrozenList{K, T}.InsertRange(int, IEnumerable{T})"/>
    new IParameterList InsertRange(int index, IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAt(int)"/>
    new IParameterList RemoveAt(int index);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveRange(int, int)"/>
    new IParameterList RemoveRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(K)"/>
    new IParameterList Remove(K key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(K)"/>
    new IParameterList RemoveLast(K key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(K)"/>
    new IParameterList RemoveAll(K key);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(Predicate{T})"/>
    new IParameterList Remove(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(Predicate{T})"/>
    new IParameterList RemoveLast(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(Predicate{T})"/>
    new IParameterList RemoveAll(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.Clear"/>
    new IParameterList Clear();
}