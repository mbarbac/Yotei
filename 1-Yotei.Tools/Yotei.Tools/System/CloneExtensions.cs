namespace Yotei.Tools;

// ========================================================
public static class CloneExtensions
{
    /// <summary>
    /// Returns either a clone of the given source, or the source itself if its type is not a
    /// cloneable one, or it has not a suitable <c>Clone()</c> method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static T TryClone<T>([AllowNull] this T source)
    {
        return source.TryClone(out var result) ? result : source;
    }

    /// <summary>
    /// Tries to obtain a clone of the given source using either its implementation of the
    /// <see cref="ICloneable"/> interface, or a suitable <c>Clone()</c> method. If it was
    /// obtained, returns true and sets the <paramref name="cloned"/> argument to the cloned
    /// value. Otherwise returns false and sets that argument to its default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="cloned"></param>
    /// <returns></returns>
    public static bool TryClone<T>([AllowNull] this T source, [MaybeNull] out T cloned)
    {
        Type type;
        MethodInfo? method;

        if (source is null)
        {
            type = typeof(T);
            if (!type.IsNullable()) throw new InvalidCastException(
                "Source type is not a nullable one.")
                .WithData(source);

            cloned = default;
            return true;
        }

        if (source is ICloneable cloneable)
        {
            cloned = (T)cloneable.Clone();
            return true;
        }

        type = source.GetType();
        method = GetCloneMethod(type);

        if (method != null)
        {
            cloned = (T?)method.Invoke(source, null);
            return true;
        }

        cloned = default!;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Caches the clone methods discovered for their respective types.
    /// </summary>
    readonly static Dictionary<Type, MethodInfo?> _Methods = [];

    /// <summary>
    /// Returns the <c>Clone()</c> method defined in the given type, or <c>null</c> if any.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static MethodInfo? GetCloneMethod(Type type)
    {
        lock (_Methods)
        {
            if (_Methods.TryGetValue(type, out var method)) return method;

            var flags = BindingFlags.Instance | BindingFlags.Public;
            method = type.GetMethod("Clone", flags);

            if (method == null ||
                method.GetParameters().Length > 0 ||
                !method.ReturnType.IsAssignableTo(type))
                method = null;

            _Methods[type] = method;
            return method;
        }
    }
}