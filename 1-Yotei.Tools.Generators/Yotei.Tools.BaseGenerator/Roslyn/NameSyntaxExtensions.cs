namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class NameSyntaxExtensions
{
    /// <summary>
    /// Returns a string with the short name of the given name syntax. If it is an alias one,
    /// then its aliased value is used if such is requested.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static string ShortName(this NameSyntax syntax, bool alias = false)
    {
        return syntax switch
        {
            SimpleNameSyntax item => item.Identifier.Text,
            QualifiedNameSyntax item => ShortName(item.Right, alias),
            AliasQualifiedNameSyntax item => ShortName(alias ? item.Alias : item.Name, false),

            _ => throw new Exception($"Name syntax kind not recognized.").WithData(syntax)
        };
    }

    /// <summary>
    /// Returns a string with the long name of the given name syntax. If it is an alias one,
    /// then its aliased value is used if such is requested.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static string LongName(this NameSyntax syntax, bool alias = false)
    {
        return syntax switch
        {
            SimpleNameSyntax item => item.Identifier.Text,
            QualifiedNameSyntax item => item.ToString(),
            AliasQualifiedNameSyntax item => LongName(alias ? item.Alias : item.Name, false),

            _ => throw new Exception($"Name syntax kind not recognized.").WithData(syntax)
        };
    }
}