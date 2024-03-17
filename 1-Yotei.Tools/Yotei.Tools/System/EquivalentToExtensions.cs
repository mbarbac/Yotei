namespace Yotei.Tools;

// ========================================================
public static class EquivalentToExtensions
{
    /// <summary>
    /// Determines if the two given objects can be considered equivalent or not.
    /// <br/> This method is a convenience to test for equality between two objects that may be
    /// null, saving a defensive null check before invoking the source 'Equals' method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool EquivalentTo<T>([AllowNull] this T source, [AllowNull] T target)
    {
        return
            (source is null && target is null) ||
            (source is not null && source.Equals(target));
    }
}