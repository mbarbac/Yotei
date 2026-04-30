namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of list of generic types.
/// </summary>
public enum EasyGenericListStyle_ZZ
{
    /// <summary>
    /// Ignore the list og generic arguments.
    /// </summary>
    None,

    /// <summary>
    /// Only include the empty placeholders. This value is used when the desired result is an
    /// anonymous list with the placeholders of the generic type arguments.
    /// </summary>
    PlaceHolders,

    /// <summary>
    /// Use the easy names of the type arguments.
    /// </summary>
    UseNames,
}