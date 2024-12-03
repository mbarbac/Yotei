namespace Yotei.Tools;

// ========================================================
public static class TypeExtensions
{
    /// <summary>
    /// Determines if the type is a static one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStatic(this Type type)
    {
        type = type.ThrowWhenNull();

        return
           type.Attributes.HasFlag(TypeAttributes.Abstract) &&
           type.Attributes.HasFlag(TypeAttributes.Sealed);
    }
}