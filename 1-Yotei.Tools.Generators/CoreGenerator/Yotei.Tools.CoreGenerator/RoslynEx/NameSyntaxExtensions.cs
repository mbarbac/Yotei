namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class NameSyntaxExtensions
{
    extension(NameSyntax syntax)
    {
        /// <summary>
        /// Returns a string with the short name of this name syntax. If it is an alias one, then
        /// its aliased value is used if such is requested.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public string ShortName(bool alias = false)
        {
            return syntax switch
            {
                SimpleNameSyntax item => item.Identifier.Text,
                QualifiedNameSyntax item => ShortName(item.Right, alias),
                AliasQualifiedNameSyntax item => ShortName(alias ? item.Alias : item.Name, false),

                _ => throw new UnExpectedException(
                    "Name syntax kind not recognized.").WithData(syntax)
            };
        }

        /// <summary>
        /// Returns a string with the long name of this name syntax. If it is an alias one, then
        /// its aliased value is used if such is requested.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public string LongName(bool alias = false)
        {
            return syntax switch
            {
                SimpleNameSyntax item => item.Identifier.Text,
                QualifiedNameSyntax item => item.ToString(),
                AliasQualifiedNameSyntax item => LongName(alias ? item.Alias : item.Name, false),

                _ => throw new UnExpectedException(
                    "Name syntax kind not recognized.").WithData(syntax)
            };
        }
    }
}