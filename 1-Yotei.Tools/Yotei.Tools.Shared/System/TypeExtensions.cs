namespace Yotei.Tools;

// ========================================================
public static class TypeExtensions
{
    extension(Type source)
    {
        /// <summary>
        /// Determines if this type is a static one, or not.
        /// </summary>
        public bool IsStatic =>
           source.Attributes.HasFlag(TypeAttributes.Abstract) &&
           source.Attributes.HasFlag(TypeAttributes.Sealed);

        /// <summary>
        /// Determines if this type is an anonymous one, or not.
        /// </summary>
        public bool IsAnonymous =>
           source.IsCompilerGenerated &&
           source.IsGenericType &&
           source.Namespace is null &&
           source.Name.Contains("Anonymous") &&
           source.Name.StartsWith("<>");

        /// <summary>
        /// Determines if this type is a compiler generated one, or not.
        /// </summary>
        public bool IsCompilerGenerated =>
           source.GetCustomAttributes<CompilerGeneratedAttribute>().Any();

        /// <summary>
        /// Determines if this type is a nullable one, or not.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                if (Nullable.GetUnderlyingType(source) != null) return true;
                if (source.IsValueType) return false;

                if (source.IsClass || source.IsInterface || source.IsArray) return true;
                if (source.IsGenericType &&
                    source.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return true;

                if (source.GetCustomAttribute<NullableAttribute>() != null) return true;

                return false;
            }
        }

        // ------------------------------------------------
#if NETSTANDARD2_0
        /// <summary>
        /// Determines whether the current type can be assigned to a variable to the given target
        /// type.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public bool IsAssignableTo(
            [NotNullWhen(true)] Type? targetType) => targetType?.IsAssignableFrom(source) ?? false;
#endif
    }
}