#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a situation where a type by itself, or the type of a variable or argument, is an
/// invalid one. For instance, because it doesn't meet some restrictions, or because it doesn't
/// implement some interfaces, or similar causes.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
class TypeException : Exception
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public TypeException() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    public TypeException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public TypeException(string message, Exception inner) : base(message, inner) { }
}