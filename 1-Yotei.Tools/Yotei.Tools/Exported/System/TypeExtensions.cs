#if YOTEI_TOOLS_GENERATORS
#pragma warning disable CS0436
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents the ocurrence of a duplicated element when it was not supposed to happen.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static partial class TypeExtensions
{
    extension(Type source)
    {
        /// <summary>
        /// Determines if the type is a nullable one.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                if (Nullable.GetUnderlyingType(source) != null) return true;
                if (source.IsClass || source.IsInterface || source.IsArray) return true;

                if (source.GetCustomAttribute<NullableAttribute>() != null) return true;
                if (source.IsGenericType &&
                    source.GetGenericTypeDefinition() == typeof(Nullable<>)) return true;
                
                return false;
            }
        }

        /// <summary>
        /// Determines if the type is a compiler generated one.
        /// </summary>
        public bool IsCompilerGenerated =>
           source.GetCustomAttributes<CompilerGeneratedAttribute>().Any();

        /// <summary>
        /// Determines if the type is a static one.
        /// </summary>
        public bool IsStatic =>
           source.Attributes.HasFlag(TypeAttributes.Abstract) &&
           source.Attributes.HasFlag(TypeAttributes.Sealed);

        /// <summary>
        /// Determines if the type is an anonymous one.
        /// </summary>
        public bool IsAnonymous =>
           source.IsCompilerGenerated &&
           source.IsGenericType &&
           source.Namespace is null &&
           source.Name.Contains("AnonymousType") /*&&
           source.Name.StartsWith("<>")*/;
    }
}

// ========================================================
/// <summary>
/// Represents the ocurrence of a duplicated element when it was not supposed to happen.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
partial class TypeExtensions
{
    extension(Type source)
    {
        /// <summary>
        /// Determines if an instance of this type can be assigned to a variable of the given
        /// target type, or not.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public bool IsAssignableTo(
            [NotNullWhen(true)] Type? targetType) => targetType?.IsAssignableFrom(source) ?? false;
    }
}
#endif