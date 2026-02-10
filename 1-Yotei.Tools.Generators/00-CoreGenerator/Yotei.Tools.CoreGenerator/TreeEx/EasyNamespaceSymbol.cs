namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyNamespaceSymbol
{
    /// <summary>
    /// Include the host namespaces in the display string.
    /// </summary>
    public bool UseHostNamespace { get; set; }

    /// <summary>
    /// Include the 'global' namespace in the display string. This setting is used only if the
    /// '<see cref="UseHostNamespace"/>' one is enabled.
    /// </summary>
    public bool UseGlobalNamespace { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default code generation settings.
    /// </summary>
    public static EasyNamespaceSymbol Default => new() { };

    /// <summary>
    /// Returns a new instance with a full settings.
    /// </summary>
    public static EasyNamespaceSymbol Full => new()
    {
        UseHostNamespace = true,
        UseGlobalNamespace = true,
    };
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this INamespaceSymbol source) => source.EasyName(EasyNamespaceSymbol.Default);

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this INamespaceSymbol source, EasyNamespaceSymbol options)
    {
        List<string> names = [];

        ISymbol? node = source;
        while (node is not null)
        {
            if (node is INamespaceSymbol ns &&
                ns.Name != null &&
                ns.Name.Length > 0) names.Add(ns.Name);

            if (names.Count > 0 && !options.UseHostNamespace) break;
            node = node.ContainingSymbol;
        }

        names.Reverse();
        var str = string.Join(".", names);

        // Note that before we've used that global is unnamed (length = 0)...
        if (str.Length > 0 && options.UseGlobalNamespace) str = $"global::{str}";
        return str;
    }
}