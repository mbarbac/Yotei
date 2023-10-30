using THost = Yotei.ORM.Records.IRecord;
using TPair = System.Collections.Generic.KeyValuePair<object?, Yotei.ORM.Records.ISchemaEntry>;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the immutable collection of values and associated metadata that is associated
/// with a record to be retrieved from or persisted into an underlying database.
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<TPair>
{
    /// <summary>
    /// The object that describes the structure and contents of this instance.
    /// </summary>
    ISchema Schema { get; }

    /// <summary>
    /// Returns a new instance where the associated schema has been replaced by the new given
    /// one. The caller must guarantee that the new metadata is compatible with the original
    /// contents.
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    THost WithSchema(ISchema schema);

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Gets a list with the value-metadata pairs whose identifiers match the given one.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<TPair> this[string specs] { get; }

    /// <summary>
    /// Gets a list with the value-metadata pairs whose identifiers match the one obtained from
    /// the given dynamic lambda expression.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<TPair> this[Func<dynamic, object> specs] { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a new instance that contains the given number of elements starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Obtains a new instance where the value at the given index has been replaced with the
    /// new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Replace(int index, object? value);

    /// <summary>
    /// Obtains a new instance where the value and metadata at the given index have been replaced
    /// with the given ones.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Replace(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Obtains a new instance where the metadata at the given index has been replaced with the
    /// new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    THost ReplaceMetadata(int index, ISchemaEntry entry);

    /// <summary>
    /// Obtains a new instance where the given value-metadata pair has been added to the original
    /// record.
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
    THost Add(TPair pair);

    /// <summary>
    /// Obtains a new instance where the value-metadata pairs from the given range have been
    /// added to the original record.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TPair> range);

    /// <summary>
    /// Obtains a new instance where the given value-metadata pair has been inserted into the
    /// original record, at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pair"></param>
    /// <returns></returns>
    THost Insert(int index, TPair pair);

    /// <summary>
    /// Obtains a new instance where the value-metadata pairs from the given range have been
    /// inserted into the original record, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<TPair> range);

    /// <summary>
    /// Obtains a new instance where the value-metadata pair at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Obtains a new instance where the given number of value-metadata apirs have been removed
    /// from the original record, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Obtains a new instance where the first element whose identifier match the given one has
    /// been removed.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    THost Remove(string specs);

    /// <summary>
    /// Obtains a new instance where the first element whose identifier match the one obtained
    /// from the given dynamic lambda expression has been removed.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    THost Remove(Func<dynamic, object> specs);

    /// <summary>
    /// Obtains a new instance where the last element whose identifier match the given one has
    /// been removed.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    THost RemoveLast(string specs);

    /// <summary>
    /// Obtains a new instance where the last element whose identifier match the one obtained
    /// from the given dynamic lambda expression has been removed.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    THost RemoveLast(Func<dynamic, object> specs);

    /// <summary>
    /// Obtains a new instance where all the elements whose identifier match the given one have
    /// been removed.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    THost RemoveAll(string specs);

    /// <summary>
    /// Obtains a new instance where all the elements whose identifier match the one obtained
    /// from the given dynamic lambda expression have been removed.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    THost RemoveAll(Func<dynamic, object> specs);

    /// <summary>
    /// Obtains a new instance where the first ocurrence of an value-metadata pair that matches
    /// the given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TPair> predicate);

    /// <summary>
    /// Obtains a new instance where the last ocurrence of an value-metadata pair that matches
    /// the given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TPair> predicate);

    /// <summary>
    /// Obtains a new instance where all the ocurrences of value-metadata pairs that match the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TPair> predicate);

    /// <summary>
    /// Obtains a new instance where all the original value-metadata pairs have been removed.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}