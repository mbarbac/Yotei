namespace Yotei.Tools;

// ========================================================
public static class CloneExtensions
{
    [return: MaybeNull]
    public static T TryClone<T>([AllowNull] this T source) => source;
}