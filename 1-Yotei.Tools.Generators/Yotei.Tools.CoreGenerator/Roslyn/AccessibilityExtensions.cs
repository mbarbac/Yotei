namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class AccessibilityExtensions
{
    /// <summary>
    /// Returns the C#-alike string representation of the given accessibility value, or null if
    /// no accessibility is applicable. If a not-null string is returned, and space is added to
    /// it if such is requested.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="addspace"></param>
    /// <returns></returns>
    public static string? EasyName(this Accessibility value, bool addspace = false)
    {
        var item = value switch
        {
            Accessibility.Private => "private", // 1
            Accessibility.ProtectedAndInternal => "protected", // 2
            Accessibility.Protected => "protected", // 3
            Accessibility.ProtectedOrInternal => "protected", // 5
            Accessibility.Public => "public", // 6

            // 0: NotApplicable => null
            // 2: ProtectedAndFriend => "protected"
            // 4: Friend => null
            // 4: Internal => null
            // 5: ProtectedOrFriend => "protected"

            _ => null
        };

        if (item != null && addspace) item += ' ';
        return item;
    }
}