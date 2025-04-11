using IItem = Yotei.ORM.Tools.Templates.IElement;
using TKey = string;

namespace Yotei.ORM.Tools.Templates;

// ========================================================
/// <inheritdoc/>
[Cloneable]
public partial interface IChain<TKey, IItem> : IInvariantList<TKey, IItem>
{
    /// <inheritdoc cref="IInvariantList{K, T}"/>
    new ICoreList<TKey, IItem> GetBuilder();

    // ----------------------------------------------------

    /// <inheritdoc cref="IInvariantList{K, T}.GetRange(int, int)"/>
    new IChain<TKey, IItem> GetRange(int index, int count);

    /// <inheritdoc cref="IInvariantList{K, T}.Replace(int, T)"/>
    new IChain<TKey, IItem> Replace(int index, IItem item);

    /// <inheritdoc cref="IInvariantList{K, T}.Add(T)"/>
    new IChain<TKey, IItem> Add(IItem item);

    /// <inheritdoc cref="IInvariantList{K, T}.AddRange(IEnumerable{T})"/>
    new IChain<TKey, IItem> AddRange(IEnumerable<IItem> range);

    /// <inheritdoc cref="IInvariantList{K, T}.Insert(int, T)"/>
    new IChain<TKey, IItem> Insert(int index, IItem item);

    /// <inheritdoc cref="IInvariantList{K, T}.InsertRange(int, IEnumerable{T})"/>
    new IChain<TKey, IItem> InsertRange(int index, IEnumerable<IItem> range);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveAt(int)"/>
    new IChain<TKey, IItem> RemoveAt(int index);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveRange(int, int)"/>
    new IChain<TKey, IItem> RemoveRange(int index, int count);

    /// <inheritdoc cref="IInvariantList{K, T}.Remove(K)"/>
    new IChain<TKey, IItem> Remove(TKey key);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveLast(K)"/>
    new IChain<TKey, IItem> RemoveLast(TKey key);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveAll(K)"/>
    new IChain<TKey, IItem> RemoveAll(TKey key);

    /// <inheritdoc cref="IInvariantList{K, T}.Remove(Predicate{T})"/>
    new IChain<TKey, IItem> Remove(Predicate<IItem> predicate);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveLast(K)"/>
    new IChain<TKey, IItem> RemoveLast(Predicate<IItem> predicate);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveAll(Predicate{T})"/>
    new IChain<TKey, IItem> RemoveAll(Predicate<IItem> predicate);

    /// <inheritdoc cref="IInvariantList{K, T}.Clear"/>
    new IChain<TKey, IItem> Clear();
}