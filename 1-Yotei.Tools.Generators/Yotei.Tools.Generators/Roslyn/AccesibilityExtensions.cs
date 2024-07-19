namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class AccesibilityExtensions
{
    public static string? ToCSharpString(this Accessibility value, bool addspace = false)
    {
        var item = value switch
        {
            Accessibility.Private => "private", // 1
            Accessibility.ProtectedAndInternal => "protected", // 2
            Accessibility.Protected => "protected", // 3
            Accessibility.ProtectedOrInternal => "protected", // 5
            Accessibility.Public => "public",
            _ => null
        };
        if (item != null && addspace) item += ' ';
        return item;
    }
    // 0: NotApplicable => null
    // 2: ProtectedAndFriend => "protected"
    // 4: Friend => null
    // 4: Internal => null
    // 5: ProtectedOrFriend => "protected"
}