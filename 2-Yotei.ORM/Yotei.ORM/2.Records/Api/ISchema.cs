using IHost = Yotei.ORM.Records.ISchema;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Representsthe collection of schema entries that describes a record.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface ISchema : IEquatable<IHost>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains an entry with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string identifier);

    /// <summary>
    /// Gets the index of the entry in this collection with the given identifier, or -1 if any.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string identifier);

    /// <summary>
    /// Returns the indexes of the entries in this collection whose identifiers match the given
    /// specifications.
    /// <br/> Matching is performed by comparing the identifier parts with the given specs, from
    /// right to left, where an empty or null specification is taken as an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<int> Match(string? specs);

    /// <summary>
    /// Returns the indexes of the entries in this collection whose identifiers match the given
    /// specifications. If a unique match is detected, then that match is placed in the out
    /// argument. Otherwise, it is set to null.
    /// <br/> Matching is performed by comparing the identifier parts with the given specs, from
    /// right to left, where an empty or null specification is taken as an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    List<int> Match(string? specs, out IItem? unique);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the entry with the given identifier has been removed.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IHost Remove(string identifier);
}