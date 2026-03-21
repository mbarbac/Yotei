namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Determines the style to use with namespaces.
/// </summary>
internal enum EasyNamespaceStyle
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