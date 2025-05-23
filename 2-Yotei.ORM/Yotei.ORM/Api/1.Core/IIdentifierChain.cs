﻿using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a collection of elements.
/// </summary>
[IInvariantList<AsNullable<TKey>, IItem>]
public partial interface IIdentifierChain : IIdentifier
{
    /// <inheritdoc cref="IInvariantList{K, T}.GetBuilder"/>
    new IBuilder GetBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the
    /// ones obtained from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given value have been
    /// added to the original collection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Add(string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given range of values
    /// have been added to the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given value have been
    /// inserted into the original collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given range of values
    /// have been inserted into the original collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<string?> range);
}