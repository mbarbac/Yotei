namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Determines the style to use with generic arguments.
/// </summary>
internal enum EasyGenericStyle
{
    /// <summary>
    /// Ignore generic arguments.
    /// </summary>
    None,

    /// <summary>
    /// Only include the generic placeholders. This value is mostly only used when there is the
    /// need of producing a generic list with only the placeholders, but not their names.
    /// </summary>
    PlaceHolders,

    /// <summary>
    /// Use the easy names of the generic arguments.
    /// </summary>
    UseEasyNames,
}