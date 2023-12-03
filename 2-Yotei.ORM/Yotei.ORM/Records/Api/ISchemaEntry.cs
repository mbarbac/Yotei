using THost = Yotei.ORM.Records.ISchemaEntry;
using TItem = Yotei.ORM.Records.IMetadataPair;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that contains the metadata that describes the structure and contents of a
/// given entry in an associated record.
/// <br/> Elements with duplicated tag names are not allowed.
/// </summary>
public partial interface ISchemaEntry : IEnumerable<TItem>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The identifier by which this instance is known.
    /// </summary>
    [WithGenerator] IIdentifier Identifier { get; }

    /// <summary>
    /// Determines if this instance describes an entry that is a primary key, or is part of the
    /// primary key group, or not, if any.
    /// </summary>
    [WithGenerator] bool IsPrimaryKey { get; }

    /// <summary>
    /// Determines if this instance describes an entry that is an unique valued one, or is part
    /// of the  a unique valued group, or not, if any.
    /// </summary>
    [WithGenerator] bool IsUniqueValued { get; }

    /// <summary>
    /// Determines if this instance describes a read only entry in a record, or not.
    /// </summary>
    [WithGenerator] bool IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value of the element with the given tag. If it does not exist, then an exception
    /// is thrown.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    object? this[string tag] { get; }

    /// <summary>
    /// Tries to obtain the value of the element whose tag is given.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(string tag, out object? value);

    /// <summary>
    /// Determines if this instance has an element with the given tag.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);

    /// <summary>
    /// Determines if this instance has an element with the given tag and value, using a default
    /// comparer for the later.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(string tag, object? value);

    /// <summary>
    /// Determines if this instance has an element with the given tag and value, using the given
    /// comparer for the later.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    bool Contains(string tag, object? value, IEqualityComparer comparer);

    /// <summary>
    /// Returns an array with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the value of the original metadata pair whose tag is given
    /// has been replaced by the new given one. If a pair with that tag name did not exist, then
    /// a new one is added by default. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <param name="add"></param>
    /// <returns></returns>
    THost Replace(string tag, object? value, bool add = true);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original one. If a
    /// metadata pair already exist with the new tag name, then an exception is thrown. If no
    /// changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Add(TItem item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original one. If a metadata pair already exist with any of the new tag names, then an
    /// exception is thrown. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the element with the given tag has been removed from the
    /// original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    THost Remove(string tag);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all last elements that match the given predicate have been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed. If no changes
    /// were needed, returns the original instance instead.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}