namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries a flat collection of tokens.
/// </summary>
[Cloneable]
public partial interface IStrTokenChain : IStrToken, IInvariantList<IStrToken>
{
    /// <summary>
    /// The actual collection of tokens carried by this instance.
    /// </summary>
    new IEnumerable<IStrToken> Payload { get; }

    // ----------------------------------------------------

    /// <inheritdoc cref="IInvariantList{T}.GetRange(int, int)"/>
    new IStrTokenChain GetRange(int index, int count);

    /// <inheritdoc cref="IInvariantList{T}.Replace(int, T)"/>
    new IStrTokenChain Replace(int index, IStrToken item);

    /// <inheritdoc cref="IInvariantList{T}.Add(T)"/>
    new IStrTokenChain Add(IStrToken item);

    /// <inheritdoc cref="IInvariantList{T}.AddRange(IEnumerable{T})"/>
    new IStrTokenChain AddRange(IEnumerable<IStrToken> range);

    /// <inheritdoc cref="IInvariantList{T}.Insert(int, T)"/>
    new IStrTokenChain Insert(int index, IStrToken item);

    /// <inheritdoc cref="IInvariantList{T}.InsertRange(int, IEnumerable{T})"/>
    new IStrTokenChain InsertRange(int index, IEnumerable<IStrToken> range);

    /// <inheritdoc cref="IInvariantList{T}.RemoveAt(int)"/>
    new IStrTokenChain RemoveAt(int index);

    /// <inheritdoc cref="IInvariantList{T}.RemoveRange(int, int)"/>
    new IStrTokenChain RemoveRange(int index, int count);

    /// <inheritdoc cref="IInvariantList{T}.Remove(T)"/>
    new IStrTokenChain Remove(IStrToken item);

    /// <inheritdoc cref="IInvariantList{T}.RemoveLast(T)"/>
    new IStrTokenChain RemoveLast(IStrToken item);

    /// <inheritdoc cref="IInvariantList{T}.RemoveAll(T)"/>
    new IStrTokenChain RemoveAll(IStrToken item);

    /// <inheritdoc cref="IInvariantList{T}.Remove(Predicate{T})"/>
    new IStrTokenChain Remove(Predicate<IStrToken> predicate);

    /// <inheritdoc cref="IInvariantList{T}.RemoveLast(Predicate{T})"/>
    new IStrTokenChain RemoveLast(Predicate<IStrToken> predicate);

    /// <inheritdoc cref="IInvariantList{T}.RemoveAll(Predicate{T})"/>
    new IStrTokenChain RemoveAll(Predicate<IStrToken> predicate);

    /// <inheritdoc cref="IInvariantList{T}.Clear"/>
    new IStrTokenChain Clear();
}