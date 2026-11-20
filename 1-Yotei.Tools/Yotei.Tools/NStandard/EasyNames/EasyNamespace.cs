#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain the easy C#-alike name of a given namespace.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
enum EasyNamespace
{
    /// <summary>
    /// Do not use namespaces.
    /// </summary>
    None,

    /// <summary>
    /// Use the complete namespaces chain.
    /// </summary>
    Complete,

    /// <summary>
    /// Use the complete namespaces chain, preceeded by a 'global::' prefix.
    /// </summary>
    Global,
}