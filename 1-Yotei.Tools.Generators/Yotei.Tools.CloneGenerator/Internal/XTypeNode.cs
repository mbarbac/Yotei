namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    readonly SymbolComparer Comparer = SymbolComparer.Default;
    AttributeData ThisAttribute = default!;
    INamedTypeSymbol ReturnType = default!;
    RoslynNameOptions ReturnNameOptions = RoslynNameOptions.Default;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        bool r = true;

        if (!base.Validate(context)) r = false;
        if (!ValidateSingleAttribute(context)) r = false;
        if (!ValidateNoRecord(context)) r = false;

        return r;
    }

    // Custom validations...
    bool ValidateSingleAttribute(SourceProductionContext context)
    {
        ThisAttribute = GetThisAttribute(context, out var error)!;
        return !error;
    }

    // Custom validations...
    bool ValidateNoRecord(SourceProductionContext context)
    {
        if (!Symbol.IsRecord) return true;

        TreeDiagnostics.RecordsNotSupported(Symbol).Report(context);
        return false;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        if (!FindReturnTypeValue(Symbol, out ReturnType, out _)) ReturnType = Symbol;
        if (!Comparer.Equals(Symbol, ReturnType)) ReturnNameOptions = RoslynNameOptions.Full;

        // Declared or implemented explicitly...
        if (FindCloneMethod(Symbol, out _)) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext __, CodeBuilder cb)
    {
        var returntype = ReturnType.EasyName(ReturnNameOptions);
        var modifiers = GetModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{returntype} Clone();");

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var done = Symbol.RecursiveOnly((INamedTypeSymbol iface, out bool value) =>
            {
                value = true;
                if (FindCloneableAttribute(iface, out _)) return true;
                if (FindCloneMethod(iface, out _)) return true;

                value = false;
                return false;
            },
            out bool found, Symbol.AllInterfaces);

            return found && done ? "new " : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an abstract one.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext __, CodeBuilder cb)
    {
        var returntype = ReturnType.EasyName(ReturnNameOptions);
        var modifiers = GetModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{returntype} Clone();");

        EmitExplicitInterfaces(cb);

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // A base method may exist already...
                if (FindCloneMethod(host, out var method, host.AllBaseTypes()))
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;
                    var str = access.ToCSharpString(addspace: true);

                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;
                    return isvirtual ? $"{str}abstract override " : $"{str}abstract ";
                }

                // Or a base method was requested we need to reabstract...
                if (FindCloneableAttribute(host, out _, host.AllBaseTypes()))
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
        var ctor = Symbol.GetCopyConstructor(strict: false);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
            return;
        }

        var returntype = ReturnType.EasyName(ReturnNameOptions);
        var thisname = Symbol.EasyName();
        var modifiers = GetModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{returntype} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_temp = new {thisname}(this);");
            cb.AppendLine("return v_temp;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitExplicitInterfaces(cb);

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var issealed = Symbol.IsSealed;
            var novirtual = FindVirtualMethodValue(Symbol, out var value) && !value;

            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // A base method may exist already...
                if (FindCloneMethod(host, out var method, host.AllBaseTypes()))
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

                // Or a base method was requested we need to reabstract...
                if (FindCloneableAttribute(host, out _, host.AllBaseTypes()))
                {
                    return host.IsAbstract || novirtual ? "public new " : "public override ";
                }
            }

            // Default...
            return novirtual || issealed ? "public " : "public virtual ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation.
    /// </summary>
    void EmitExplicitInterfaces(CodeBuilder cb)
    {
        var ifaces = GetExplicitInterfaces();

        foreach (var iface in ifaces)
        {
            var typename = iface.EasyName(RoslynNameOptions.Full with { UseTypeNullable = false });
            var valuename = iface.Name == "ICloneable" ? "object" : typename;

            cb.AppendLine();
            cb.AppendLine(valuename);
            cb.AppendLine($"{typename}.Clone()");
            cb.AppendLine($"=> ({valuename})Clone();");
        }
    }

    /// <summary>
    /// Gets the collection of interfaces that need explicit implementation.
    /// </summary>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        return list;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            var found = false;

            // First child ones...
            foreach (var child in iface.Interfaces)
            {
                var temp = TryCapture(child);
                if (temp) found = true;
            }

            // Then, the given one if needed...
            if (!found) found =
                FindCloneMethod(iface, out _) ||
                FindCloneableAttribute(iface, out _);

            // Adding if needed and not duplicated...
            if (found)
            {
                var temp = list.Find(x => Comparer.Equals(x, iface));
                if (temp is null) list.Add(iface);
            }

            return found;
        }
    }

    /*void TryAddICloneable()
    {
        var any = Symbol.AllInterfaces.Any(x => x.Name == "ICloneable");
        var add = any || FindAddICloneableValue(Symbol, out var found) && found;

        if (add)
        {
            var comp = GetBranchCompilation();
            var item = comp.GetTypeByMetadataName("System.ICloneable");

            if (item != null)
            {
                var temp = list.Find(x => comparer.Equals(x, item));
                if (temp == null) list.Add(item);
            }
        }
    }*/

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a 'Clone()' method on the given type, or on the first valid one in the
    /// given chains.
    /// </summary>
    static bool FindCloneMethod(
        INamedTypeSymbol type,
        out IMethodSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IMethodSymbol value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0 &&
                x.ReturnsVoid == false);

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

        items = [..items.Where(x =>
            x.AttributeClass is not null &&
            x.AttributeClass.Name.StartsWith(nameof(CloneableAttribute)))];

        switch (items.Length)
        {
            case 1: error = false; return items[0];
            case 0: TreeDiagnostics.NoAttributes(Symbol).Report(context); error = true; return null!;
            default: TreeDiagnostics.TooManyAttributes(Symbol).Report(context); error = true; return null!;
        }
    }

    /// <summary>
    /// Tries to find a <see cref="CloneableAttribute"/> or a <see cref="CloneableAttribute{T}"/>
    /// attribute decorating the given type, or the first valid one in the given chains.
    /// <br/> If several attributes are found, the non-generic one takes precedence.
    /// </summary>
    static bool FindCloneableAttribute(
        INamedTypeSymbol type,
        out AttributeData value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out AttributeData value) =>
        {
            value = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();
            if (value is not null) return true;

            value = type.GetAttributes(typeof(CloneableAttribute<>)).FirstOrDefault();
            if (value is not null) return true;

            return false;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find the return type from the <see cref="CloneableAttribute.ReturnType"/>
    /// property, or from the <see cref="CloneableAttribute{T}"/> one.
    /// </summary>
    static bool FindReturnTypeValue(
        AttributeData data, out INamedTypeSymbol value, out bool isnullable)
    {
        // Generic attribute...
        if (data.AttributeClass is not null && data.AttributeClass.Arity == 1)
        {
            value = (INamedTypeSymbol)data.AttributeClass.TypeArguments[0];
            value = value.UnwrapNullable(out isnullable);
            return true;
        }

        // Not-generic attribute...
        else
        {
            if (data.GetNamedArgument(nameof(CloneableAttribute.ReturnType), out var arg))
            {
                if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
                {
                    value = temp.UnwrapNullable(out isnullable);
                    return true;
                }
            }
        }

        // None...
        value = null!;
        isnullable = false;
        return false;
    }

    /// <summary>
    /// Invoked to find the return type from the <see cref="CloneableAttribute.ReturnType"/>
    /// property, or from the <see cref="CloneableAttribute{T}"/> one, applied to the given type,
    /// or to the first valid one in the given chains.
    /// </summary>
    static bool FindReturnTypeValue(
        INamedTypeSymbol type,
        out INamedTypeSymbol value,
        out bool isnullable,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        value = null!;
        isnullable = false;

        var found = FindCloneableAttribute(type, out var data, chains);
        if (found) found = FindReturnTypeValue(data, out value, out isnullable);
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find the return type from the <see cref="CloneableAttribute.VirtualMethod"/>
    /// property, or from the <see cref="CloneableAttribute{T}"/> one.
    /// </summary>
    static bool FindVirtualMethodValue(AttributeData data, out bool value)
    {
        if (data.GetNamedArgument(nameof(CloneableAttribute.VirtualMethod), out var arg))
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

    /// <summary>
    /// Invoked to find the return type from the <see cref="CloneableAttribute.VirtualMethod"/>
    /// property, or from the <see cref="CloneableAttribute{T}"/> one, applied to the given type,
    /// or to the first valid one in the given chains.
    /// </summary>
    static bool FindVirtualMethodValue(
        INamedTypeSymbol type,
        out bool value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        value = false;

        var found = FindCloneableAttribute(type, out var data, chains);
        if (found) found = FindVirtualMethodValue(data, out value);
        return found;
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(CloneGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated code.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{AttributeDoc}}
        """);
}