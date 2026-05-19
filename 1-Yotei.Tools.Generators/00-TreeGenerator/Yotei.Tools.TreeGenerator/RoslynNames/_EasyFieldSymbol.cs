namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given field-alike element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IFieldSymbol source) => source.EasyName(new EasyFieldOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given field-alike element, using the given
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source, EasyFieldOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        // DEBUG-ONLY...
        return source.Name;
    }
}