using IBuilder = Yotei.ORM.IIdentifierChainBuilder;
using IItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a multi-part database identifier.
/// </summary>
[IInvariantList<AsNullable<TKey>, IItem>]
public partial interface IIdentifierChain : IIdentifier
{
    /// <inheritdoc cref="IInvariantList{K, T}.GetBuilder"/>
    new IBuilder GetBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the
    /// ones obtained from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierChain Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given value have been
    /// added to the original collection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierChain Add(string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given range of values
    /// have been added to the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierChain AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given value have been
    /// inserted into the original collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierChain Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given range of values
    /// have been inserted into the original collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierChain InsertRange(int index, IEnumerable<string?> range);
}