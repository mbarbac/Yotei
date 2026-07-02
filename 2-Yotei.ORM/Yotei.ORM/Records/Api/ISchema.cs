using TKey = Yotei.ORM.IIdentifier;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using IHost = Yotei.ORM.Records.ISchema;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of metadata entries that describe a unit of transfer to and
/// from an underlying database (it typically being a record).
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface ISchema : IEquatable<ISchema>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    new IBuilder ToBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains an entry with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string identifier);

    /// <summary>
    /// Gets the index of first entry in this instance with the given identifier, or -1 if any.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string identifier);

    /// <summary>
    /// Gets the index of last entry in this instance with the given identifier, or -1 if any.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(string identifier);

    /// <summary>
    /// Gets the indexes of the entries in this instance with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(string identifier);

    /// <summary>
    /// Returns the indexes of the entries in this instance whose identifiers match the given
    /// specifications, in the form of a string with dot-separated parts where any empty or null
    /// one is considered an implicit match. Comparison is performed per each part from right to
    /// left.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<int> Match(string? specs);

    /// <summary>
    /// Returns the indexes of the entries in this instance whose identifiers match the given
    /// specifications, in the form of a string with dot-separated parts where any empty or null
    /// one is considered an implicit match. Comparison is performed per each part from right to
    /// left.
    /// <br/> If an unique match is detected, then it is placed in the out argument. Otherise, it
    /// is set to null.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    List<int> Match(string? specs, out IItem? unique);

    /// <summary>
    /// Returns a copy of this instance where the first entry with the given identifier has been
    /// removed.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IHost Remove(string identifier);

    /// <summary>
    /// Returns a copy of this instance where the last entry with the given identifier has been
    /// removed.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IHost RemoveLast(string identifier);

    /// <summary>
    /// Returns a copy of this instance where all the entries with the given identifier has been
    /// removed.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IHost RemoveAll(string identifier);
}