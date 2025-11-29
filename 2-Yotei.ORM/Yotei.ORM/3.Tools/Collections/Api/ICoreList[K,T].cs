namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements identified by their respective keys.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<K, T> : ICoreList<T>
{
    /// <summary>
    /// Invoked to obtain the key associated to the given element.
    /// </summary>
    Func<T, K> GetKey { get; }

    /// <summary>
    /// Invoked to return a validated key before using it in this collection.
    /// </summary>
    Func<K, K> ValidateKey { get; }

    /// <summary>
    /// Invoked to determine if, for the purposes of this collection, the two given keys are
    /// equal or not.
    /// </summary>
    Func<K, K, bool> CompareKeys { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains at least one element with the given key, according
    /// to the rules of this instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool ContainsKey(K key);

    /// <summary>
    /// Returns the index of the first ocurrence of an element with the given key, according to
    /// the rules of this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOfKey(T item);

    /// <summary>
    /// Returns the index of the last ocurrence of an element with the given key, according to
    /// the rules of this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOfKey(T item);

    /// <summary>
    /// Returns the indexes of all the ocurrences of elements with the given key, according to
    /// the rules of this instance.
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOfKey(T item);

    // ----------------------------------------------------

    /// <summary>
    /// Removes from this collection the first element with the given key, according to the rules
    /// of this instance. If the given delegate is not null, then it is invoked with the removed
    /// element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveKey(K key, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the first element with the given key, according to the rules
    /// of this instance. If so, the removed element is returned in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveKey(K key, out T item);

    /// <summary>
    /// Removes from this collection the last element with the given key, according to the rules
    /// of this instance. If the given delegate is not null, then it is invoked with the removed
    /// element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveLastKey(K key, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the last element with the given key, according to the rules
    /// of this instance. If so, the removed element is returned in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveLastKey(K key, out T item);

    /// <summary>
    /// Removes from this collection all the elements with the given key, according to the rules
    /// of this instance. If the given delegate is not null, then it is invoked with the removed
    /// elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveAllKeys(K key, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection al the elements with the given key, according to the rules
    /// of this instance. The removed elements are returned in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveAllKeys(K key, out List<T> items);
}