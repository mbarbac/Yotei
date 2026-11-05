#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class CloneExtensions
{
    extension<T>(T source)
    {
        /// <summary>
        /// Tries to return a clone of this value, if its type implements the <see cref="ICloneable"/>
        /// interface, or implements an instance paramerless <c>Clone</c> method. If a clone cannot
        /// be obtained, returns the original value.
        /// </summary>
        /// <returns></returns>
        [return: MaybeNull]
        public T TryClone() => source.TryClone(out var cloned) ? cloned : source;

        /// <summary>
        /// Tries to obtain a clone of this value, if its type implements the <see cref="ICloneable"/>
        /// interface, or implements an instance paramerless <c>Clone</c> method. If a clone cannot
        /// be obtained, returns false and sets the out argument to a default value.
        /// </summary>
        /// <param name="cloned"></param>
        /// <returns></returns>
        public bool TryClone([MaybeNull] out T cloned)
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