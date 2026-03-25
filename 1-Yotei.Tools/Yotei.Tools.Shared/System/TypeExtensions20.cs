#if YOTEI_TOOLS_COREGENERATOR

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class TypeExtensions20
{
    extension(Type type)
    {
        /// <summary>
        /// Determines whether the current type can be assigned to a variable to the given target
        /// type.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public bool IsAssignableTo(
            [NotNullWhen(true)] Type? targetType) => targetType?.IsAssignableFrom(type) ?? false;
    }
}

#endif