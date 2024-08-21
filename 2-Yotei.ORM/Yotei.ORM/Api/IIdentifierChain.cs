namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an ordered multi-part identifier.
/// </summary>
[IFrozenList<string, IIdentifierPart>]
public partial interface IIdentifierChain : IIdentifier
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IIdentifierChainBuilder ToBuilder();

    // ----------------------------------------------------

    /// <inheritdoc cref="IFrozenList{K, T}.Contains(K)"/>
    new bool Contains(string? value);

    /// <inheritdoc cref="IFrozenList{K, T}.IndexOf(K)"/>
    new int IndexOf(string? value);

    /// <inheritdoc cref="IFrozenList{K, T}.LastIndexOf(K)"/>
    new int LastIndexOf(string? value);

    /// <inheritdoc cref="IFrozenList{K, T}.IndexesOf(K)"/>
    new List<int> IndexesOf(string? value);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new copy of this instance where the original element at the given index has been
    /// replaced by the ones obtained from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierChain Replace(int index, string? value);

    /// <summary>
    /// Returns a new copy of this instance where the elements obtained from the given value have
    /// been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierChain Add(string? value);

    /// <summary>
    /// Returns a new copy of this instance where the elements obtained from the given range of
    /// values have been added to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierChain AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new copy of this instance where the elements obtained from the given value have
    /// been inserted into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierChain Insert(int index, string? value);

    /// <summary>
    /// Returns a new copy of this instance where the elements obtained from the given range of
    /// values have been inserted into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierChain InsertRange(int index, IEnumerable<string?> range);
}