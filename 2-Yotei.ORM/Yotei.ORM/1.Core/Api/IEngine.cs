﻿namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Describes an underlying database engine.
/// </summary>
public partial interface IEngine : IEquatable<IEngine>
{
    /// <summary>
    /// Determines if the engines treats its identifier-alike names case sensitively, or not.
    /// </summary>
    [With] bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used to represent null values.
    /// </summary>
    [With] string NullValueLiteral { get; }

    /// <summary>
    /// Determines if the engine treats the parameters in a command by their ordinal positions,
    /// rather than by their names.
    /// </summary>
    [With] bool PositionalParameters { get; }

    /// <summary>
    /// The default prefix used by the engine to identify the parameters in a command.
    /// </summary>
    [With] string ParameterPrefix { get; }

    /// <summary>
    /// Determines if this engine provides native paging capabilities, or rather they have to
    /// be emulated by the framework.
    /// </summary>
    [With] bool NativePaging { get; }

    /// <summary>
    /// Determines if the engine wraps identifier-alike elements with specific terminators,
    /// or not.
    /// </summary>
    [With] bool UseTerminators { get; }

    /// <summary>
    /// The left terminator used by the engine to wrap identifier-alike elements.
    /// Its value is not used if the <see cref="UseTerminators"/> setting is <c>false</c>.
    /// </summary>
    [With] char LeftTerminator { get; }

    /// <summary>
    /// The right terminator used by the engine to wrap identifier-alike elements.
    /// Its value is not used if the <see cref="UseTerminators"/> setting is <c>false</c>.
    /// </summary>
    [With] char RightTerminator { get; }
}