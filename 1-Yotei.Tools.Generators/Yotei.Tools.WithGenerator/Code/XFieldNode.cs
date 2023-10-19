namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XFieldNode : FieldNode
{
    public XFieldNode(TypeNode parent, FieldCandidate candidate) : base(parent, candidate) { }
    public XFieldNode(TypeNode parent, IFieldSymbol symbol) : base(parent, symbol) { }

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
        // Base validations..
        if (!base.Validate(context)) return false;
        if (!Parent.Symbol.ValidateNotRecord(context)) return false;

        // Base validations..
        if (!base.Validate(context)) return false;
        if (!Parent.Symbol.ValidateNotRecord(context)) return false;

        // Needs to be writable
        if (!Parent.IsInterface && (
            Symbol.IsConst || Symbol.IsReadOnly || Symbol.HasConstantValue))
        {
            context.ErrorFieldNotWritable(Symbol);
            return false;
        }

        // Validations passed...
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
        // Intercepting explicit elements...
        if (HasMethod(Parent.Symbol) != null) return;

        // Initiating...
        var parentType = Parent.Symbol.FullyQualifiedName(addNullable: false);
        var memberType = Symbol.Type.FullyQualifiedName(addNullable: true);
        var valueName = $"v_{Symbol.Name}";
        var modifiers = GetModifiers(); if (modifiers != null) modifiers += " ";
        PrintDocumentation(cb, valueName);

        // Interfaces...
        if (Parent.IsInterface)
        {
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName});");
            return;
        }

        // Abstract...
        if (Parent.Symbol.IsAbstract)
        {
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName});");
        }

        // Regular...
        else
        {
            var builder = new TypeBuilder(context, Parent.Symbol);
            var enforced = new EnforcedMember(Symbol, valueName);
            var underscores = ObtainIncludeUnderscores(out var utemp, out _) && utemp;
            var specs = ObtainSpecs(out var stemp, out _) ? stemp : null;
            var receiver = "v_temp";
            var code = builder.GetCode(receiver, specs, enforced, underscores);

            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName})");
            cb.AppendLine("{");
            cb.IndentLevel++;

            if (code == null)
            {
                cb.AppendLine("throw new NotImplementedException();");
            }
            else if (code == "this")
            {
                cb.AppendLine($"{Symbol.Name} = {valueName};");
                cb.AppendLine($"return this;");
            }
            else
            {
                cb.Append(code);
                cb.AppendLine($"return {receiver};");
            }

            cb.IndentLevel--;
            cb.AppendLine("}");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the appropriate documentation.
    /// </summary>
    void PrintDocumentation(CodeBuilder cb, string name) => cb.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the hosting type where the value of the decorated member
        /// '{{Symbol.Name}}' has been replaced by the value obtained from the given variable
        /// '{{name}}'.
        /// </summary>
        /// <param name="{{name}}"></param>
        /// <returns></returns>
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string with the appropriate modifiers, or null if any.
    /// </summary>
    /// <returns></returns>
    string? GetModifiers()
    {
        // Interfaces...
        if (Parent.IsInterface)
        {
            return Parent.Symbol.AllInterfaces.Any(x =>
                HasDecoratedMember(x) != null ||
                HasMethod(x) != null)
                ? "new"
                : null;
        }

        // Implementation...
        else
        {
            if (Parent.Symbol.IsAbstract) return "public abstract";

            var prevent = ObtainPreventVirtual(out var temp, out _) && temp;
            var times = GetChainTimes(Parent.Symbol, true);
            if (times)
            {
                return !prevent ? "public override" : "public new";
            }

            if (Parent.Symbol.IsSealed) return "public";
            return !prevent ? "public virtual" : "public";
        }

        // Computes the number of times in the chain...
        bool GetChainTimes(ITypeSymbol type, bool top)
        {
            if (!top)
            {
                var method = HasMethod(type); if (method != null) return true;
                var member = HasDecoratedMember(type); if (member != null) return true;
                if (type.HasAttributes(WithGeneratorAttr.LongName) && HasMember(type) != null) return true;
            }
            var parent = type.BaseType;
            return parent == null ? false : GetChainTimes(parent, false);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type has a member with the appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IFieldSymbol? HasMember(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name);
    }

    /// <summary>
    /// Determines if the type has a *decorated* member with the appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IFieldSymbol? HasDecoratedMember(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.HasAttributes(WithGeneratorAttr.LongName));
    }

    /// <summary>
    /// Determines if the type has a compatible 'With' member, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IMethodSymbol? HasMethod(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.Specs"/> setting, provided the
    /// symbol is decorated appropriately.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inherited"></param>
    /// <returns></returns>
    bool ObtainSpecs(out string? value, out bool inherited)
    {
        inherited = false;
        if (WithGeneratorAttr.GetSpecs(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetSpecs(Parent.Symbol, out value)) return true;

        inherited = true;
        foreach (var type in Parent.Symbol.AllBaseTypes())
        {
            var member = HasDecoratedMember(type);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(type, out value)) return true;
        }

        inherited = true;
        foreach (var type in Parent.Symbol.AllInterfaces)
        {
            var member = HasDecoratedMember(type);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(type, out value)) return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.IncludeUnderscores"/> setting,
    /// provided the symbol is decorated appropriately.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inherited"></param>
    /// <returns></returns>
    bool ObtainIncludeUnderscores(out bool value, out bool inherited)
    {
        inherited = false;
        if (WithGeneratorAttr.GetIncludeUnderscores(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetIncludeUnderscores(Parent.Symbol, out value)) return true;

        inherited = true;
        foreach (var type in Parent.Symbol.AllBaseTypes())
        {
            var member = HasDecoratedMember(type);
            if (member != null &&
                WithGeneratorAttr.GetIncludeUnderscores(member, out value)) return true;

            if (WithGeneratorAttr.GetIncludeUnderscores(type, out value)) return true;
        }

        inherited = true;
        foreach (var type in Parent.Symbol.AllInterfaces)
        {
            var member = HasDecoratedMember(type);
            if (member != null &&
                WithGeneratorAttr.GetIncludeUnderscores(member, out value)) return true;

            if (WithGeneratorAttr.GetIncludeUnderscores(type, out value)) return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.PreventVirtual"/> setting,
    /// provided the symbol is decorated appropriately.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inherited"></param>
    /// <returns></returns>
    bool ObtainPreventVirtual(out bool value, out bool inherited)
    {
        inherited = false;
        if (WithGeneratorAttr.GetPreventVirtual(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetPreventVirtual(Parent.Symbol, out value)) return true;

        inherited = true;
        foreach (var type in Parent.Symbol.AllBaseTypes())
        {
            var member = HasDecoratedMember(type);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(type, out value)) return true;
        }

        inherited = true;
        foreach (var type in Parent.Symbol.AllInterfaces)
        {
            var member = HasDecoratedMember(type);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(type, out value)) return true;
        }

        return false;
    }
}