namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain the display string of a namespace element.
/// </summary>
public enum EasyNamespaceStyle
{
    /// <summary>
    /// Do not use namespaces.
    /// </summary>
    None,

    /// <summary>
    /// Use the complete default chain.
    /// </summary>
    Default,

    /// <summary>
    /// Use the complete namespace chain, preceeded by the 'global::' prefix.
    /// </summary>
    UseGlobal,
}