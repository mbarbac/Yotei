namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a record, an ordered collection of related values that has been retrieved from, or
/// is meant to be persisted into, an underlying database, as a single unit.
/// </summary>
public interface IRecord
{
    /// <summary>
    /// The number of values carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value stored in this instance in the column whose index is given.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    [return: MaybeNull]
    T GetValue<T>(int index);
}