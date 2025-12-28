#pragma warning disable IDE0060

namespace Experimental;

// ========================================================
public class IsNullable<T> { }

// ========================================================
public static class Fakes
{
    public static T ThrowWhenNull<T>(
        [AllowNull] this T source,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source;
    }
}