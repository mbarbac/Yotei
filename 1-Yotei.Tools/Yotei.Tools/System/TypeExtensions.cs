namespace Yotei.Tools;

// =============================================================
public static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Determines if this type is a static one, or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                type.ThrowWhenNull();
                return
                    type.Attributes.HasFlag(TypeAttributes.Abstract) &&
                    type.Attributes.HasFlag(TypeAttributes.Sealed);
            }
        }

        /// <summary>
        /// Determines if this type is an anonymous one, or not.
        /// </summary>
        public bool IsAnonymous
        {
            get
            {
                type.ThrowWhenNull();
                return
                    type.IsCompilerGenerated &&
                    type.IsGenericType &&
                    type.Namespace is null &&
                    type.Name.Contains("Anonymous") &&
                    type.Name.StartsWith("<>");
            }
        }

        /// <summary>
        /// Determines if this type is a compiler generated one, or not.
        /// </summary>
        public bool IsCompilerGenerated
        {
            get
            {
                type.ThrowWhenNull();
                return type
                    .GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length != 0;
            }
        }

        /// <summary>
        /// Determines if this type is a nullable one, or not.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                type.ThrowWhenNull();

                if (type.IsClass || type.IsInterface || type.IsArray) return true;
                if (type.IsGenericType)
                {
                    var temp = type.GetGenericTypeDefinition();
                    if (temp == typeof(Nullable<>)) return true;
                }
                return false;
            }
        }
    }
}