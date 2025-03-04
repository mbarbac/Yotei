namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class AccessibilityExtensions
{
    /// <summary>
    /// Returns the C# string representation of the specified accessibility. May return null
    /// if no accessibility is applicable.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="addspace"></param>
    /// <returns></returns>
    public static string? ToCSharpString(this Accessibility value, bool addspace = false)
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