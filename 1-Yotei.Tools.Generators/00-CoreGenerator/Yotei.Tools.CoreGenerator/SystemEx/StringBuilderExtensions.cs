namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class StringBuilderExtensions
{
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