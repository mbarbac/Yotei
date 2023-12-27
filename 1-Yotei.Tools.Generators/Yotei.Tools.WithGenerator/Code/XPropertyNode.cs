namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }

    /// <summary>
    /// The name of the method to generate.
    /// </summary>
    string MethodName => $"With{Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;
        if (!context.TypeIsNotRecord(ParentNode.Symbol)) return false;
        if (!context.PropertyHasGetter(Symbol)) return false;
        if (!ParentNode.Symbol.IsInterface() && !context.PropertyHasSetter(Symbol)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="builder"></param>
    public override void Print(SourceProductionContext context, CodeBuilder builder)
    {
        // Intercepting explicit implementation...
        if (HasMethod(ParentNode.Symbol) != null) return;

        // Common variables...
        var parentType = ParentNode.Symbol.FullyQualifiedName(addNullable: false);
        var memberType = Symbol.Type.FullyQualifiedName(addNullable: true);
        var vname = $"v_{Symbol.Name}";
        PrintDocumentation(builder, vname);

        var specs = TryGetSpecs(out var temp) ? temp.NullWhenEmpty() : null;
        var comparison = StringComparison.OrdinalIgnoreCase;

        // Interface...
        if (ParentNode.Symbol.IsInterface())
        {
            var modifiers = GetModifiers();

            builder.AppendLine($"{modifiers}{parentType}");
            builder.AppendLine($"{MethodName}({memberType} {vname});");
            return;
        }

        // THIS: modifiy the host instance...
        if (string.Compare(specs, "this", comparison) == 0)
        {
            var modifiers = GetModifiers();

            builder.AppendLine($"{modifiers}{parentType}");
            builder.AppendLine($"{MethodName}({memberType} {vname})");
            builder.AppendLine("{");
            builder.IndentLevel++;

            builder.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
            builder.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
            builder.AppendLine();

            builder.AppendLine($"{Symbol.Name} = {vname};");
            builder.AppendLine("return this;");

            builder.IndentLevel--;
            builder.AppendLine("}");
        }

        // BASE: use a base method...
        else if (string.Compare(specs, "base", comparison) == 0)
        {
            var modifiers = GetModifiers();

            builder.AppendLine($"{modifiers}{parentType}");
            builder.AppendLine($"{MethodName}({memberType} {vname})");
            builder.AppendLine("{");
            builder.IndentLevel++;

            var chain = FindBaseChain(); if (chain == null)
            {
                builder.AppendLine("throw new NotImplementedException();");
                context.NoBaseMethod(Symbol);
            }
            else
            {
                builder.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
                builder.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
                builder.AppendLine();

                builder.AppendLine($"var v_temp = {chain}.{MethodName}({vname});");
                builder.AppendLine($"return ({ParentNode.Symbol.Name})v_temp;");
            }

            builder.IndentLevel--;
            builder.AppendLine("}");

            string? FindBaseChain()
            {
                var parent = ParentNode.Symbol.BaseType;
                var num = parent == null ? 0 : FindBaseNum(parent);
                if (num > 0)
                {
                    var sb = new StringBuilder();
                    for (int i = 0; i < num; i++)
                    {
                        if (i > 0) sb.Append('.');
                        sb.Append("base");
                    }
                    return sb.ToString();
                }
                return null;
            }

            int FindBaseNum(ITypeSymbol type)
            {
                var parent = type.BaseType;
                var num = parent == null ? 0 : FindBaseNum(parent);

                if (HasDecoratedMember(type) != null ||
                    HasMethod(type) != null ||
                    (HasMember(type) != null && type.HasAttributes(WithGeneratorAttr.LongName)))
                {
                    num++;
                }
                return num;
            }
        }

        // COPY: use a copy constructor...
        else if (specs is null || string.Compare(specs, "copy", comparison) == 0)
        {
            if (ParentNode.Symbol.IsAbstract)
            {
                var modifiers = "public abstract ";

                builder.AppendLine($"{modifiers}{parentType}");
                builder.AppendLine($"{MethodName}({memberType} {vname});");
            }
            else
            {
                var modifiers = GetModifiers();

                builder.AppendLine($"{modifiers}{parentType}");
                builder.AppendLine($"{MethodName}({memberType} {vname})");
                builder.AppendLine("{");
                builder.IndentLevel++;

                var method =
                    ParentNode.Symbol.GetCopyConstructor(true) ??
                    ParentNode.Symbol.GetCopyConstructor(false);

                if (method == null)
                {
                    builder.AppendLine("throw new NotImplementedException();");
                    context.NoCopyConstructor(ParentNode.Symbol);
                }
                else
                {
                    var vtemp = "v_temp";

                    builder.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
                    builder.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
                    builder.AppendLine();

                    builder.AppendLine($"var {vtemp} = new {ParentNode.Symbol.Name}(this)");
                    builder.AppendLine("{");

                    builder.IndentLevel++;
                    builder.AppendLine($"{Symbol.Name} = {vname}");
                    builder.IndentLevel--;

                    builder.AppendLine("};");
                    builder.AppendLine($"return {vtemp};");
                }

                builder.IndentLevel--;
                builder.AppendLine("}");
            }
        }

        // Invalid specs...
        else
        {
            context.InvalidSpecs(Symbol, specs);
            return;
        }

        // Interfaces to implement...
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            parentType = iface.FullyQualifiedName(addNullable: false);
            memberType = GetMemberTypeOn(iface);

            builder.AppendLine();
            builder.AppendLine($"{parentType}");
            builder.AppendLine($"{parentType}.{MethodName}({memberType} value) => {MethodName}(value);");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints documentation.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="vname"></param>
    void PrintDocumentation(CodeBuilder builder, string vname) => builder.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the host type where the value of the decorated member has been
        /// replaced by the new given one.
        /// </summary>
        /// <param name ="{{vname}}"></param>
        /// <returns></returns>
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate method modifiers, or null if any.
    /// </summary>
    /// <returns></returns>
    string? GetModifiers()
    {
        // Interfaces...
        if (ParentNode.Symbol.IsInterface())
        {
            return ParentNode.Symbol.AllInterfaces.Any(x =>
                HasDecoratedMember(x) != null ||
                HasMethod(x) != null)
                ? "new "
                : null;
        }

        // Implementation...
        else
        {
            var prevent = TryGetPreventVirtual(out var temp) && temp;
            var appears = AppearsInChain(ParentNode.Symbol, true);
            if (appears)
            {
                return prevent ? "public new " : "public override ";
            }
            else
            {
                return ParentNode.Symbol.IsSealed || prevent
                    ? "public "
                    : "public virtual ";
            }

            // Determines if appears in chain...
            bool AppearsInChain(ITypeSymbol type, bool top)
            {
                if (!top)
                {
                    if (HasMethod(type) != null) return true;
                    else if (HasDecoratedMember(type) != null) return true;

                    else if (type.HasAttributes(WithGeneratorAttr.LongName) &&
                        HasMember(type) != null)
                        return true;
                }
                var parent = type.BaseType;
                return parent != null && AppearsInChain(parent, false);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of interfaces that need implementation.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("", "IDE0305")]
    ITypeSymbol[] GetInterfacesToImplement()
    {
        var list = new CustomList<ITypeSymbol>
        {
            OnAcceptDuplicate = (x, y) => false,
            OnCompare = SymbolEqualityComparer.Default.Equals,
        };

        foreach (var iface in ParentNode.Symbol.Interfaces) Populate(iface);
        return list.ToArray();

        // Need to capture all suitable interfaces to implement in the inheritance chain...
        bool Populate(ITypeSymbol iface)
        {
            var done = false;

            if (HasDecoratedMember(iface) != null) done = true;
            else if (HasMethod(iface) != null) done = true;

            foreach (var child in iface.Interfaces) if (Populate(child)) done = true;

            if (done) list.Add(iface);
            return done;
        }
    }

    /// <summary>
    /// Returns the type of the member as defined in the given interface.
    /// </summary>
    /// <param name="iface"></param>
    /// <returns></returns>
    string GetMemberTypeOn(ITypeSymbol iface)
    {
        var member = HasMember(iface);

        return member != null
            ? member.Type.FullyQualifiedName(addNullable: true)
            : Symbol.Type.FullyQualifiedName(addNullable: true);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type has a member with the appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IPropertySymbol? HasMember(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IPropertySymbol>()
        .FirstOrDefault(x => x.Name == Symbol.Name);

    /// <summary>
    /// Determines if the type has a DECORATED member with the appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IPropertySymbol? HasDecoratedMember(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IPropertySymbol>()
        .FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.HasAttributes(WithGeneratorAttr.LongName));

    /// <summary>
    /// Determines if the type implements a compatible method, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IMethodSymbol? HasMethod(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IMethodSymbol>()
        .FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.Specs"/> setting.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetSpecs(out string? value)
    {
        if (WithGeneratorAttr.GetSpecs(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetSpecs(ParentNode.Symbol, out value)) return true;

        foreach (var parent in ParentNode.Symbol.AllBaseTypes())
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        foreach (var parent in ParentNode.Symbol.AllInterfaces)
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.PreventVirtual"/> setting.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetPreventVirtual(out bool value)
    {
        if (WithGeneratorAttr.GetPreventVirtual(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetPreventVirtual(ParentNode.Symbol, out value)) return true;

        foreach (var parent in ParentNode.Symbol.AllBaseTypes())
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        foreach (var parent in ParentNode.Symbol.AllInterfaces)
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        return false;
    }
}