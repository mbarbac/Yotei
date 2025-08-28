namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="FieldNode"/>
internal class XFieldNode : FieldNode
{
    public XFieldNode(TypeNode parent, IFieldSymbol symbol) : base(parent, symbol) { }
    public XFieldNode(TypeNode parent, FieldCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";
    bool IsInherited => Candidate is null;

    readonly SymbolComparer Comparer = SymbolComparer.Default;
    INamedTypeSymbol ReturnType = default!;
    RoslynNameOptions ReturnNameOptions = RoslynNameOptions.Default;
    AttributeData ThisAttribute = default!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        bool r = true;

        if (!base.Validate(context)) r = false;
        if (!ValidateSpecific(context)) r = false;
        if (!ValidateHostNoRecord(context)) r = false;

        return r;
    }

    /// <summary>
    /// Specific validations.
    /// </summary>
    bool ValidateSpecific(SourceProductionContext context)
    {
        bool r = true;

        if (!Host.IsInterface() && !Symbol.IsWrittable())
        {
            TreeDiagnostics.NotWrittable(Symbol).Report(context);
            r = false;
        }

        return r;
    }

    /// <summary>
    /// Custom validations.
    /// </summary>
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
        if (!ValidateSingleAttribute(context)) return;
        if (!FindReturnTypeValue(ThisAttribute, out ReturnType, out _)) ReturnType = Host;
        if (!Comparer.Equals(Host, ReturnType)) ReturnNameOptions = RoslynNameOptions.Full;

        // Declared or implemented explicitly...
        if (FindMethod(Host, out _)) return;

        // Dispatching...
        if (Host.IsInterface()) EmitHostInterface(context, cb);
        else if (Host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostConcrete(context, cb);
    }

    /// <summary>
    /// This validation MUST be invoked in 'Emit()' because, if this instance is an inherited
    /// member, then the hierarchy validations happens at host level, and not at member one.
    /// </summary>
    bool ValidateSingleAttribute(SourceProductionContext context)
    {
        // Inherited members use the attribute applied to their host...
        if (IsInherited)
        {
            var name = nameof(InheritWithsAttribute);
            var items = Host.GetAttributes().Where(x =>
                x.AttributeClass is not null &&
                x.AttributeClass.Name.StartsWith(name)).ToArray();

            switch (items.Length)
            {
                case 1: ThisAttribute = items[0]; return true;
                case 0: TreeDiagnostics.NoAttributes(Host).Report(context); return false;
                default: TreeDiagnostics.TooManyAttributes(Host).Report(context); return false;
            }
        }

        // Regular members use the attribute applied to themselves...
        else
        {
            var items = Candidate is not null
                ? Candidate.Attributes
                : Symbol.GetAttributes();

            items = [..items.Where(x =>
                x.AttributeClass is not null &&
                x.AttributeClass.Name.StartsWith(nameof(WithAttribute)))];

            switch (items.Length)
            {
                case 1: ThisAttribute = items[0]; return true;
                case 0: TreeDiagnostics.NoAttributes(Host).Report(context); return false;
                default: TreeDiagnostics.TooManyAttributes(Host).Report(context); return false;
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext __, CodeBuilder cb)
    {
        var membertype = Symbol.Type.EasyName(RoslynNameOptions.Full);
        var returntype = ReturnType.EasyName(ReturnNameOptions);
        var modifiers = GetModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{returntype}");
        cb.AppendLine($"{MethodName}({membertype} {ArgumentName});");

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var found = IsInherited;
            if (!found)
            {
                Host.RecursiveOnly((INamedTypeSymbol iface, out bool value) =>
                {
                    value = FindMethod(iface, out var method);
                    if (value)
                    {
                        value = Comparer.Equals(Symbol.Type, method.Parameters[0].Type);
                        return true;
                    }

                    value = FindDecoratedMember(iface, out var member);
                    if (value)
                    {
                        value = Comparer.Equals(Symbol.Type, member.Type);
                        return true;
                    }

                    value = false;
                    return false;
                },
                out found, Host.AllInterfaces);
            }

            return found ? "new " : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an abstract one.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext __, CodeBuilder cb)
    {
        var membertype = Symbol.Type.EasyName(RoslynNameOptions.Full);
        var returntype = ReturnType.EasyName(ReturnNameOptions);
        var modifiers = GetModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{returntype}");
        cb.AppendLine($"{MethodName}({membertype} {ArgumentName});");

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var parent = Host.BaseType;
            if (parent != null)
            {
                // A base method already exist...
                if (FindMethod(parent, out var method, parent.AllBaseTypes()))
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;
                    var str = access.ToCSharpString(addspace: true);

                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;
                    return isvirtual ? $"{str}abstract override " : $"{str}abstract ";
                }

                // Or a base method was requested...
                if (FindWithAttribute(parent, out _, parent.AllBaseTypes()))
                {
                    return "public abstract override ";
                }
            }

            // Default...
            return "public abstract ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is a concrete one.
    /// </summary>
    void EmitHostConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Host.GetCopyConstructor(strict: false);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Host).Report(context);
            return;
        }

