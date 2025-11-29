namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elementsidentified by their respective keys.
/// <br/> The semantics of the element-oriented methods use, by default, the default comparer
/// for its type. The semantics of the key-oriented method determine equality or keys using the
/// rules in this instance.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<K, T> : ICoreList<T>
{
    /// <summary>
    /// Invoked to determine if the two given elements are equal or not. The semantics of this
    /// delegate are expected to follow, by default, the comparer for their type.
    /// </summary>
    new Func<T, T, bool> CompareItems { get; }

    /// <summary>
    /// Invoked to obtain the key associated to a given element.
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

    /// <summary>
    /// Invoked to find the duplicates of the given key.
    /// </summary>
    Func<K, IEnumerable<T>> GetKeyDuplicates { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains at least one element with the given key, according
    /// to the rules in this instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool ContainsKey(K key);

    /// <summary>
    /// Returns the index of the first element in this collection with the given key, according
    /// to the rules in this instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOfKey(K key);

    /// <summary>
    /// Returns the index of the last element in this collection with the given key, according
    /// to the rules in this instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOfKey(K key);

    /// <summary>
    /// Returns the indexes of all the elements in this collection with the given key, according
    /// to the rules in this instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOfKey(K key);

    // ----------------------------------------------------

    /// <summary>
    /// Removes from this collection the first ocurrence of an element with the given key, as
    /// determined by the rules in this instance. If the given delegate is not null, it is
    /// invoked with the removed element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveKey(K key, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element with the given key, as
    /// determined by the rules in this instance. If so, returns the removed element in the out
    /// argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveKey(K key, out T removed);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element with the given key, as
    /// determined by the rules in this instance. If the given delegate is not null, it is
    /// invoked with the removed element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveLastKey(K key, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element with the given key, as
    /// determined by the rules in this instance. If so, returns the removed element in the out
    /// argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveLastKey(K key, out T removed);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements with the given key, as
    /// determined by the rules in this instance. If the given delegate is not null, it is
    /// invoked with the removed elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveAllKey(K key, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements with the given key, as
    /// determined by the rules in this instance. If so, returns the removed elements in the out
    /// argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveAllKey(K key, out List<T> removed);
}