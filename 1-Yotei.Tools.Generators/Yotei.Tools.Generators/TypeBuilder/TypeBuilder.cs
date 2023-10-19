namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Provides the ability of generating code that assigns to a given variable a new instance of
/// the associated type, taking into consideration a set of specifications for doing so, and an
/// optional enforced member whose value is externally provided.
/// </summary>
internal partial class TypeBuilder
{
    readonly SymbolEqualityComparer SymbolComparer = SymbolEqualityComparer.Default;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    public TypeBuilder(SourceProductionContext context, ITypeSymbol type)
    {
        Context = context;
        TypeSymbol = Validate(type);
    }

    ITypeSymbol Validate(ITypeSymbol type)
    {
        type.ThrowWhenNull(nameof(type));

        if (type.IsNamespace)
        {
            Context.TypeIsNamespace(type, DiagnosticSeverity.Error);
            throw new ArgumentException($"Symbol '{type.Name}' is a namespace, not a type.");
        }
        if (type is not INamedTypeSymbol)
        {
            Context.TypeNotNamed(type, DiagnosticSeverity.Error);
            throw new ArgumentException($"Symbol '{type.Name}' is not a named symbol.");
        }
        return type;
    }

    /// <summary>
    /// The context provided to this instance.
    /// </summary>
    public SourceProductionContext Context { get; }

    /// <summary>
    /// The type for which the code of generating a new instance will be emitted.
    /// </summary>
    public ITypeSymbol TypeSymbol { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Builder: {TypeSymbol.Name}";

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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the methods in the associated type (and in its base ones) that return
    /// an instance compatible with the type this one is associated with.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> GetTypeMethods()
    {
        var list = new CoreList<IMethodSymbol>()
        {
            AcceptDuplicate = (item) => false,
            Compare = (inner, other) =>
            {
                if (SymbolComparer.Equals(inner, other)) return true;
                if (inner.Name == other.Name && EquivalentParams()) return true;
                return false;

                bool EquivalentParams()
                {
                    if (inner.Parameters.Length != other.Parameters.Length) return false;
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the properties in the associated type (and in its base ones) that
    /// can be read from, not duplicated by name.
    /// </summary>
    /// <returns></returns>
    List<IPropertySymbol> GetTypeProperties()
    {
        var list = new CoreList<IPropertySymbol>()
        {
            AcceptDuplicate = (item) => false,
            Compare = (x, y) => x.Name == y.Name,
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the fields in the associated type (and in its base ones) that can
    /// be read from, not duplicated by name.
    /// </summary>
    /// <returns></returns>
    List<IFieldSymbol> GetTypeFields()
    {
        var list = new CoreList<IFieldSymbol>()
        {
            AcceptDuplicate = (item) => false,
            Compare = (x, y) => x.Name == y.Name,
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns the code to clone the value, or the value itself, as requested.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="clone"></param>
    /// <returns></returns>
    public static string TryClone(string value, bool clone)
    {
        return clone
            ? $"({value} is null ) ? null : {value}.Clone()"
            : value;
    }

    /// <summary>
    /// Determines if the given name is a match for the given parameter, in the context of the
    /// given method. If it is an ambiguous match, then the error flag is set.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="error"></param>
    /// <param name="par"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool MatchArgument(
        string name,
        out bool error,
        IParameterSymbol par, IMethodSymbol method)
    {
        error = false;
        name = name.NotNullNotEmpty(nameof(name));

        // Case sensitive...
        if (name == par.Name) return true;

        // Relaxed...
        if (string.Compare(name, par.Name, ignoreCase: true) == 0)
        {
            var pars = method.Parameters.Where(
                x => string.Compare(name, x.Name, ignoreCase: true) == 0).ToList();

            if (pars.Count == 1) return true;
            error = true;
        }

        return false;
    }

    /// <summary>
    /// Returns the member whose name matches the given one, or null if any is found. If it is
    /// an ambiguous match, then the error flag is set.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="error"></param>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static ISymbol? MatchMember(
        string name,
        out bool error,
        List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        error = false;
        name = name.NotNullNotEmpty(nameof(name));

        // Case sensitive
        var prop = properties.Find(x => x.Name == name); if (prop != null) return prop;
        var field = fields.Find(x => x.Name == name); if (field != null) return field;

        // Relaxed...
        var tp = properties.Where(x => string.Compare(x.Name, name, ignoreCase: true) == 0).ToList();
        var tf = fields.Where(x => string.Compare(x.Name, name, ignoreCase: true) == 0).ToList();

        // Ambiguous match...
        if ((tp.Count + tf.Count) > 1) { error = true; return null; }

        // Match or null...
        if (tp.Count == 1) return tp[0];
        if (tf.Count == 1) return tf[0];
        return null;
    }
}