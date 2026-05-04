namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    private const string ATTRIBUTE = "Attribute";
    private const string NULLABLE_ATTRIBUTE = "System.Runtime.CompilerServices.NullableAttribute";

    /* HIGH: RoslynNameExtensions: Attribute constants
    
    private const string READ_ONLY_ATTRIBUTE = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
    private const string SCOPED_ATTRIBUTE = "System.Runtime.CompilerServices.ScopedRefAttribute";
    private const string REQUIRES_LOCATION = "System.Runtime.CompilerServices.RequiresLocationAttribute";
    */

    // ----------------------------------------------------

    /// <summary>
    /// If the type refers to an special one, then returns its special name without taking into
    /// consideration if it is an array, a pointer, a by-ref type. or a nullable wrapper. If it
    /// is not an special one, then returns null.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? ToSpecialName(this ITypeSymbol source)
    {
        /* HIGH: RoslynNameExtensions: ToSpecialName cases
        if (source.IsByRef) return Core(source.GetElementType() ?? source);
        if (source.IsArray) return Core(source.GetElementType() ?? source);
        if (source.IsNullableWrapper()) return Core(source.GetGenericArguments()[0]);
        return Core(source);
        
         if (source is IArrayTypeSymbol array)
        {
            var type = array.ElementType;
            var str = Core(type);
            str = $"{str}[{new string(',', array.Rank - 1)}]";
            return str;
        }
         */

        if (source is IArrayTypeSymbol array) return Core(array.ElementType);
        if (source.IsNullableWrapper()) return Core(((INamedTypeSymbol)source).TypeArguments[0]);
        return Core(source);

        // Gets the actual special name for the naked source type...
        static string? Core(ITypeSymbol source) => source.SpecialType switch
        {
            SpecialType.System_Void => "void",
            SpecialType.System_Object => "object",
            SpecialType.System_String => "string",
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Char => "char",
            SpecialType.System_SByte => "sbyte",
            SpecialType.System_Byte => "byte",
            SpecialType.System_UInt16 => "ushort",
            SpecialType.System_Int16 => "short",
            SpecialType.System_UInt32 => "uint",
            SpecialType.System_Int32 => "int",
            SpecialType.System_UInt64 => "ulong",
            SpecialType.System_Int64 => "long",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "double",
            SpecialType.System_Decimal => "decimal",
            _ => null,
        };
    }

    /*

    // ----------------------------------------------------

    HIGH: RoslynNameExtensions: HasReadOnlyAttribute
    /// <summary>
    /// Determines if the given element is decorated with a readonly attribute that is carried
    /// in its metadata.
    /// </summary>
    public static bool HasReadOnlyAttribute(this ICustomAttributeProvider info)
    {
        return info
            .GetCustomAttributes(false)
            .Any(x => x.GetType().FullName == READ_ONLY_ATTRIBUTE);
    }
    */

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a generic 'T' one, for EasyName purposes only.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsGenericAlike(
        this ITypeSymbol source) => source.TypeKind is TypeKind.TypeParameter;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a CLR nullable one (<see cref="Nullable{T}"/>), or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsCoreNullable(this ITypeSymbol source) =>
        source is INamedTypeSymbol named &&
        named.Arity == 1 &&
        source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

    /// <summary>
    /// Determines if the given type is a nullable wrapper (either a <see cref="Nullable{T}"/> or
    /// a <see cref="IsNullable{T}"/> one), or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableWrapper(this ITypeSymbol source) =>
        source is INamedTypeSymbol named &&
        named.Arity == 1 && (
        source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
        source.OriginalDefinition.Name == nameof(IsNullable<>));

    /// <summary>
    /// If the given type is a nullable wrapper, returns the underlying type and sets the out
    /// argument to true. Otherwise returns the original type and sets the out argument to false.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static INamedTypeSymbol UnwrapNullable(this INamedTypeSymbol source, out bool nullable)
    {
        if (source.IsNullableWrapper())
        {
            nullable = true;
            return (INamedTypeSymbol)source.TypeArguments[0];
        }

        nullable = false;
        return source;
    }

    extension(AttributeData at)
    {
        // ----------------------------------------------------

        /// <summary>
        /// Returns if the given attribute is a 'NullableAttribute' one, or not, and if so, whether
        /// nullability is enabled or not.
        /// </summary>
        /// <param name="at"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public bool IsNullabilityEnabled(out bool enabled)
        {
            ArgumentNullException.ThrowIfNull(at);
            enabled = false;

            // Attribute is a 'NullableAttribute' one, we return 'true' despite it is enabled or not...
            if (at.AttributeClass?.ToDisplayString() == NULLABLE_ATTRIBUTE)
            {
                if (at.ConstructorArguments.Length > 0)
                {
                    var arg = at.ConstructorArguments[0];

                    if (arg.Kind is TypedConstantKind.Primitive or TypedConstantKind.Enum)
                    { enabled = arg.Value is byte b && b == 2; }

                    else if (arg.Kind == TypedConstantKind.Array && !arg.Values.IsDefaultOrEmpty)
                    { enabled = arg.Values[0].Value is byte b && b == 2; }
                }
                return true;
            }

            /* HIGH: RoslynNameExtensions: IsNullabilityEnabled, old-reflection code...
                var items = at.GetType().GetField("NullableFlags");
                if (items != null)
                {
                    var value = items.GetValue(at);

                    if ((value is byte b && b == 2) ||
                        (value is byte[] bs && bs.Length > 0 && bs[0] == 2))
                        return true;
                }
             */

            // Attribute was not a 'NullableAttribute' one...
            return false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol has been annotated as a nullable one, because nullability
    /// annotations are used, or because its metadata carries a <see cref="NullableAttribute"/>
    /// attribute or a <see cref="IsNullableAttribute"/> one.
    /// <br/> Nullable wrappers are not identified by this method.
    /// <br/> For consistency reasons, generic 'T'-alike are treated as conventional reference ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableAnnotated(this ISymbol source)
    {
        /* HIGH: RoslynNameExtensions: IsNullableAnnotated, T-alike types...
        // All but T-alike types...
        if (source is ITypeSymbol type && type.TypeKind == TypeKind.TypeParameter)
        {
            var atx = type.GetAttributes([typeof(NullableAttribute)]).ToArray();
            if (atx.Length > 0 &&
                atx.Any(x => x.IsNullabilityEnabled(out var enabled) &&
                enabled))
                return true;
        }*/

        // By using nullable annotations...
        switch (source)
        {
            case ITypeSymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
            case IParameterSymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
            case IPropertySymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
            case IFieldSymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
            case IEventSymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
        }

        // By finding in metadata...
        if (source.GetAttributes([typeof(IsNullableAttribute)]).Any()) return true;

        var ats = source.GetAttributes([typeof(NullableAttribute)]).ToArray();
        if (ats.Any(x => x.IsNullabilityEnabled(out var enabled) && enabled)) return true;

        // No nullability found...
        return false;
    }
}