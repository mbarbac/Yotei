namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }
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
        if (!ParentNode.Symbol.IsInterface() &&
            !context.PropertyHasSetter(Symbol)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicit implementation...
        if (HasMethod(ParentNode.Symbol) != null) return;

        // Documentation...
        var vname = $"v_{Symbol.Name}";
        PrintDocumentation(cb, vname);

        // Emitting code...
        var options = new EasyNameOptions(
            fullTypeName: true,
            typeParameters: true,
            nullableAnnotation: false);
        var parentType = ParentNode.Symbol.EasyName(options);
        var memberType = Symbol.Type.EasyName(options with { NullableAnnotation = true });

        var modifiers = GetModifiers();
        var specs = GetSpecs(out var temp) ? temp.NullWhenEmpty() : null;
        var comp = StringComparison.OrdinalIgnoreCase;

        if (ParentNode.Symbol.IsInterface()) { PrintInterface(); return; }

        if (string.Compare(specs, "this", comp) == 0) PrintThis();
        else if (string.Compare(specs, "base", comp) == 0) PrintBase();
        else if (specs is null || string.Compare(specs, "copy", comp) == 0) PrintCopy();
        else { context.InvalidSpecs(Symbol, specs); return; }

        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            parentType = iface.EasyName(options);
            var element = HasMember(iface);
            var elementType = element != null ? element.Type.EasyName(options with { }) : memberType;

            cb.AppendLine();
            cb.AppendLine($"{parentType}");
            cb.AppendLine($"{parentType}.{MethodName}({elementType} value) => {MethodName}(value);");
        }

        /// <summary>
        /// Emits code when the host instance is an interface...
        /// </summary>
        void PrintInterface()
        {
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {vname});");
        }

        /// <summary>
        /// Emits code when modifying the member of the calling instance...
        /// </summary>
        void PrintThis()
        {
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {vname})");
            cb.AppendLine("{");
            cb.IndentLevel++;

            cb.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
            cb.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
            cb.AppendLine();

            cb.AppendLine($"{Symbol.Name} = {vname};");
            cb.AppendLine("return this;");

            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        /// <summary>
        /// Emits code when we shall invoke a base method...
        /// </summary>
        void PrintBase()
        {
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {vname})");
            cb.AppendLine("{");
            cb.IndentLevel++;

            var chain = FindBaseChain(); if (chain == null)
            {
                cb.AppendLine("throw new NotImplementedException();");
                context.NoBaseMethod(Symbol);
            }
            else
            {
                cb.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
                cb.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
                cb.AppendLine();

                cb.AppendLine($"var v_temp = {chain}.{MethodName}({vname});");
                cb.AppendLine($"return ({ParentNode.Symbol.Name})v_temp;");
            }

            cb.IndentLevel--;
            cb.AppendLine("}");

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

        /// <summary>
        /// Emits code when the host instance is an abstract type...
        /// </summary>
        void PrintAbstract()
        {
            cb.AppendLine($"public abstract {parentType}");
            cb.AppendLine($"{MethodName}({memberType} {vname});");
        }

        /// <summary>
        /// Emits code when we shall use a copy constructor (the default)...
        /// </summary>
        void PrintCopy()
        {
            if (ParentNode.Symbol.IsAbstract) { PrintAbstract(); return; }

            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {vname})");
            cb.AppendLine("{");
            cb.IndentLevel++;

            var method =
                ParentNode.Symbol.GetCopyConstructor(true) ??
                ParentNode.Symbol.GetCopyConstructor(false);

            if (method == null)
            {
                cb.AppendLine("throw new NotImplementedException();");
                context.NoCopyConstructor(ParentNode.Symbol);
            }
            else
            {
                var vtemp = "v_temp";

                cb.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
                cb.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
                cb.AppendLine();

                var options = new EasyNameOptions(
                    fullTypeName: false,
                    typeParameters: true,
                    nullableAnnotation: false);
                var name = ParentNode.Symbol.EasyName(options);

                cb.AppendLine($"var {vtemp} = new {name}(this)");
                cb.AppendLine("{");

                cb.IndentLevel++;
                cb.AppendLine($"{Symbol.Name} = {vname}");
                cb.IndentLevel--;

                cb.AppendLine("};");
                cb.AppendLine($"return {vtemp};");
            }

            cb.IndentLevel--;
            cb.AppendLine("}");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints documentation.
    /// </summary>
    /// <param name="cb"></param>
    /// <param name="vname"></param>
    void PrintDocumentation(CodeBuilder cb, string vname) => cb.AppendLine($$"""
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
            var prevent = GetPreventVirtual(out var temp) && temp;
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
        var list = new CustomList<ITypeSymbol>()
        {
            Comparer = SymbolEqualityComparer.Default.Equals,
            CanInclude = (@this, item) => @this.IndexOf(item) < 0,
        };

        foreach (var iface in ParentNode.Symbol.Interfaces) Populate(iface);
        return list.ToArray();

        // Captures all suitable interfaces in the inheritance chain...
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
    /// Tries to get the effective value of the <see cref="WithGeneratorAttr.Specs"/> setting.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetSpecs(out string? value)
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
    /// Tries to get the effective value of the <see cref="WithGeneratorAttr.PreventVirtual"/> setting.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetPreventVirtual(out bool value)
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