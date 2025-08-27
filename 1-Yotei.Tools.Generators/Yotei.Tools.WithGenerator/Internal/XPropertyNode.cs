namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }

    readonly SymbolComparer Comparer = SymbolComparer.Default;
    INamedTypeSymbol ReturnType = default!;
    RoslynNameOptions ReturnNameOptions = RoslynNameOptions.Default;
    AttributeData ThisAttribute = default!;
    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        bool r = true;

        if (!base.Validate(context)) r = false;
        if (!ValidateSpecific(context)) r = false;
        if (!ValidateSingleAttribute(context)) r = false;
        if (!ValidateHostNoRecord(context)) r = false;

        return r;
    }

    // Specific validations...
    bool ValidateSpecific(SourceProductionContext context)
    {
        bool r = true;

        if (Symbol.IsIndexer)
        {
            TreeDiagnostics.IndexerNotSupported(Symbol).Report(context);
            r = false;
        }
        if (!Symbol.HasGetter())
        {
            TreeDiagnostics.NoGetter(Symbol).Report(context);
            r = false;
        }
        if (!Host.IsInterface() && !Symbol.HasSetterOrInit())
        {
            TreeDiagnostics.NoSetter(Symbol).Report(context);
            r = false;
        }

        return r;
    }

    // Custom validations...
    bool ValidateSingleAttribute(SourceProductionContext context)
    {
        ThisAttribute = GetThisAttribute(context, out var error)!;
        if (error) return false;

        if (ThisAttribute is null)
        {
            if (!FindInheritWithsAttribute(Host, out ThisAttribute))
            {
                TreeDiagnostics.NoAttributes(Host).Report(context);
                return false;
            }
        }

        return true;
    }

    // Custom validation
    bool ValidateHostNoRecord(SourceProductionContext context)
    {
        if (!Host.IsRecord) return true;

        TreeDiagnostics.RecordsNotSupported(Host).Report(context);
        return false;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        if (!FindReturnTypeValue(ThisAttribute, out ReturnType, out _)) ReturnType = Host;
        if (!Comparer.Equals(Host, ReturnType)) ReturnNameOptions = RoslynNameOptions.Full;

        // Declared or implemented explicitly...
        if (FindWithMethod(Host, out _)) return;

        // Dispatching...
        if (Host.IsInterface()) EmitHostInterface(context, cb);
        else if (Host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext __, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an abstract one.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext __, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is a concrete one.
    /// </summary>
    void EmitHostConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation.
    /// </summary>
    void EmitExplicitInterfaces(CodeBuilder cb)
    {
        throw null;
    }

    /// <summary>
    /// Gets the collection of interfaces that need explicit implementation.
    /// </summary>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        throw null;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a 'With[Name](value) method with the appropiate name and value type in
    /// the given type, or in the first valid one in the given chains.
    /// <br/> In strict mode, the value type must be the same or derived from this symbol one.
    /// In non-strict mode, it just need to be a compatible one.
    /// </summary>
    bool FindWithMethod(
        INamedTypeSymbol type,
        bool strict,
        out IMethodSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IMethodSymbol value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == MethodName &&
                x.Parameters.Length == 1 &&
                (strict
                    ? x.Parameters[0].Type.IsAssignableTo(Symbol.Type)
                    : Symbol.Type.IsAssignableTo(x.Parameters[0].Type)));

            return value is not null;
        },
        out value, chains);
    }

    /// <summary>
    /// Tries to find a 'With[Name](value) method with the appropiate name and value type in
    /// the given type, or in the first valid one in the given chains. The value type must be
    /// a compatible one with this symbol.
    /// </summary>
    bool FindWithMethod(
        INamedTypeSymbol type,
        out IMethodSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return FindWithMethod(type, false, out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member with the appropiate name in the given type, or in the first valid
    /// one in the given chains.
    /// <br/> In strict mode, the member's type must be the same or derived from this symbol one.
    /// In non-strict mode, it just need to be a compatible one.
    /// </summary>
    bool FindMember(
        INamedTypeSymbol type,
        bool strict,
        out IPropertySymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IPropertySymbol value) =>
        {
            value = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == Symbol.Name &&
                x.Parameters.Length == 0 &&
                (strict
                    ? x.Type.IsAssignableTo(Symbol.Type)
                    : Symbol.Type.IsAssignableTo(x.Type)));

            return value is not null;
        },
        out value, chains);
    }

    /// <summary>
    /// Tries to find a member with the appropiate name in the given type, and decorated with
    /// either the <see cref="WithAttribute"/> attribute of the <see cref="WithAttribute{T}"/>
    /// one, in the given type or in the first valid one in the given chains.
    /// <br/> In strict mode, the member's type must be the same or derived from this symbol one.
    /// In non-strict mode, it just need to be a compatible one.
    /// </summary>
    bool FindDecoratedMember(
        INamedTypeSymbol type,
        bool strict,
        out IPropertySymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IPropertySymbol value) =>
        {
            value = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == Symbol.Name &&
                x.Parameters.Length == 0 &&
                (strict
                    ? x.Type.IsAssignableTo(Symbol.Type)
                    : Symbol.Type.IsAssignableTo(x.Type)) &&
                (x.HasAttributes(typeof(WithAttribute)) ||
                 x.HasAttributes(typeof(WithAttribute<>)))
                );

            return value is not null;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the attribute that decorates this member, or null if any. Detected erros are
    /// reported in the given context.
    /// </summary>
    AttributeData? GetThisAttribute(SourceProductionContext context, out bool error)
    {
        var items = Candidate is not null
            ? Candidate.Attributes
            : Symbol.GetAttributes();

        items = items.Where(x =>
            x.AttributeClass is not null &&
            x.AttributeClass.Name.StartsWith(nameof(WithAttribute))).ToImmutableArray();

        switch (items.Length)
        {
            case 1: error = false;  return items[0];
            case 0: error = false;  return null;
            default: TreeDiagnostics.TooManyAttributes(Host).Report(context); error = true; return null!;
        }
    }

    /// Tries to find either the the<see cref="WithAttribute"/> attribute, or the
    /// <see cref="WithAttribute{T}"/> one, in the given type, or in the first valid one in the
    /// given chains.
    /// <br/> If several attributes are found, the non-generic one takes precedence.
    bool FindWithAttribute(
        INamedTypeSymbol type,
        out AttributeData value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out AttributeData value) =>
        {
            value = null!;
            if (!FindDecoratedMember(type, strict: false, out var member)) return false;
            if (member is null) return false;
            
            value = member.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
            if (value is not null) return true;

            value = member.GetAttributes(typeof(WithAttribute<>)).FirstOrDefault();
            if (value is not null) return true;

            return false;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find either the the <see cref="InheritWithsAttribute"/> attribute, or the
    /// <see cref="InheritWithsAttribute{T}"/> one, in the given type, or in the first valid
    /// one in the given chains.
    /// <br/> If several attributes are found, the non-generic one takes precedence.
    /// </summary>
    static bool FindInheritWithsAttribute(
        INamedTypeSymbol type,
        out AttributeData value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out AttributeData value) =>
        {
            value = type.GetAttributes(typeof(InheritWithsAttribute)).FirstOrDefault();
            if (value is not null) return true;

            value = type.GetAttributes(typeof(InheritWithsAttribute<>)).FirstOrDefault();
            if (value is not null) return true;

            return false;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find the value of the <see cref="WithAttribute.ReturnType"/> property or
    /// the <see cref="InheritWithsAttribute.ReturnType"/> one from the given attribute data.
    /// </summary>
    static bool FindReturnTypeValue(
        AttributeData data, out INamedTypeSymbol value, out bool isnullable)
    {
        // Generic attribute
        if (data.AttributeClass is not null && data.AttributeClass.Arity == 1)
        {
            value = (INamedTypeSymbol)data.AttributeClass.TypeArguments[0];
            value = value.UnwrapNullable(out isnullable);
            return true;
        }

        // Not-generic attribute...
        else
        {
            var name = nameof(WithAttribute.ReturnType);

            if (data.GetNamedArgument(name, out var arg))
            {
                if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
                {
                    value = temp.UnwrapNullable(out isnullable);
                    return true;
                }
            }
        }

        // Not found...
        value = null!;
        isnullable = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find the value of the <see cref="WithAttribute.VirtualMethod"/> property or
    /// the <see cref="InheritWithsAttribute.VirtualMethod"/> one from the given attribute data.
    /// </summary>
    static bool FindVirtualMethodValue(AttributeData data, out bool value)
    {
        var name = nameof(WithAttribute.VirtualMethod);

        if (data.GetNamedArgument(name, out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = false;
        return false;
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated code.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the 'with' keyword for the '{{Symbol.Name}}' member.
        /// </summary>
        {{AttributeDoc}}
        """);
}