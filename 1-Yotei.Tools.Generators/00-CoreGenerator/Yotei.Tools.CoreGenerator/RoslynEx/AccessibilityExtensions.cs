namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class AccessibilityExtensions
{
    extension(Accessibility source)
    {
        /// <summary>
        /// Obtains the string that correspond to the given accesibility value, or null if any.
        /// If it is 'private', then null is returned unless explicitly requested.
        /// </summary>
        /// <param name="usePrivate"></param>
        /// <returns></returns>
        public string? ToAccessibilityString(bool usePrivate = false) => source switch
        {
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.Private => usePrivate ? "private" : null,
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => null,
        };
    }
}