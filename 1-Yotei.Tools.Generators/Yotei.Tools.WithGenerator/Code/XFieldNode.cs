namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc/>
internal class XFieldNode : FieldNode
{
    public XFieldNode(TypeNode parent, IFieldSymbol symbol) : base(parent, symbol) { }
    public XFieldNode(TypeNode parent, FieldCandidate candidate) : base(parent, candidate) { }

    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (!context.TypeIsNotRecord(ParentNode.Symbol)) return false;

        if (!ParentNode.Symbol.IsInterface() &&
            !context.FieldIsWrittable(Symbol)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicit implementation...
        if (HasMethod(ParentNode.Symbol, recursive: false)) return;

        // Emitting code...
        PrintDocumentation(cb);
        if (ParentNode.Symbol.IsInterface()) PrintInterface(context, cb);
        else if (ParentNode.Symbol.IsAbstract) PrintAbstract(context, cb);
        else
        {
            var specs = GetSpecs(ParentNode.Symbol, out var temp) ? temp.NullWhenEmpty() : null;
            var comp = StringComparison.OrdinalIgnoreCase;

            if (string.Compare(specs, "this", comp) == 0) PrintThisBuilder(context, cb);
            else if (string.Compare(specs, "base", comp) == 0) PrintBaseBuilder(context, cb);
            else if (specs == null || string.Compare(specs, "copy", comp) == 0) PrintCopyBuilder(context, cb);
            else context.InvalidSpecs(Symbol, specs);
        }
    }

