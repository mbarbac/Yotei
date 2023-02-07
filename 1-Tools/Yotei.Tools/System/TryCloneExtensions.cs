namespace Yotei.Tools;

// ========================================================
public static class TryCloneExtensions
{
    /// <summary>
    /// Tries to obtain a clone of the given source, returning either that clone, if possible,
    /// or the original object instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>

    [return: MaybeNull]
    public static T TryClone<T>(
        [AllowNull] this T source) => source.TryClone(out var result) ? result : source;

    /// <summary>
    /// Tries to obtain a clone of the given source.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="cloned"></param>
    /// <returns></returns>
    public static bool TryClone<T>([AllowNull] this T source, [MaybeNull] out T cloned)
    {
        var type = source == null ? typeof(T) : source.GetType();

        if (source == null)
        {
            if (!type.IsNullable()) throw new InvalidCastException(
                "Source type is a not nullable one.")
                .WithData(type);

            cloned = default;
            return true;
        }
        else
        {
            if (source is ICloneable cloneable)
            {
                cloned = (T)cloneable.Clone();
                return true;
            }
            else
            {
                var method = GetCloneMethod(type);
                if (method != null)
                {
                    cloned = (T)method.Invoke(source, null)!;
                    return true;
                }
                else
                {
                    cloned = default;
                    return false;
                }
            }
        }
    }

    // ----------------------------------------------------

    readonly static Dictionary<Type, MethodInfo?> Methods = new();

    static MethodInfo? GetCloneMethod(Type type)
    {
        lock (Methods)
        {
            if (Methods.TryGetValue(type, out var method)) return method;
            else
            {
                var flags = BindingFlags.Instance | BindingFlags.Public;
                method = type.GetMethod("Clone", flags);

                if (method == null ||
                    method.ReturnType == typeof(void) ||
                    method.GetParameters().Length > 0)
                    method = null;

                Methods[type] = method;
                return method;
            }
        }
    }
}