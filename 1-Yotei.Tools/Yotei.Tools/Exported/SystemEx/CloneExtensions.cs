#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class CloneExtensions
{
    extension<T>([AllowNull] T source)
    {
        /// <summary>
        /// Returns either a clone of this instance, or the original one if no clone can be
        /// obtained using its <see cref="ICloneable"/> implementation, or a parameterless
        /// instance <c>Clone</c> method.
        /// </summary>
        /// <returns></returns>
        [return: MaybeNull]
        public T TryClone() => source.TryClone(out var item) ? item : source;

        /// <summary>
        /// Tries to obtain a clone of this instance either using its <see cref="ICloneable"/>
        /// implementation, if any, or a parameterless instance <c>Clone</c> method.
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

    /// <summary>
    /// Caches the <c>Clone</c> method found in each registered type, or null if any.
    /// </summary>
    readonly static Dictionary<Type, MethodInfo?> Methods = [];

    /// <summary>
    /// Returns the clone method of the given type, if any.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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