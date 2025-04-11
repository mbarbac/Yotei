using IItem = Yotei.ORM.Tools.Templates.IElement;

namespace Yotei.ORM.Tools.Templates;

// ========================================================
/// <inheritdoc/>
[Cloneable]
public partial interface IChain<IItem> : IInvariantList<IItem>
{
    /// <inheritdoc cref="IInvariantList{T}"/>
    new ICoreList<IItem> GetBuilder();

    // ----------------------------------------------------

    /// <inheritdoc cref="IInvariantList{T}.GetRange(int, int)"/>
    new IChain<IItem> GetRange(int index, int count);

    /// <inheritdoc cref="IInvariantList{T}.Replace(int, T)"/>
    new IChain<IItem> Replace(int index, IItem item);

    /// <inheritdoc cref="IInvariantList{T}.Add(T)"/>
    new IChain<IItem> Add(IItem item);

    /// <inheritdoc cref="IInvariantList{T}.AddRange(IEnumerable{T})"/>
    new IChain<IItem> AddRange(IEnumerable<IItem> range);

    /// <inheritdoc cref="IInvariantList{T}.Insert(int, T)"/>
    new IChain<IItem> Insert(int index, IItem item);

    /// <inheritdoc cref="IInvariantList{T}.InsertRange(int, IEnumerable{T})"/>
    new IChain<IItem> InsertRange(int index, IEnumerable<IItem> range);

    /// <inheritdoc cref="IInvariantList{T}.RemoveAt(int)"/>
    new IChain<IItem> RemoveAt(int index);

    /// <inheritdoc cref="IInvariantList{T}.RemoveRange(int, int)"/>
    new IChain<IItem> RemoveRange(int index, int count);

    /// <inheritdoc cref="IInvariantList{T}.Remove(T)"/>
    new IChain<IItem> Remove(IItem item);

    /// <inheritdoc cref="IInvariantList{T}.RemoveLast(T)"/>
    new IChain<IItem> RemoveLast(IItem item);

    /// <inheritdoc cref="IInvariantList{T}.RemoveAll(T)"/>
    new IChain<IItem> RemoveAll(IItem item);

    /// <inheritdoc cref="IInvariantList{T}.Remove(Predicate{T})"/>
    new IChain<IItem> Remove(Predicate<IItem> predicate);

    /// <inheritdoc cref="IInvariantList{T}.RemoveLast(K)"/>
    new IChain<IItem> RemoveLast(Predicate<IItem> predicate);

    /// <inheritdoc cref="IInvariantList{T}.RemoveAll(Predicate{T})"/>
    new IChain<IItem> RemoveAll(Predicate<IItem> predicate);

    /// <inheritdoc cref="IInvariantList{T}.Clear"/>
    new IChain<IItem> Clear();
}