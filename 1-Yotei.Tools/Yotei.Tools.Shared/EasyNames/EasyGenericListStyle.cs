namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to include the generic type arguments in the display string.
/// </summary>
public enum EasyGenericListStyle
{
    /// <summary>
    /// Ignore generic arguments.
    /// </summary>
    None,

    /// <summary>
    /// Only include the generic placeholders. This value is mostly only used when the desired
    /// result is an anonymous list of generic arguments.
    /// </summary>
    PlaceHolders,

    /// <summary>
    /// Use the easy names of the generic arguments.
    /// </summary>
    UseNames,
}