#pragma warning disable IDE0057

namespace Yotei.ORM.Internal;

// ========================================================
public static partial class StringExtensions
{
    /// <summary>
    /// Extracts the main and alias parts from the given 'main AS alias' source.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static (string Main, string? Alias) IntoMainAndAlias(this string source)
    {
        string main = source.NotNullNotEmpty(trim: false);
        string? alias = null;

        var index = source.LastIndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
        if (index >= 0)
        {
            main = source.Substring(0, index);
            alias = source.Substring(index + 4);
        }

        return (main, alias);
    }

    /// <summary>
    /// Parses the given source string into a 'Target = Value' setter instance, if possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    //public static bool IntoSetter(this string source, out DbTokenSetter? result)
    //{
    //    source = source.NotNullNotEmpty();
    //}
}