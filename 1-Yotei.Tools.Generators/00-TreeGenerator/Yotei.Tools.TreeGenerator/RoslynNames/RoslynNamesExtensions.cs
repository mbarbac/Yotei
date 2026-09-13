namespace Yotei.Tools.Generators;

// ========================================================
internal static partial class RoslynNamesExtensions
{
    private const string ATTRIBUTE = "Attribute";
    private const string NULLABLE_ATTRIBUTE = "System.Runtime.CompilerServices.NullableAttribute";

    extension(ITypeSymbol source)
    {
        /// <summary>
        /// If the type refers to an special one, then returns its special name without taking into
        /// consideration if it is an array, a pointer, a by-ref type. or a nullable wrapper. If it
        /// is not an special one, then returns null.
        /// </summary>
        /// <returns></returns>
        public string? ToSpecialName()
        {
            // TODO: ByRef...
            if (source is IArrayTypeSymbol array) return Core(array.ElementType);
            if (source is IPointerTypeSymbol pointer) return Core(pointer.PointedAtType);
            if (source.IsNullableWrapper) return Core(((INamedTypeSymbol)source).TypeArguments[0]);
            return Core(source);

            // Gets the actual special name for the 'naked' source type...
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

        // ------------------------------------------------

        /// <summary>
        /// Determines if the type is a generic 'T' one, for EasyName purposes only.
        /// </summary>
        public bool IsGenericAlike => source.TypeKind is TypeKind.TypeParameter;

        // ------------------------------------------------

        /// <summary>
        /// Determines if the type is a CLR nullable one (<see cref="Nullable{T}"/>), or not.
        /// </summary>
        public bool IsCoreNullable =>
            source is INamedTypeSymbol named &&
            named.Arity == 1 &&
            source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

        /// <summary>
        /// Determines if the given type is a nullable wrapper (either a <see cref="Nullable{T}"/>
        /// or a <see cref="IsNullable{T}"/> one), or not.
        /// </summary>
        public bool IsNullableWrapper =>
            source is INamedTypeSymbol named &&
            named.Arity == 1 && (
            source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
            source.OriginalDefinition.Name == nameof(IsNullable<>));
    }

    extension(INamedTypeSymbol source)
    {
        // ------------------------------------------------

        /// <summary>
        /// If the given type is a named nullable one, then returns the underlying type and sets
        /// the out argument to true. Otherwise, returns the original type an the out argumens is
        /// set to false.
        /// </summary>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public INamedTypeSymbol UnwrapNullable(out bool nullable)
        {
            if (source.IsNullableWrapper)
            {
                nullable = true;
                return (INamedTypeSymbol)source.TypeArguments[0];
            }

            nullable = false;
            return source;
        }
    }

    extension(AttributeData source)
    {
        // ------------------------------------------------

        /// <summary>
        /// Determines if the given attribute data is a <see cref="NullableAttribute"/> one and,
        /// if so, whether nullability is enabled or not. Otherwise returns false.
        /// </summary>
        /// <returns></returns>
        public bool IsNullabilityEnabled()
        {
            if (source.AttributeClass?.ToDisplayString() == NULLABLE_ATTRIBUTE)
            {
                if (source.ConstructorArguments.Length > 0)
                {
                    var arg = source.ConstructorArguments[0];

                    if ((arg.Kind is TypedConstantKind.Primitive or TypedConstantKind.Enum) &&
                        (arg.Value is byte b1 && b1 == 2))
                        return true;

                    if ((arg.Kind is TypedConstantKind.Array && !arg.Values.IsDefaultOrEmpty) &&
                        (arg.Values[0].Value is byte b2 && b2 == 2))
                        return true;
                }
            }

            return false;
        }
    }

    extension(ISymbol source)
    {
        // ------------------------------------------------

        /// <summary>
        /// Determines if the given symbol has been annotated as a nullable one, either because
        /// its metadata carries either the <see cref="NullableAttribute"/> attribute, or the
        /// <see cref="IsNullableAttribute"/> one. This method only cares about these metadata
        /// attributes, not taking in consideration if the type is a nullable wrapper or not.
        /// <br/>- Reference types may not carry the <see cref="NullableAttribute"/> even if they
        /// have been annotated, so this is why the <see cref="IsNullableAttribute"/> exists (as
        /// well as the <see cref="IsNullable{T}"/> wrapper).
        /// <br/>- For consistency reasons, generic T-alike generic types are treated as regular
        /// reference ones.
        /// </summary>
        /// <returns></returns>
        public bool IsNullableAnnotated()
        {
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

            if (source
                .GetAttributes([typeof(NullableAttribute)]).Any(x => x.IsNullabilityEnabled()))
                return true;

            // No nullability found...
            return false;
        }
    }
}