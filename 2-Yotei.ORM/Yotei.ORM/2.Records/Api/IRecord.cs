namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a collection of related contents retrieved from, or to be persisted into, an
/// underlying database.
/// </summary>
public interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <summary>
    /// Gets the number of entries in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of original elements, starting from the
    /// given index. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the original value at the given index replaced by the new
    /// given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a new instance with the given value added to the collection.
    /// </summary>
    /// <param name="value"></param>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a new instance with the values from the given range added to the collection. If no
    /// changes have been made, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance with the given value inserted at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a new instance with the values from the given range inserted into the collection.
    /// If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance with the value at the given index removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number or elements, starting from the given index,
    /// have been removed. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where all the original values removed. If no changes have been
    /// made, returns the original instance.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();
}