﻿namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Describes an underlying database relational engine.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IEngine : ORM.IEngine
{
    /// <inheritdoc cref="ORM.IEngine.KnownTags"/>
    [With] new IKnownTags KnownTags { get; }

    /// <summary>
    /// The underlying ADO.NET factory used by this instance.
    /// <br/> This property is INFRASTRUCTURE and shall NOT be used by application code.
    /// </summary>
    [With] DbProviderFactory Factory { get; }
}