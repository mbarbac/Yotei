namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyNamespace
{
    /// <summary>
    /// If <see langword="true"/>, include the containing namespaces in the display string.
    /// </summary>
    public bool UseContainingNamespace { get; set; }

    /// <summary>
    /// If <see langword="true"/>, include the 'global' namespace in the display string. This
    /// setting is used only if the '<see cref="UseContainingNamespace"/>' one is enabled.
    /// </summary>
    public bool UseGlobalNamespace { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets a new empty instance.
    /// </summary>
    public static EasyNamespace Empty => new();

    /// <summary>
    /// Gets a new default instance:
    /// <br/>- Use containing namespaces but not the global one.
    /// </summary>
    public static EasyNamespace Default => new()
    {
        UseContainingNamespace = true
    };

    // ----------------------------------------------------

    /// <summary>
    /// Returns a display string for the given element using this options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(INamespaceSymbol source) => source.EasyName(this);
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this INamespaceSymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this INamespaceSymbol source, EasyNamespace options)
    {
        List<string> names = [];

        ISymbol? node = source;
        while (node is not null)
        {
            if (node is INamespaceSymbol ns &&
                ns.Name != null &&
                ns.Name.Length > 0) names.Add(ns.Name);

            if (!options.UseContainingNamespace) break;
        }

        names.Reverse();
        var str = string.Join(".", names);

        // Note that before we've used that global is unnamed (length = 0)...
        if (str.Length > 0 && options.UseGlobalNamespace) str = $"global::{str}";
        return str;
    }
}