    /// <summary>
    /// Case: host type is an interface...
    /// </summary>
    /// <param name="_"></param>
    /// <param name="cb"></param>
    void PrintInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();

        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(
            typeFullName: false,
            typeGenerics: true,
            typeNullable: false));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            typeFullName: true,
            typeGenerics: true,
            typeNullable: true));

        cb.AppendLine("[Yotei.Tools.WithGenerator.YoteiGenerated]");
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName});");
    }

    /// <summary>
    /// Case: host type is abstract...
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void PrintAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(
            typeFullName: false,
            typeGenerics: true,
            typeNullable: false));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            typeFullName: true,
            typeGenerics: true,
            typeNullable: true));

        cb.AppendLine("[Yotei.Tools.WithGenerator.YoteiGenerated]");
        cb.AppendLine($"public abstract {parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName});");
        PrintNeededInterfaces(context, cb);
    }

    /// <summary>
    /// Case: "copy" specs...
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void PrintCopyBuilder(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();

        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(
            typeFullName: false,
            typeGenerics: true,
            typeNullable: false));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            typeFullName: true,
            typeGenerics: true,
            typeNullable: true));

        cb.AppendLine("[Yotei.Tools.WithGenerator.YoteiGenerated]");
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;

        var parent = ParentNode.Symbol;
        var method = parent.GetCopyConstructor(true) ?? parent.GetCopyConstructor(false);
        if (method == null)
        {
            cb.AppendLine("throw new NotImplementedException();");
            context.NoCopyConstructor(parent);
        }
        else
        {
            var vtemp = "v_temp";

            cb.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
            cb.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {ArgumentName})) return this;");
            cb.AppendLine();

            cb.AppendLine($"var {vtemp} = new {parentType}(this)"); // Copy constructor...
            cb.AppendLine("{");

            cb.IndentLevel++;
            cb.AppendLine($"{Symbol.Name} = {ArgumentName}");
            cb.IndentLevel--;

            cb.AppendLine("};");
            cb.AppendLine($"return {vtemp};");
        }

        cb.IndentLevel--;
        cb.AppendLine("}");
        PrintNeededInterfaces(context, cb);
    }

    /// <summary>
    /// Case: "this" specs...
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void PrintThisBuilder(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();

        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(
            typeFullName: false,
            typeGenerics: true,
            typeNullable: false));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            typeFullName: true,
            typeGenerics: true,
            typeNullable: true));

        cb.AppendLine("[Yotei.Tools.WithGenerator.YoteiGenerated]");
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;

        cb.AppendLine($"{Symbol.Name} = {ArgumentName};"); // This instance...
        cb.AppendLine("return this;");

        cb.IndentLevel--;
        cb.AppendLine("}");
        PrintNeededInterfaces(context, cb);
    }

    /// <summary>
    /// Case: "base" specs...
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void PrintBaseBuilder(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();

        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(
            typeFullName: false,
            typeGenerics: true,
            typeNullable: false));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            typeFullName: true,
            typeGenerics: true,
            typeNullable: true));

        cb.AppendLine("[Yotei.Tools.WithGenerator.YoteiGenerated]");
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;

        var parent = ParentNode.Symbol.BaseType;
        if (parent == null || !HasBaseMethod(out var chain))
        {
            cb.AppendLine("throw new NotImplementedException();");
            context.NoBaseMethod(Symbol, parentType);
        }
        else
        {
            var vtemp = "v_temp";
            cb.AppendLine($"var {vtemp} = {chain}.{MethodName}({ArgumentName});"); // Base...
            cb.AppendLine($"return ({parentType}){vtemp};");
        }

        cb.IndentLevel--;
        cb.AppendLine("}");
        PrintNeededInterfaces(context, cb);
    }

    bool HasBaseMethod(out string? chain)
    {
        var num = 0;
        var temp = ParentNode.Symbol.BaseType; while (temp != null)
        {
            if (HasDecoratedMember(temp, recursive: false) ||
                HasMethod(temp, recursive: false) ||
                (HasMember(temp, recursive: false) && temp.HasAttributes(WithGeneratorAttr.LongName)))
            {
                num++;
                chain = string.Join(".", Enumerable.Repeat("base", num));
                return true;
            }

            temp = temp.BaseType;
        }

        chain = null;
        return false;
    }

    /// <summary>
    /// Emits interfaces that need implementation...
    /// </summary>
    /// <param name="_"></param>
    /// <param name="cb"></param>
    void PrintNeededInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            var parentType = iface.EasyName(new EasyNameOptions(
                typeFullName: true,
                typeGenerics: true,
                typeNullable: false));

            var member = GetTypeMember(iface);
            if (member == null)
            {
                foreach (var parent in iface.AllInterfaces)
                {
                    member = GetTypeMember(parent);
                    if (member != null) break;
                }

                if (member == null) throw new InvalidOperationException(
                    "Cannot find member in interface.")
                    .WithData(Symbol.Name)
                    .WithData(iface.Name);
            }

            var memberType = member.Type.EasyName(new EasyNameOptions(
                typeFullName: true,
                typeGenerics: true,
                typeNullable: true));

            cb.AppendLine();
            cb.AppendLine($"{parentType}");
            cb.AppendLine($"{parentType}.{MethodName}({memberType} value) => {MethodName}(value);");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints documentation.
    /// </summary>
    /// <param name="cb"></param>
    void PrintDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
    /// <summary>
    /// Returns an instance of the host type where the value of the decorated member has been
    /// replaced by the new given one.
    /// </summary>
    /// <param name ="{{ArgumentName}}"></param>
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
                HasDecoratedMember(x, recursive: false) ||
                HasMethod(x, recursive: false))
                ? "new "
                : null;
        }

        // Implementation...
        else
        {
            var prevent = GetPreventVirtual(ParentNode.Symbol, out var temp) && temp;
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
                    if (HasMethod(type, recursive: false)) return true;
                    else if (HasDecoratedMember(type, recursive: false)) return true;

                    else if (type.HasAttributes(WithGeneratorAttr.LongName) &&
                        HasMember(type, recursive: false))
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

            if (HasDecoratedMember(iface, recursive: false)) done = true;
            else if (HasMethod(iface, recursive: false)) done = true;

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
    IFieldSymbol? GetTypeMember(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IFieldSymbol>()
        .FirstOrDefault(x => x.Name == Symbol.Name);

    bool HasMember(ITypeSymbol type, bool recursive)
    {
        if (GetTypeMember(type) != null) return true;
        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return HasMember(parent, recursive);
        }
        return false;
    }

    /// <summary>
    /// Determines if the type has a DECORATED member with the appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IFieldSymbol? GetTypeDecoratedMember(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IFieldSymbol>()
        .FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.HasAttributes(WithGeneratorAttr.LongName));

    bool HasDecoratedMember(ITypeSymbol type, bool recursive)
    {
        if (GetTypeDecoratedMember(type) != null) return true;
        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return HasDecoratedMember(parent, recursive);
        }
        return false;
    }

    /// <summary>
    /// Determines if the type implements a compatible method, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IMethodSymbol? GetTypeMethod(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IMethodSymbol>()
        .FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

    bool HasMethod(ITypeSymbol type, bool recursive)
    {
        if (GetTypeMethod(type) != null) return true;
        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return HasMethod(parent, recursive);
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the effective value of the <see cref="WithGeneratorAttr.PreventVirtual"/>
    /// setting, starting at the given type and up through its inheritance chain.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetPreventVirtual(ITypeSymbol type, out bool value)
    {
        var member = GetTypeDecoratedMember(type);
        if (member != null &&
            WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

        if (WithGeneratorAttr.GetPreventVirtual(type, out value)) return true;

        foreach (var parent in type.AllBaseTypes())
        {
            member = GetTypeDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        foreach (var parent in type.AllInterfaces)
        {
            member = GetTypeDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get the effective value of the <see cref="WithGeneratorAttr.Specs"/>
    /// setting, starting at the given type and up through its inheritance chain.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetSpecs(ITypeSymbol type, out string? value)
    {
        var member = GetTypeDecoratedMember(type);
        if (member != null &&
            WithGeneratorAttr.GetSpecs(member, out value)) return true;

        if (WithGeneratorAttr.GetSpecs(type, out value)) return true;

        foreach (var parent in type.AllBaseTypes())
        {
            member = GetTypeDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        foreach (var parent in type.AllInterfaces)
        {
            member = GetTypeDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        return false;
    }
}