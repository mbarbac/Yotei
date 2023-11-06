namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XProperty : PropertyNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    public XProperty(PropertyCandidate candidate) : base(candidate) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    public XProperty(IPropertySymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (!context.TypeIsNotRecord(HostType)) return false;

        if (!context.PropertyHasGetter(Symbol)) return false;

        if (!HostType.IsInterface() &&
            !context.PropertyHasSetter(Symbol)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The name of the method to generate.
    /// </summary>
    string MethodName => $"With{Symbol.Name}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    public override void Print(SourceProductionContext context, FileBuilder file)
    {
        if (HasMethod(HostType) != null) return; // Intercepting explicit implementation...

        var parentType = HostType.FullyQualifiedName(addNullable: false);
        var memberType = Symbol.Type.FullyQualifiedName(addNullable: true);
        var vname = $"v_{Symbol.Name}";
        PrintDocumentation(file, vname);
        
        var modifiers = GetModifiers();
        if (modifiers != null) modifiers += " ";

        if (HostType.IsInterface())
        {
            PrintInterface();
            return;
        }

        var specs = TryGetSpecs(out var temp) ? temp : null;
        var comparison = StringComparison.OrdinalIgnoreCase;

        if (string.Compare(specs, "this", comparison) == 0) PrintThis();
        else if (string.Compare(specs, "base", comparison) == 0) PrintBase();
        else if (specs is null || string.Compare(specs, "copy", comparison) == 0) PrintRegular();
        else
        {
            context.InvalidSpecs(Symbol, specs);
            return;
        }

        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            parentType = iface.FullyQualifiedName(addNullable: false);
            memberType = GetMemberTypeOn(iface);

            file.AppendLine();
            file.AppendLine($"{parentType}");
            file.AppendLine($"{parentType}.{MethodName}({memberType} value) => {MethodName}(value);");
        }

        /// <summary>
        /// Invoked to emit code when the host is an interface...
        /// </summary>
        void PrintInterface()
        {
            file.AppendLine($"{modifiers}{parentType}");
            file.AppendLine($"{MethodName}({memberType} {vname});");
        }

        /// <summary>
        /// Invoked to emit code when the specs equals to "this"...
        /// </summary>
        void PrintThis()
        {
            file.AppendLine($"{modifiers}{parentType}");
            file.AppendLine($"{MethodName}({memberType} {vname})");
            file.AppendLine("{");
            file.IndentLevel++;

            file.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
            file.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
            file.AppendLine();

            file.AppendLine($"{Symbol.Name} = {vname};");
            file.AppendLine("return this;");

            file.IndentLevel--;
            file.AppendLine("}");
        }

        /// <summary>
        /// Invoked to emit code when the specs equals to "base"...
        /// </summary>
        void PrintBase()
        {
            file.AppendLine($"{modifiers}{parentType}");
            file.AppendLine($"{MethodName}({memberType} {vname})");
            file.AppendLine("{");
            file.IndentLevel++;

            var chain = FindBase(HostType, 0); if (chain == null)
            {
                file.AppendLine("throw new NotImplementedException();");
                context.NoBaseFound(Symbol);
            }
            else
            {
                file.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
                file.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
                file.AppendLine();

                file.AppendLine($"{chain}.{MethodName}({vname});");
                file.AppendLine("return this;");
            }

            file.IndentLevel--;
            file.AppendLine("}");

            // Returns the number of 'base' elements to use, or null if any.
            string? FindBase(ITypeSymbol type, int num)
            {
                num++;
                var parent = type.BaseType; if (parent != null)
                {
                    if (HasDecoratedMember(parent) != null ||
                        HasMethod(parent) != null ||
                        (parent.HasAttributes(WithGeneratorAttr.LongName) && HasMember(parent) != null))
                    {
                        var array = new string[num];
                        for (int i = 0; i < num; i++) array[i] = "base";
                        return string.Join(".", array);
                    }
                    return FindBase(parent, num);
                }
                return null;
            }
        }

        /// <summary>
        /// Invoked to emit code when the specs are the default ones...
        /// </summary>
        void PrintRegular()
        {
            if (HostType.IsAbstract)
            {
                file.AppendLine($"public abstract {parentType}");
                file.AppendLine($"{MethodName}({memberType} {vname});");
            }
            else
            {
                file.AppendLine($"{modifiers}{parentType}");
                file.AppendLine($"{MethodName}({memberType} {vname})");
                file.AppendLine("{");
                file.IndentLevel++;

                var method = HostType.GetCopyConstructor(true) ?? HostType.GetCopyConstructor(false);
                if (method == null)
                {
                    file.AppendLine("throw new NotImplementedException();");
                    context.NoCopyConstructor(HostType);
                }
                else
                {
                    var vtemp = "v_temp";

                    file.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
                    file.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
                    file.AppendLine();

                    file.AppendLine($"var {vtemp} = new {HostType.Name}(this)");
                    file.AppendLine("{");

                    file.IndentLevel++;
                    file.AppendLine($"{Symbol.Name} = {vname}");
                    file.IndentLevel--;

                    file.AppendLine("};");
                    file.AppendLine($"return {vtemp};");
                }

                file.IndentLevel--;
                file.AppendLine("}");
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the method documentation.
    /// </summary>
    void PrintDocumentation(FileBuilder file, string vname) => file.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the host type where the value of the decorated member
        /// '<see cref="{{Symbol.Name}}"/>' has been replaced by the new given one.
        /// </summary>
        /// <param name="{{vname}}"></param>
        /// <returns></returns>
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate method modifiers, or null if any.
    /// </summary>
    string? GetModifiers()
    {
        // Interfaces...
        if (HostType.IsInterface())
        {
            return HostType.AllInterfaces.Any(x =>
                HasDecoratedMember(x) != null ||
                HasMethod(x) != null)
                ? "new"
                : null;
        }

        // Implementation...
        else
        {
            var prevent = TryGetPreventVirtual(out var temp) && temp;
            var appears = AppearsInChain(HostType, true);
            if (appears)
            {
                return !prevent ? "public override" : "public new";
            }
            return !prevent ? "public virtual" : "public";
        }

        // Determines if appears in the chain...
        bool AppearsInChain(ITypeSymbol type, bool top)
        {
            if (!top)
            {
                if (HasMethod(type) != null) return true;
                if (HasDecoratedMember(type) != null) return true;
                if (type.HasAttributes(WithGeneratorAttr.LongName) && HasMember(type) != null) return true;
            }
            var parent = type.BaseType;
            return parent == null ? false : AppearsInChain(parent, false);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the interfaces that need implementation.
    /// </summary>
    ITypeSymbol[] GetInterfacesToImplement()
    {
        var list = new CoreList<ITypeSymbol>()
        {
            AcceptDuplicate = (item) => false,
            Compare = SymbolEqualityComparer.Default.Equals,
        };

        foreach (var iface in HostType.Interfaces) Populate(iface);
        return list.ToArray();

        // Needs to capture all suitable interfaces in the chain...
        bool Populate(ITypeSymbol iface)
        {
            var done = false;

            if (HasDecoratedMember(iface) != null) done = true;
            if (HasMethod(iface) != null) done = true;

            foreach (var child in iface.Interfaces) if (Populate(child)) done = true;

            if (done) list.Add(iface);
            return done;
        }
    }

    /// <summary>
    /// Returns the member type as defined in the given interface, if such is possible.
    /// </summary>
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
    IPropertySymbol? HasMember(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name);
    }

    /// <summary>
    /// Determines if the type has a decorated member, or not.
    /// </summary>
    IPropertySymbol? HasDecoratedMember(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.HasAttributes(WithGeneratorAttr.LongName));
    }

    /// <summary>
    /// Determines if the type implements a compatible method, or not.
    /// </summary>
    IMethodSymbol? HasMethod(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.Specs"/> setting.
    /// </summary>
    bool TryGetSpecs(out string? value)
    {
        if (WithGeneratorAttr.GetSpecs(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetSpecs(HostType, out value)) return true;

        foreach (var parent in HostType.AllBaseTypes())
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        foreach (var parent in HostType.AllInterfaces)
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
    bool TryGetPreventVirtual(out bool value)
    {
        if (WithGeneratorAttr.GetPreventVirtual(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetPreventVirtual(HostType, out value)) return true;

        foreach (var parent in HostType.AllBaseTypes())
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        foreach (var parent in HostType.AllInterfaces)
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        return false;
    }
}