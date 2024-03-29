﻿namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object univocally associated with a given primary source.
/// </summary>
public interface IPrimarySourced
{
    /// <summary>
    /// The identifier of the primary source this instance is univocally associated with.
    /// </summary>
    IIdentifier PrimarySource { get; }
}