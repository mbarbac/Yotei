namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class NameSyntaxExtensions
{
    /// <summary>
    /// Returns a string with the short name of the given name syntax. If it is an alias one,
    /// then its aliased value is used if such is requested.
    /// </summary>
    /// <param name="nameSyntax"></param>
    /// <param name="useAlias"></param>
    /// <returns></returns>
    public static string ShortName(this NameSyntax nameSyntax, bool useAlias = false)
    {
        return nameSyntax switch
        {
            SimpleNameSyntax item => item.Identifier.Text,
            QualifiedNameSyntax item => ShortName(item.Right, useAlias),
            AliasQualifiedNameSyntax item => ShortName(useAlias ? item.Alias : item.Name, false),

            _ => throw new ArgumentException($"Unsupported name syntax: {nameSyntax}")
        };
    }

    /// <summary>
    /// Returns a string with the short name of the given name syntax, it potentially being a
    /// multipart dotted one. If it is an alias one, then its aliased value is used if such is
    /// requested.
    /// </summary>
    /// <param name="nameSyntax"></param>
    /// <param name="useAlias"></param>
    /// <returns></returns>
    public static string LongName(this NameSyntax nameSyntax, bool useAlias = false)
    {
        return nameSyntax switch
        {
            SimpleNameSyntax item => item.Identifier.Text,
            QualifiedNameSyntax item => item.ToString(),
            AliasQualifiedNameSyntax item => LongName(useAlias ? item.Alias : item.Name, false),

            _ => throw new ArgumentException($"Unsupported name syntax: {nameSyntax}")
        };
    }
}