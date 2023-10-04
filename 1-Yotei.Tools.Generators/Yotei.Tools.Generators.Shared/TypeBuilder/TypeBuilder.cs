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

    // ----------------------------------------------------

    /// <summary>
    /// Captures the appropriate builders for the given name specifications, null to only use
    /// the type constructors, or the name of a regular method that returns a type compatible
    /// with the type associated with this instance.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> CaptureBuilders()
    {
        var methods = Specs.Name == null ? GetTypeConstructors() : GetTypeMethods();

        if (Specs.Name != null)
        {
            var matches = methods.Where(x => x.Name == Specs.Name).ToList();

            methods = matches.Count > 0 ? matches : methods.Where(
                x => string.Compare(x.Name, Specs.Name, ignoreCase: true) == 0).ToList();
        }

        methods = methods.OrderByDescending(x => x.Parameters.Length).ToList();
        return methods;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member with a matching name in the given lists of members. Returns the
    /// member found or null if any. If several are found, an ambiguous error is reported.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="error"></param>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    ISymbol? MatchMember(
        string name, out bool error,
        List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        error = false;

        // Case sensitive...
        var prop = properties.Find(x => x.Name == name); if (prop != null) return prop;
        var field = fields.Find(x => x.Name == name); if (field != null) return field;

        // Relaxed...
        var tprops = properties.Where(x => string.Compare(x.Name, name, true) == 0).ToList();
        var tfields = fields.Where(x => string.Compare(x.Name, name, true) == 0).ToList();

        if ((tprops.Count + tfields.Count) > 1) // Ambiguous match...
        {
            error = true;
            return null;
        }

        if (tprops.Count == 1) return tprops[0];
        if (tfields.Count == 1) return tfields[0];
        return null;
    }

    /// <summary>
    /// Determines if the given builder argument matches with the given parameter, in the context
    /// of the given method. Returns true if so, or false is there is no match. In the later case
    /// an error might be reported is there is an ambiguous match.
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="par"></param>
    /// <param name="method"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    bool MatchArgument(
        BuilderArgument arg, IParameterSymbol par, IMethodSymbol method, out bool error)
    {
        error = false;

        // Case sensitive...
        if (arg.Name == par.Name) return true;

        // Relaxed...
        if (string.Compare(arg.Name, par.Name, ignoreCase: true) == 0)
        {
            var pars = method.Parameters.Where(
                x => string.Compare(x.Name, arg.Name, ignoreCase: true) == 0)
                .ToList();

            if (pars.Count == 1) return true;
            error = true;
        }

        return false;
    }
}