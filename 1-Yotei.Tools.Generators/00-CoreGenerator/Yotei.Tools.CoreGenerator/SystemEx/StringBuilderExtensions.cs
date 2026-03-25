namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class StringBuilderExtensions
{
    // OJO: https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder.chars?view=net-10.0
    // Incluso en NET 10 no hay metodos como el de abajo
    // O sea: EndsWith(string, comparison) etc...
    // Ni StartsWith...

    extension(StringBuilder source)
    {
        /// <summary>
        /// Determines whether this builder ends with the given character.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public bool EndsWith(char ch) => source.Length > 0 && source[^1] == ch;
    }
}