namespace Yotei.Tools;

// ========================================================
public static class CloneExtensions
{
    extension<T>([AllowNull] T source)
    {
        /// <summary>
        /// Returns either a clone of this instance, if it can be obtained, or this instance
        /// itself otherwise. This method uses the <see cref="ICloneable"/> implementation of
        /// the type, or a parameterless instance public 'Clone()' method.
        /// </summary>
        /// <returns></returns>
        [return: MaybeNull]
        public T TryClone() => source.TryClone(out var item) ? item : source;

        /// <summary>
        /// Tries to obtain a clone of this instance by using the <see cref="ICloneable"/>
        /// implementation of its type, or a parameterless instance public 'Clone()' method. If
        /// a clone can not be obtained, returns <see langword="false"/> and the our argument is
        /// set to an undefined value.
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
    /// Returns the 'Clone' method of the given type, or null if any.
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