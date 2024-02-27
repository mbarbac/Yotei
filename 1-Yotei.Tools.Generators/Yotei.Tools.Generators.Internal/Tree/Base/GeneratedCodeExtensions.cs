namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class GeneratedCodeExtensions
{
    /// <summary>
    /// Determines if the symbol is decorated with the <see cref="GeneratedCodeAttribute"/>.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsGeneratedCode(ISymbol symbol)
    {
        return symbol.GetAttributes(nameof(GeneratedCodeAttribute)).Any();
    }

    /// <summary>
    /// Determines if the symbol is decorated with the <see cref="GeneratedCodeAttribute"/>.
    /// Even if the attribute is used, the <paramref name="tool"/> and <paramref name="version"/>
    /// arguments may be null if the respective strings are null or empty.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="tool"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool IsGeneratedCode(ISymbol symbol, out string? tool, out string? version)
    {
        tool = null;
        version = null;

        var at = symbol.GetAttributes(nameof(GeneratedCodeAttribute)).FirstOrDefault();
        if (at != null)
        {
            var arg = at.ConstructorArguments[0];
            if (!arg.IsNull && arg.Value is string stool) tool = stool.NullWhenEmpty();

            arg = at.ConstructorArguments[1];
            if (!arg.IsNull && arg.Value is string sversion) version = sversion.NullWhenEmpty();

            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets a string for a <see cref="GeneratedCodeAttribute"/> attribute with the given tool
    /// name and optional version.
    /// </summary>
    /// <param name="tool"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static string GetGeneratedCodeAttribute(string tool, string? version)
    {
        tool = tool.NullWhenEmpty() ?? string.Empty;
        version = version.NullWhenEmpty() ?? string.Empty;

        return $"[GeneratedCode(\"{tool}\", \"{version}\")]";
    }
}