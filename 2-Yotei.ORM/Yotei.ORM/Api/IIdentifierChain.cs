namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a multi-part database identifier, it being an ordered collection of single part
/// ones.
/// </summary>
[IFrozenList<string, IIdentifierPart>]
public partial interface IIdentifierChain : IIdentifier
{
    /// <inheritdoc cref="IFrozenList{K, T}.Contains(K)"/>
    new bool Contains(string? key);

    /// <inheritdoc cref="IFrozenList{K, T}.IndexOf(K)"/>
    new int IndexOf(string? key);

    /// <inheritdoc cref="IFrozenList{K, T}.LastIndexOf(K)"/>
    new int LastIndexOf(string? key);

    /// <inheritdoc cref="IFrozenList{K, T}.IndexesOf(K)"/>
    new List<int> IndexesOf(string? key);

    // ----------------------------------------------------
    /*
    /// <inheritdoc cref="IFrozenList{K, T}.Remove(K)"/>
    new IIdentifierChain Remove(string? key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(K)"/>
    new IIdentifierChain RemoveLast(string? key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(K)"/>
    new IIdentifierChain RemoveAll(string? key);*/

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by
    /// the ones obtained from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IIdentifierChain Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given value have been added to
    /// the original ones.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IIdentifierChain Add(string? value);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of values have been
    /// added to the original ones.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IIdentifierChain AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given value has been inserted
    /// starting at the given index into the original ones.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IIdentifierChain Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given range of values
    /// have been inserted starting at the given index into the original ones.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IIdentifierChain InsertRange(int index, IEnumerable<string?> range);
}