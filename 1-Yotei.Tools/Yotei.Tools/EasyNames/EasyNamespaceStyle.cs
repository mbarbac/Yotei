namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Determines the style to use with namespaces.
/// </summary>
public enum EasyNamespaceStyle
{
    /// <summary>
    /// Do not use namespaces.
    /// </summary>
    None,

    /// <summary>
    /// Use the complete namespace, but not the 'global:' one.
    /// </summary>
    Standard,

    /// <summary>
    /// Use the complete namespace, starting with the 'global:' one.
    /// </summary>
    UseGlobal,
}