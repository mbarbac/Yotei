#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#pragma warning disable IDE0079
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class CloneExtensions
{
    /// <summary>
    /// Returns a clone of the given value, using either its <see cref="ICloneable"/> implementation
    /// or a parameterless instance <c>Clone</c> method, or returns the original value if no clone
    /// could be obtained.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static T TryClone<T>(
        this T source) => source.TryClone(out var item) ? item : source;

    /// <summary>
    /// Tries to obtain a clone of this instance, using either its <see cref="ICloneable"/>
    /// implementation or a parameterless instance <c>Clone</c> method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="cloned"></param>
    /// <returns></returns>
    public static bool TryClone<T>(this T source, [MaybeNull] out T cloned)
    {
        Type type;
        MethodInfo? method;

        // Returning null only if the type is a nullable one...
        if (source is null)
        {
            type = typeof(T);
            if (!type.IsNullable) throw new InvalidCastException(
                "Source type is not a nullable one.")
                .WithData(type);

            cloned = default;
            return true;
        }

        // Using the ICloneable implementation, if any...
        if (source is ICloneable cloneable)
        {
            cloned = (T)cloneable.Clone();
            return true;
        }

        // Trying to use a 'Clone' method...
        type = source.GetType();
        method = GetCloneMethod(type);

        if (method is not null) // Clone method found...
        {
            cloned = (T?)method.Invoke(source, null);
            return true;
        }
        else // No clone method...
        {
            cloned = default;
            return false;
        }
    }

    // ----------------------------------------------------
    readonly static Dictionary<Type, MethodInfo?> Methods = [];

    /// <summary>
    /// Returns the <c>Clone</c> method associated with the given type, or null if any.
    /// </summary>
    static MethodInfo? GetCloneMethod(Type type)
    {
        lock (Methods)
        {
            if (Methods.TryGetValue(type, out var method)) return method; // Including null ones!

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = type.GetMethods(flags).Where(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0 &&
                x.ReturnType.IsAssignableTo(type)).ToArray();

            method = methods.Length == 1 ? methods[0] : null;
            Methods[type] = method;
            return method;
        }
    }
}