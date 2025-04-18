namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a multi-part database identifier.
/// </summary>
[IInvariantList<AsNullable<string>, IIdentifierPart>]
public partial interface IIdentifierChain : IIdentifier
{
    /// <inheritdoc cref="IInvariantList{K, T}.Replace(int, T)"/>
    IIdentifierChain Replace(int index, string? value);

    /// <inheritdoc cref="IInvariantList{K, T}.Add(T)"/>
    IIdentifierChain Add(string? value);

    /// <inheritdoc cref="IInvariantList{K, T}.AddRange(IEnumerable{T})"/>
    IIdentifierChain AddRange(IEnumerable<string?> range);

    /// <inheritdoc cref="IInvariantList{K, T}.Insert(int, T)"/>
    IIdentifierChain Insert(int index, string? value);

    /// <inheritdoc cref="IInvariantList{K, T}.InsertRange(int, IEnumerable{T})"/>
    IIdentifierChain InsertRange(int index, IEnumerable<string?> range);
}