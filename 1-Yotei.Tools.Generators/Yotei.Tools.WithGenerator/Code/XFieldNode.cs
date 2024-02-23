namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="FieldNode"/>
internal class XFieldNode(
    TypeNode parent, IFieldSymbol symbol) : FieldNode(parent, symbol)
{
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (!ParentNode.Symbol.IsInterface() &&
            !context.FieldIsWrittable(Symbol)) return false;

        if (!base.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the documentation and decorator.
    /// </summary>
    /// <param name="cb"></param>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the host type where the value of the decorated member has been
        /// replaced by the new given one.
        /// </summary>
        /// <param name ="{{ArgumentName}}"></param>
        /// <returns></returns>
        {{XWithGenerator.YoteiGenerated}}
        """);

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        if (HasMethod(ParentNode.Symbol) != null) return; // Already implemented! 

        if (ParentNode.Symbol.IsInterface()) EmitInterface(context, cb);
        else if (ParentNode.Symbol.IsAbstract) EmitAbstract(context, cb);
        else
        {
            var specs = GetSpecs(ParentNode.Symbol, out var temp) ? temp.NullWhenEmpty() : null;
            var comp = StringComparison.OrdinalIgnoreCase;

            if (string.Compare(specs, "this", comp) == 0) EmitThisBuilder(context, cb);
            else if (string.Compare(specs, "base", comp) == 0) EmitBaseBuilder(context, cb);
            else if (specs == null || string.Compare(specs, "copy", comp) == 0) EmitCopyBuilder(context, cb);
            else context.InvalidSpecs(Symbol, specs);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Case: type is interface.
    /// </summary>
    void EmitInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            useFullName: true,
            addNullable: true,
            useGenerics: true));

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName});");
    }

    /// <summary>
    /// Case: type is abstract.
    /// </summary>
    void EmitAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            useFullName: true,
            addNullable: true,
            useGenerics: true));

        EmitDocumentation(cb);
        cb.AppendLine($"public abstract {parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName});");
    }

    /// <summary>
    /// Case: type is class or struct, "copy" specs.
    /// </summary>
    void EmitCopyBuilder(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            useFullName: true,
            addNullable: true,
            useGenerics: true));

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;

        var parent = ParentNode.Symbol;
        var method = parent.GetCopyConstructor(true) ?? parent.GetCopyConstructor(false);
        if (method == null)
        {
            cb.AppendLine("// Cannot find a copy constructor.");
            context.NoCopyConstructor(parent);
        }
        else // Using the copy constructor...
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
    }

    /// <summary>
    /// Case: type is class or struct, "this" specs.
    /// </summary>
    void EmitThisBuilder(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            useFullName: true,
            addNullable: true,
            useGenerics: true));

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;

        cb.AppendLine($"{Symbol.Name} = {ArgumentName};"); // This instance...
        cb.AppendLine("return this;");

        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Case: type is class or struct, "base" specs.
    /// </summary>
    void EmitBaseBuilder(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var parentType = ParentNode.Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var memberType = Symbol.Type.EasyName(new EasyNameOptions(
            useFullName: true,
            addNullable: true,
            useGenerics: true));

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;

        var parent = ParentNode.Symbol.BaseType;
        if (parent == null || !HasBaseMethod(out var chain))
        {
            cb.AppendLine("// Cannot find a base method to invoke.");
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
    }

    bool HasBaseMethod(out string? chain)
    {
        var num = 0;
        var temp = ParentNode.Symbol.BaseType; while (temp != null)
        {
            if (HasDecoratedMember(temp) != null ||
                HasMethod(temp) != null || (
                HasMember(temp) != null && temp.HasAttributes(WithGeneratorAttr.LongName)))
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

        // Others...
        else
        {
            var prevent = GetPreventVirtual(ParentNode.Symbol, out var temp) && temp;
            var appears = AppearsInChain(ParentNode.Symbol.BaseType);

            if (appears)
            {
                return prevent ? "public new " : "public override ";
            }
            else
            {
                return Symbol.IsSealed || prevent
                    ? "public "
                    : "public virtual ";
            }

            // Determines if appears in chain...
            bool AppearsInChain(ITypeSymbol? type)
            {
                if (type is null) return false;

                if (HasMethod(type) != null) return true;
                if (HasDecoratedMember(type) != null) return true;
                if (type.HasAttributes(WithGeneratorAttr.LongName) && HasMember(type) != null)
                    return true;

                return AppearsInChain(type.BaseType);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type has already implemented a corresponding method.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    IMethodSymbol? HasMethod(ITypeSymbol type, bool recursive = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

        if (item != null) return item;

        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return HasMethod(parent, recursive);
        }

        return null;
    }

    /// <summary>
    /// Determines if the type has a corresponding member defined, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    IFieldSymbol? HasMember(ITypeSymbol type, bool recursive = false)
    {
        var item = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name);

        if (item != null) return item;

        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return HasMember(parent, recursive);
        }

        return null;
    }

    /// <summary>
    /// Determines if the type has a corresponding decorated member defined, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    IFieldSymbol? HasDecoratedMember(ITypeSymbol type, bool recursive = false)
    {
        var item = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.HasAttributes(WithGeneratorAttr.LongName));

        if (item != null) return item;

        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return HasDecoratedMember(parent, recursive);
        }

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the effective value of the <see cref="WithGeneratorAttr.PreventVirtual"/> setting,
    /// using the given type an its appropriate inheritance chain.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetPreventVirtual(ITypeSymbol type, out bool value)
    {
        // The symbol itself...
        var member = HasDecoratedMember(type);
        if (member != null &&
            WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

        if (WithGeneratorAttr.GetPreventVirtual(type, out value)) return true;

        // It might be defined in the inheritance hierarchy...
        foreach (var parent in type.AllBaseTypes())
        {
            member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        foreach (var parent in type.AllInterfaces)
        {
            member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        // Not defined...
        return false;
    }

    /// <summary>
    /// Gets the effective value of the <see cref="WithGeneratorAttr.Specs"/> setting,
    /// using the given type an its appropriate inheritance chain.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetSpecs(ITypeSymbol type, out string? value)
    {
        // The symbol itself...
        var member = HasDecoratedMember(type);
        if (member != null &&
            WithGeneratorAttr.GetSpecs(member, out value)) return true;

        if (WithGeneratorAttr.GetSpecs(type, out value)) return true;

        // It might be defined in the inheritance hierarchy...
        foreach (var parent in type.AllBaseTypes())
        {
            member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        foreach (var parent in type.AllInterfaces)
        {
            member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        // Not defined...
        return false;
    }
}