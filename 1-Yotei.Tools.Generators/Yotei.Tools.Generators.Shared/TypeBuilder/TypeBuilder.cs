namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Provides the ability of generating code that assigns to a given variable a new instance of
/// the associated type, taking into consideration a set of specifications for doing so, if any,
/// and an optional enforced member whose value is externally provided.
/// </summary>
internal partial class TypeBuilder
{
    readonly SymbolEqualityComparer SymbolComparer = SymbolEqualityComparer.Default;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="typeSymbol"></param>
    public TypeBuilder(
        SourceProductionContext context, ITypeSymbol typeSymbol)
    {
        Context = context;
        TypeSymbol = typeSymbol.ThrowWhenNull(nameof(typeSymbol));

        if (TypeSymbol.IsNamespace)
        {
            context.ErrorTypeIsNamespace(typeSymbol);
            throw new ArgumentException($"Symbol '{typeSymbol.Name}' is a namespace, not a type.");
        }
        if (TypeSymbol is not INamedTypeSymbol)
        {
            context.ErrorTypeNotNamed(typeSymbol);
            throw new ArgumentException($"Symbol '{typeSymbol.Name}' is not a named symbol.");
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Builder: {TypeSymbol.Name}";

    /// <summary>
    /// The context provided to this instance.
    /// </summary>
    public SourceProductionContext Context { get; }

    /// <summary>
    /// The type for which the code of generating a new instance will be emitted.
    /// </summary>
    public ITypeSymbol TypeSymbol { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method is a constructor, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    static bool IsConstructor(IMethodSymbol item) => item.MethodKind == MethodKind.Constructor;

    /// <summary>
    /// Determines if the given method is an ordinary one, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    static bool IsOrdinary(IMethodSymbol item) => item.MethodKind == MethodKind.Ordinary;

    /// <summary>
    /// Returns a list with the type constructors.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> GetTypeConstructors()
    {
        if (TypeSymbol.IsAbstract) return new();

        return TypeSymbol.GetMembers().OfType<IMethodSymbol>().Where(x =>
            IsConstructor(x) &&
            x.IsStatic == false &&
            IsVisible(x))
            .ToList();

        // Determines if the element is valid or not.
        static bool IsVisible(IMethodSymbol method)
        {
            return method.DeclaredAccessibility is
                Accessibility.Public or
                Accessibility.Protected or
                Accessibility.Internal or
                Accessibility.ProtectedOrInternal or
                Accessibility.Private;
        }
    }

    /// <summary>
    /// Returns a list with the methods in the associated type (and in its base ones) that return
    /// an instance compatible with the type this one is associated with.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> GetTypeMethods()
    {
        var list = new NoDuplicatesList<IMethodSymbol>()
        {
            ThrowDuplicates = false,
            Equivalent = (inner, other) =>
            {
                if (SymbolComparer.Equals(inner, other)) return true;
                if (inner.Name == other.Name && EquivalentParams()) return true;
                return false;

                bool EquivalentParams()
                {
                    if (inner.Parameters.Length !=  other.Parameters.Length) return false;
                    for (int i = 0; i < inner.Parameters.Length; i++)
                    {
                        if (!SymbolComparer.Equals(
                            inner.Parameters[0].Type,
                            other.Parameters[0].Type))
                            return false;
                    }
                    return true;
                }
            },
        };
        Populate(TypeSymbol, true);
        return list.ToList();

        // Populates the list with the elements of the given type...
        void Populate(ITypeSymbol type, bool top)
        {
            var members = type.GetMembers().OfType<IMethodSymbol>()
                .Where(x => IsValid(x, top))
                .ToDebugArray();

            list.AddRange(members);

            var parent = type.BaseType;
            if (parent != null) Populate(parent, false);
        }

        // Determines if the element is valid or not.
        bool IsValid(IMethodSymbol item, bool top)
        {
            return
                item.CanBeReferencedByName &&
                IsOrdinary(item) &&
                IsVisible(item, top) &&
                item.ReturnType.IsAssignableTo(TypeSymbol);
        }

        // Determines if the element is visible or not.
        bool IsVisible(IMethodSymbol item, bool top)
        {
            if (item.DeclaredAccessibility is
                Accessibility.Public or
                Accessibility.Protected or
                Accessibility.Internal or
                Accessibility.ProtectedOrInternal) return true;

            return top && item.DeclaredAccessibility == Accessibility.Private;
        }
    }

    /// <summary>
    /// Returns a list with the properties in the associated type (and in its base ones) that
    /// can be read from, not duplicated by name.
    /// </summary>
    /// <returns></returns>
    List<IPropertySymbol> GetTypeProperties()
    {
        var list = new NoDuplicatesList<IPropertySymbol>()
        {
            ThrowDuplicates = false,
            Equivalent = (x, y) => x.Name == y.Name,
        };
        Populate(TypeSymbol, true);
        return list.ToList();

        // Populates the list with the elements of the given type...
        void Populate(ITypeSymbol type, bool top)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => IsValid(x, top))
                .ToDebugArray();

            list.AddRange(members);

            var parent = type.BaseType;
            if (parent != null) Populate(parent, false);
        }

        // Determines if the element is valid or not.
        bool IsValid(IPropertySymbol item, bool top)
        {
            return
                item.CanBeReferencedByName &&
                item.GetMethod != null &&
                item.IsStatic == false &&
                item.IsIndexer == false &&
                IsVisible(item, top);
        }

        // Determines if the element is visible or not.
        bool IsVisible(IPropertySymbol item, bool top)
        {
            if (item.DeclaredAccessibility is
                Accessibility.Public or
                Accessibility.Protected or
                Accessibility.Internal or
                Accessibility.ProtectedOrInternal) return true;

            return top && item.DeclaredAccessibility == Accessibility.Private;
        }
    }

    /// <summary>
    /// Returns a list with the properties that can be written to, from the given collection.
    /// </summary>
    /// <returns></returns>
    List<IPropertySymbol> GetWriteProperties(IEnumerable<IPropertySymbol> list)
    {
        return list.Where(x =>
            x.SetMethod != null &&
            x.IsWriteOnly == false)
            .ToList();
    }

    /// <summary>
    /// Returns a list with the fields in the associated type (and in its base ones) that can
    /// be read from, not duplicated by name.
    /// </summary>
    /// <returns></returns>
    List<IFieldSymbol> GetTypeFields()
    {
        var list = new NoDuplicatesList<IFieldSymbol>()
        {
            ThrowDuplicates = false,
            Equivalent = (x, y) => x.Name == y.Name,
        };
        Populate(TypeSymbol, true);
        return list.ToList();

        // Populates the list with the elements of the given type...
        void Populate(ITypeSymbol type, bool top)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => IsValid(x, top))
                .ToDebugArray();

            list.AddRange(members);

            var parent = type.BaseType;
            if (parent != null) Populate(parent, false);
        }

        // Determines if the element is valid or not.
        bool IsValid(IFieldSymbol item, bool top)
        {
            return
                item.CanBeReferencedByName &&
                item.IsStatic == false &&
                item.AssociatedSymbol == null &&
                item.IsImplicitlyDeclared == false &&
                IsVisible(item, top);
        }

        // Determines if the element is visible or not.
        bool IsVisible(IFieldSymbol item, bool top)
        {
            if (item.DeclaredAccessibility is
                Accessibility.Public or
                Accessibility.Protected or
                Accessibility.Internal or
                Accessibility.ProtectedOrInternal) return true;

            return top && item.DeclaredAccessibility == Accessibility.Private;
        }
    }

    /// <summary>
    /// Returns a list with the fields that can be written to, from the given collection.
    /// </summary>
    /// <returns></returns>
    List<IFieldSymbol> GetWriteFields(IEnumerable<IFieldSymbol> list)
    {
        return list.Where(x =>
            x.IsConst == false &&
            x.HasConstantValue == false &&
            x.IsReadOnly == false)
            .ToList();
    }
}