namespace Yotei.Tools;

// ========================================================
public static class CloneExtensions
{
    extension<T>([AllowNull] T source)
    {
        /// <summary>
        /// Returns either a clone of this instance, if it can be obtained, or this instance
        /// itself otherwise. A clone is obtained by either using the implementation of the
        /// <see cref="ICloneable"/> interface on this type, or by invoking its parameterless,
        /// public and instance 'Clone()' method, if any.
        /// </summary>
        /// <returns></returns>
        [return: MaybeNull]
        public T TryClone() => source.TryClone(out var result) ? result : source;

        /// <summary>
        /// Tries to obtain a clone of this instance by either using its implementation of the
        /// <see cref="ICloneable"/> interface, or its parameterless, public and instance 'Clone'
        /// method, if any. If so, returns '<c>true</c>' and places that cloned value in the out
        /// argument. If a clone cannot be obtained, returns '<c>false</c>' and sets the out
        /// argument to an undefined value.
        /// </summary>
        /// <param name="cloned"></param>
        /// <returns></returns>
        public bool TryClone([MaybeNull] out T cloned)
        {
            // We return null only if the type is a nullable one...
            if (source is null)
            {
                var type = typeof(T);
                if (!type.IsNullable) throw new InvalidCastException(
                    "Source type is not a nullable one.")
                    .WithData(type);

                cloned = default;
                return true;
            }

            // We may have an ICloneable implementation...
            if (source is ICloneable cloneable)
            {
                cloned = (T)cloneable.Clone();
                return true;
            }

            // Either we find a suitable 'Clone()' method or return the instance itself...
            else
            {
                var type = source.GetType();
                var method = GetCloneMethod(type);
                if (method is not null)
                {
                    cloned = (T?)method.Invoke(source, null);
                    return true;
                }

                cloned = default;
                return false;
            }
        }
    }

    // ----------------------------------------------------

    readonly static Dictionary<Type, MethodInfo?> Methods = [];

    /// <summary>
    /// Invoked to get the 'Clone()' method defined in the given type, or '<c>null</c>' if any.
    /// </summary>
    static MethodInfo? GetCloneMethod(Type type)
    {
        lock (Methods)
        {
            if (Methods.TryGetValue(type, out var method)) return method; // Including null ones!

            var flags = BindingFlags.Instance | BindingFlags.Public;
            method = type.GetMethod("Clone", flags);

            if (method == null ||
                method.GetParameters().Length > 0 || !method.ReturnType.IsAssignableTo(type))
                method = null;

            Methods[type] = method;
            return method;
        }
    }
}