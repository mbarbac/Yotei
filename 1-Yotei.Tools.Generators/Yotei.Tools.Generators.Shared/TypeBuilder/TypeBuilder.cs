namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Provides the ability of generating code that assigns to a given variable a new instance of
/// the associated type, taking into consideration a set of optional specifications for doing so,
/// and an optional enforced value to be assign to a given member of the type while building it.
/// </summary>
internal partial class TypeBuilder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    public TypeBuilder(SourceProductionContext context, ITypeSymbol typeSymbol)
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
    /// Returns a list with all type constructors.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> GetTypeConstructors()
    {
        if (TypeSymbol.IsAbstract) return new();

        return TypeSymbol.GetMembers().OfType<IMethodSymbol>().Where(x =>
            x.MethodKind == MethodKind.Constructor &&
            x.IsStatic == false &&
            IsVisible(x))
            .ToList();

        // Determines if the element is visible, or not.
        static bool IsVisible(IMethodSymbol item)
        {
            return item.DeclaredAccessibility is
                Accessibility.Public or
                Accessibility.Protected or
                Accessibility.Internal or
                Accessibility.ProtectedOrInternal or
                Accessibility.Private;
        }
    }

    /// <summary>
    /// Returns a list with the type methods that return an instance compatible with the type
    /// this instance is associated with.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> GetTypeMethods()
    {
        var list = new NoDuplicatesList<IMethodSymbol>()
        {
            ThrowDuplicates = false,
            Comparer = SymbolComparer.Equals,
        };
        Populate(TypeSymbol, true);
        return list.ToList();

        // Recursive...
        void Populate(ITypeSymbol type, bool top)
        {
            var members = type.GetMembers().OfType<IMethodSymbol>()
                .Where(x => Condition(x))
                .ToDebugArray();

            bool Condition(IMethodSymbol method)
            {
                if (!method.CanBeReferencedByName) return false;
                if (method.MethodKind != MethodKind.Ordinary) return false;
                if (!method.ReturnType.IsAssignableTo(TypeSymbol)) return false;
                if (!IsVisible(method, top)) return false;
                return true;
            }

            list.AddRange(members);

            var parent = type.BaseType;
            if (parent != null) Populate(parent, false);
        }

        // Determines if the method is visible, or not.
        static bool IsVisible(IMethodSymbol item, bool top)
        {
            if (item.DeclaredAccessibility is
                Accessibility.Public or
                Accessibility.Protected or
                Accessibility.Internal or
                Accessibility.ProtectedOrInternal)
                return true;

            return top && item.DeclaredAccessibility == Accessibility.Private;
        }
    }

    /// <summary>
    /// Returns a list with the not-indexed readable properties in the associated type and on its
    /// base types, not duplicated by name.
    /// </summary>
    /// <returns></returns>
    List<IPropertySymbol> GetTypeProperties()
    {
        var list = new NoDuplicatesList<IPropertySymbol>()
        {
            ThrowDuplicates = false,
            Comparer = (x, y) => x.Name == y.Name,
        };
        Populate(TypeSymbol, true);
        return list.ToList();

        // Populates recursively...
        void Populate(ITypeSymbol type, bool top)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
                x.CanBeReferencedByName &&
                x.GetMethod != null &&
                x.IsStatic == false &&
                x.IsIndexer == false &&
                IsVisible(x, top))
                .ToDebugArray();

            list.AddRange(members);

            var parent = type.BaseType;
            if (parent != null) Populate(parent, false);
        }

        // Determines if the element is visible, or not.
        static bool IsVisible(IPropertySymbol item, bool top)
        {
            if (item.DeclaredAccessibility is
                Accessibility.Public or
                Accessibility.Protected or
                Accessibility.Internal or
                Accessibility.ProtectedOrInternal)
                return true;

            return top && item.DeclaredAccessibility == Accessibility.Private;
        }
    }

    /// <summary>
    /// Extracts the writable properties from the given collection.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    List<IPropertySymbol> GetWriteProperties(IEnumerable<IPropertySymbol> list)
    {
        return list.Where(x =>
            x.SetMethod != null &&
            x.IsWriteOnly == false)
            .ToList();
    }

    /// <summary>
    /// Returns a list with the readable fields in the associated type and on its base types, not
    /// duplicated by name.
    /// </summary>
    /// <returns></returns>
    List<IFieldSymbol> GetTypeFields()
    {
        var list = new NoDuplicatesList<IFieldSymbol>()
        {
            ThrowDuplicates = false,
            Comparer = (x, y) => x.Name == y.Name,
        };
        Populate(TypeSymbol, true);
        return list.ToList();

        // Populates recursively...
        void Populate(ITypeSymbol type, bool top)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().Where(x =>
                x.CanBeReferencedByName &&
                x.IsStatic == false &&
                x.AssociatedSymbol == null &&
                x.IsImplicitlyDeclared == false &&
                IsVisible(x, top))
                .ToDebugArray();

            list.AddRange(members);

            var parent = type.BaseType;
            if (parent != null) Populate(parent, false);
        }

        // Determines if the element is visible, or not.
        static bool IsVisible(IFieldSymbol item, bool top)
        {
            if (item.DeclaredAccessibility is
                Accessibility.Public or
                Accessibility.Protected or
                Accessibility.Internal or
                Accessibility.ProtectedOrInternal)
                return true;

            return top && item.DeclaredAccessibility == Accessibility.Private;
        }
    }

    /// <summary>
    /// Extracts the writable fields from the given collection.
    /// </summary>
    /// <param name="list"></param>
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