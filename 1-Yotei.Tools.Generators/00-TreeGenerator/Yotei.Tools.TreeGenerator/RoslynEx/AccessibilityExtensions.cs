namespace Yotei.Tools.Generators;

// ========================================================
public static class AccessibilityExtensions
{
    extension(Accessibility source)
    {
        /// <summary>
        /// Obtains the string that correspond to the given accesibility value, or null if any.
        /// Caller needs to specifiy whether the host element is an interface or not and, if the
        /// accessibility is 'private', then null is returned unless explicitly requested.
        /// </summary>
        /// <param name="usePrivate"></param>
        /// <returns></returns>
        public string? ToAccessibilityString(bool isInterface, bool usePrivate = false)
        {
            return source switch
            {
                Accessibility.Public when !isInterface => "public",
                Accessibility.Private when usePrivate => "private",
                Accessibility.Protected => "protected",
                Accessibility.Internal => "internal",
                Accessibility.ProtectedOrInternal => "internal protected",
                Accessibility.ProtectedAndInternal => "private protected",
                _ => null,
            };
        }
    }
}