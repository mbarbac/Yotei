namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a builder for <see cref="IIdentifierChain"/> instances.
/// </summary>
[Cloneable]
public partial interface IIdentifierChainBuilder : ICoreList<string, IIdentifierPart>
{
    /// <summary>
    /// Returns a new instance based upon the contents currently captured by this one.
    /// </summary>
    /// <returns></returns>
    IIdentifierChain ToInstance();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <inheritdoc cref="IIdentifier.Value"/>
    string? Value { get; }

    // ----------------------------------------------------

    /// <inheritdoc cref="ICoreList{K, T}.Contains(K)"/>
    new bool Contains(string? value);

    /// <inheritdoc cref="ICoreList{K, T}.IndexOf(K)"/>
    new int IndexOf(string? value);

    /// <inheritdoc cref="ICoreList{K, T}.LastIndexOf(K)"/>
    new int LastIndexOf(string? value);

    /// <inheritdoc cref="ICoreList{K, T}.IndexesOf(K)"/>
    new List<int> IndexesOf(string? value);

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the original element at the given index with the one(s) obtained from the given
    /// value. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    int Replace(int index, string? value);

    /// <summary>
    /// Adds to this instance the element(s) obtained from the given value. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int Add(string? value);

    /// <summary>
    /// Adds to this instance the element(s) obtained from the given range of values. Returns the
    /// number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Inserts into this instance, starting at the given index, the element(s) obtained from the
    /// given value. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    int Insert(int index, string? value);

    /// <summary>
    /// Inserts into this instance, starting at the given index, the element(s) obtained from the
    /// given range of values. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<string?> range);
}