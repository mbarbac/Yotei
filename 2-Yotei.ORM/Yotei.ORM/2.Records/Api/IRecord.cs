namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an ordered collection of related values to be retrieved from or to be persisted
/// into an underlying database as a single unit. Instances of this type may carry a schema that,
/// if not null, acts as a descriptor of the record structure and contents.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The schema that describes the structure and contents of this instance, or <c>null</c>
    /// if it this instance is a schema-less one.
    /// </summary>
    [With(ReturnInterface = true)]
    ISchema? Schema { get; }

    /// <summary>
    /// The number of values carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Gets the value associated with the entry whose unique identifier is given. This property
    /// throws an exception if that identifier is not found, or if this instance is a schema-less
    /// one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    object? this[string identifier] { get; }

    /// <summary>
    /// Tries to get the value associated with the entry whose unique identifier is given. This
    /// method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGet(string identifier, out object? value);

    /// <summary>
    /// Gets an array with the values in this instance.
    /// </summary>
    /// <returns></returns>
    object?[] ToArray();

    /// <summary>
    /// Gets a list with the values in this instance.
    /// </summary>
    /// <returns></returns>
    List<object?> ToList();
}