        var membertype = Symbol.Type.EasyName(RoslynNameOptions.Full);
        var returntype = ReturnType.EasyName(ReturnNameOptions);
        var hostname = Host.EasyName();
        var modifiers = GetModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{returntype}");
        cb.AppendLine($"{MethodName}({membertype} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_temp = new {hostname}(this)");
            cb.AppendLine("{");
            cb.IndentLevel++;
            {
                cb.AppendLine($"{Symbol.Name} = {ArgumentName}");
            }
            cb.IndentLevel--;
            cb.AppendLine("};");
            cb.AppendLine("return v_temp;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var issealed = Host.IsSealed || Symbol.IsSealed;
            var novirtual = FindVirtualMethodValue(ThisAttribute, out var temp) && !temp;

            var parent = Host.BaseType;
            if (parent != null)
            {
                // A base method already exist...
                if (FindMethod(parent, out var method, parent.AllBaseTypes()))
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;
                    var str = access.ToCSharpString(addspace: true);

                    if (novirtual) return $"{str}override sealed ";
                    else
                    {
                        var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;
                        return isvirtual ? $"{str}override " : $"{str}new ";
                    }
                }

                // Or a base method was requested...
                if (FindWithAttribute(parent, out _, parent.AllBaseTypes()))
                {
                    if (novirtual || issealed) return "public new ";
                    return "public override ";
                }
            }

            // Default...
            return novirtual || issealed ? "public " : "public virtual ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find a 'With[Name](value)' method in the given type, or in the first valid one
    /// in the given chains.
    /// </summary>
    bool FindMethod(
        INamedTypeSymbol type,
        out IMethodSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IMethodSymbol value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == MethodName &&
                x.Parameters.Length == 1 &&
                Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

            return value is not null;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find a member with the appropriate name in the given type, or in the first valid
    /// one in the given chains.
    /// </summary>
    bool FindMember(
        INamedTypeSymbol type,
        out IFieldSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IFieldSymbol value) =>
        {
            var items = type.GetMembers().OfType<IFieldSymbol>().ToArray();
            foreach (var item in items)
            {
                if (item.Name != Symbol.Name) continue;

                value = item;
                return true;
            }
            value = null!;
            return false;
        },
        out value, chains);
    }

    /// <summary>
    /// Invoked to find a decorated member with the appropriate name in the given type, or in the
    /// first valid one in the given chains.
    /// </summary>
    bool FindDecoratedMember(
        INamedTypeSymbol type,
        out IFieldSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IFieldSymbol value) =>
        {
            var items = type.GetMembers().OfType<IFieldSymbol>().ToArray();
            foreach (var item in items)
            {
                if (item.Name != Symbol.Name) continue;
                if (!item.HasAttributes(typeof(WithAttribute)) &&
                    !item.HasAttributes(typeof(WithAttribute<>))) continue;

                value = item;
                return true;
            }
            value = null!;
            return false;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find the 'With' attribute in a valid member of the given type, or in the first
    /// valid type in the given chains. If several are found, the non-generic one takes precedence.
    /// </summary>
    bool FindWithAttribute(
        INamedTypeSymbol type,
        out AttributeData value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out AttributeData value) =>
        {
            value = null!;
            if (!FindDecoratedMember(type, out var member)) return false;

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
    /// Invoked to find the 'InheritWiths' attribute in the given type, or in the first valid one
    /// in the given chains. If several are found, the non-generic one takes precedence.
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
    /// Invoked to find the value of the 'ReturnType' property from the given attribute data.
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
    /// Invoked to find the value of the 'VirtualMethod' property from the given attribute data.
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