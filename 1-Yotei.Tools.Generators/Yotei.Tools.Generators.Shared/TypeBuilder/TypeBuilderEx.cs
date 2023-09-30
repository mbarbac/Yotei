namespace Yotei.Tools.Generators;

// ========================================================
internal partial class TypeBuilder
{
    /// <summary>
    /// Tries to match the given name with a member from the given lists. If found, its symbol is
    /// placed in the out argument, and removed from the appropriate list if such is requested.
    /// Returns either the symbol found, or null. If an ambiguous match is detected, then an
    /// error is reported.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="member"></param>
    /// <param name="remove"></param>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    ISymbol? MatchMember(
        string name,
        bool remove, out bool error,
        List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        name = name.NotNullNotEmpty(nameof(name));
        error = false;

        // Case-sensitive...
        var prop = properties.Find(x => x.Name == name);
        if (prop != null)
        {
            if (remove) properties.Remove(prop);
            return prop;
        }
        var field = fields.Find(x => x.Name == name);
        if(field != null)
        {
            if (remove) fields.Remove(field);
            return field;
        }

        // Relaxed...
        var ptemps = properties.Where(x => string.Compare(x.Name, name, ignoreCase: true) == 0).ToList();
        var ftemps = fields.Where(x => string.Compare(x.Name, name, ignoreCase: true) == 0).ToList();

        // Ambiguous match...
        if ((ptemps.Count + ftemps.Count) > 1)
        {
            error = true;
            return null;
        }

        // Member match...
        if (ptemps.Count == 1)
        {
            if (remove) properties.Remove(ptemps[0]);
            return ptemps[0];
        }
        if (ftemps.Count == 1)
        {
            if (remove) fields.Remove(ftemps[0]);
            return ftemps[0];
        }

        // Not found...
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the builders with the name requested in the given specifications, null for
    /// constructors, other for regular methods.
    /// </summary>
    void CaptureTypeBuilders()
    {
        if (BuilderSpecs.Name == null) TypeMethods = GetTypeConstructors();
        else
        {
            var methods = GetTypeMethods();
            var stricts = methods.Where(x => x.Name == BuilderSpecs.Name).ToList();

            TypeMethods = stricts.Count > 0 ? stricts : methods.Where(x =>
                string.Compare(x.Name, BuilderSpecs.Name, ignoreCase: true) == 0).ToList();
        }

        // Ordering...
        TypeMethods = TypeMethods.OrderByDescending(x => x.Parameters.Length).ToList();
    }

    /// <summary>
    /// Gets a list with the type constrcutors.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> GetTypeConstructors()
    {
        if (Symbol.IsAbstract) return new();

        return Symbol.GetMembers().OfType<IMethodSymbol>().Where(x =>
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
    /// Gets a list with the type methods whose return type is compatible with the type this
    /// instance is associated with.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> GetTypeMethods()
    {
        var list = new NoDuplicatesList<IMethodSymbol>()
        {
            ThrowDuplicates = false,
            Comparer = SymbolEqualityComparer.Default.Equals,
        };

        Populate(Symbol, true);
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
                if (!method.ReturnType.IsAssignableTo(Symbol)) return false;
                if (!IsVisible(method, top)) return false;
                return true;
            }

            list.AddRange(members);

            var parent = type.BaseType;
            if (parent != null) Populate(parent, false);
        }

        // Determines if the element is visible, or not.
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

    // ----------------------------------------------------

    /// <summary>
    /// Captures the not-indexed readable properties in the associated type, and in its base
    /// ones.
    /// </summary>
    void CaptureTypeProperties()
    {
        var list = new NoDuplicatesList<IPropertySymbol>()
        {
            ThrowDuplicates = false,
            Comparer = (x, y) => x.Name == y.Name,
        };
        Populate(Symbol, true);
        TypeProperties = list.ToList();

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

    // ----------------------------------------------------

    /// <summary>
    /// Captures the readable fields in the associated type, and in its base ones.
    /// </summary>
    void CaptureTypeFields()
    {
        var list = new NoDuplicatesList<IFieldSymbol>()
        {
            ThrowDuplicates = false,
            Comparer = (x, y) => x.Name == y.Name,
        };
        Populate(Symbol, true);
        TypeFields = list.ToList();

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
    public List<IFieldSymbol> GetWriteFields(IEnumerable<IFieldSymbol> list)
    {
        return list.Where(x =>
            x.IsConst == false &&
            x.HasConstantValue == false &&
            x.IsReadOnly == false)
            .ToList();
    }
}