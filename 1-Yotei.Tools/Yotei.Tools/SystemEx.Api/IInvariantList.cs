namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of arbitrary elements whose behavior can be
/// customized.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IInvariantList<T> : IEnumerable<T>, ICloneable
{
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    new IInvariantList<T> Clone();

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Validator"/>
    /// </summary>
    Func<T, bool, T> Validator { get; }

    /// <summary>
    /// Returns a copy of this instance where the value of the <see cref="Validator"/> property
    /// has been replaced by the new given one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> WithValidator(Func<T, bool, T> value);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Comparer"/>
    /// </summary>
    Func<T, T, bool> Comparer { get; }

    /// <summary>
    /// Returns a copy of this instance where the value of the <see cref="Comparer"/> property
    /// has been replaced by the new given one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> WithComparer(Func<T, T, bool> value);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Behavior"/>
    /// </summary>
    CoreListBehavior Behavior { get; }

    /// <summary>
    /// Returns a copy of this instance where the value of the <see cref="Behavior"/> property
    /// has been replaced by the new given one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> WithBehavior(CoreListBehavior value);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.FlatCollection"/>
    /// </summary>
    bool FlatCollection { get; }

    /// <summary>
    /// Returns a copy of this instance where the value of the <see cref="FlatCollection"/>
    /// property has been replaced by the new given one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> WithFlatCollection(bool value);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Count"/>
    /// </summary>
    int Count { get; }

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Trim"/>
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T this[int index] { get; }

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Contains(T)"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(T item);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.IndexOf(T)"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(T item);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.LastIndexOf(T)"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.IndexesOf(T)"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Contains(Predicate{T})"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.IndexOf(Predicate{T})"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.LastIndexOf(Predicate{T})"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.IndexesOf(Predicate{T})"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.ToList"/>
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.ToArray"/>
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance that contains the given number of elements from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<T> GetRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been replaced
    /// by the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> ReplaceItem(int index, T item);

    /// <summary>
    /// Returns a copy of this instance where the given element has been added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> Add(T item);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a copy of this instance where the given element has been inserted into it, at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> Insert(int index, T item);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been inserted
    /// into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of the given element, or an
    /// equivalent one, has been removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(T item);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of the given element, or an
    /// equivalent one, has been removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(T item);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of the given element, or the
    /// equivalent ones, have been removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(T item);

    /// <summary>
    /// Returns a copy of this instance where the given number of elements, starting at the given
    /// index, have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of an element that matches the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of an element that matches the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of elements that match the given
    /// predicate have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IInvariantList<T> Clear();
}