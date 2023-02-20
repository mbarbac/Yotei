namespace Yotei.Generators;

// ========================================================
internal static class NameSyntaxExtensions
{
    /// <summary>
    /// Returns the short (not multipart) name of the given name syntax. If it is an alias one,
    /// then its aliased value is used if requested.
    /// </summary>
    /// <param name="nameSyntax"></param>
    /// <param name="useAlias"></param>
    /// <returns></returns>
    public static string ShortName(this NameSyntax nameSyntax, bool useAlias = false)
    {
        nameSyntax = nameSyntax.ThrowIfNull(nameof(nameSyntax));

        return nameSyntax switch
        {
            SimpleNameSyntax item => item.Identifier.Text,
            QualifiedNameSyntax item => ShortName(item.Right, useAlias),
            AliasQualifiedNameSyntax item => ShortName(useAlias ? item.Alias : item.Name, false),

            _ => throw new UnreachableException($"Unsupported name syntax: {nameSyntax}")
        };
    }

    /// <summary>
    /// Returns the long (multipart) name of the given name syntax. If it is an alias one, then
    /// its aliased value is used if requested.
    /// </summary>
    /// <param name="nameSyntax"></param>
    /// <param name="useAlias"></param>
    /// <returns></returns>
    public static string LongName(this NameSyntax nameSyntax, bool useAlias = false)
    {
        nameSyntax = nameSyntax.ThrowIfNull(nameof(nameSyntax));

        return nameSyntax switch
        {
            SimpleNameSyntax item => item.Identifier.Text,
            QualifiedNameSyntax item => item.ToString(),
            AliasQualifiedNameSyntax item => LongName(useAlias ? item.Alias : item.Name, false),

            _ => throw new UnreachableException($"Unsupported name syntax: {nameSyntax}")
        };
    }
}