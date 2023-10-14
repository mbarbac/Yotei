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
        // Base validations...
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
        // Intercepting manual declaration or implementation...
        if (HasMethod(Parent.Symbol) != null) return;

        // Initiating...
        var modifiers = GetModifiers();
        var valueName = $"v_{Symbol.Name}";
        var parentType = Parent.Symbol.FullyQualifiedName(addNullable: false);
        var memberType = Symbol.Type.FullyQualifiedName(addNullable: true);
        PrintDocumentation(cb, valueName);

        // Interfaces...
        if (Parent.IsInterface)
        {
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName});");
            return;
        }

        // Abstract...
        if (Symbol.IsAbstract)
        {
            modifiers = "public abstract ";
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName});");
        }

        // Regular...
        else
        {
            if (Parent.Symbol.Name == "Manager") { } // STOPDEBUG

            var builder = new TypeBuilder(context, Parent.Symbol);
            var enforced = new EnforcedMember(Symbol, valueName);
            var underscores = GetUnderscores();
            var specs = GetSpecs();
            var receiver = "v_temp";
            var code = builder.GetCode(receiver, specs, enforced, underscores);

            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName})");
            cb.AppendLine("{");
            cb.IndentLevel++;

            if (code == null)
            {
                context.WarningCannotGenerateCode(Symbol);
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
    /// Returns a string with the appropriate method modifiers, followed by a 'space' if needed.
    /// </summary>
    /// <returns></returns>
    string GetModifiers()
    {
        // Interfaces...
        if (Parent.IsInterface)
        {
            return Parent.Symbol.AllInterfaces.Any(x =>
                HasDecoratedMember(x) != null ||
                HasMethod(x) != null)
                ? "new "
                : string.Empty;
        }

        // Implementation...
        else
        {
            bool value;

            foreach (var type in Parent.Symbol.AllBaseTypes())
            {
                var method = HasMethod(type);
                if (method != null)
                {
                    return method.IsVirtual || method.IsOverride || method.IsAbstract
                        ? "public override "
                        : "public new ";
                }

                var member = HasDecoratedMember(type);
                if (member != null)
                {
                    if (WithGeneratorAttr.GetPreventVirtual(member, out value))
                    {
                        return !value ? "public override " : "public new ";
                    }
                }

                if (type.HasAttributes(WithGeneratorAttr.LongName))
                {
                    foreach (var iface in type.AllInterfaces)
                    {
                        method = HasMethod(iface);
                        if (method != null)
                        {
                            return method.IsVirtual || method.IsOverride || method.IsAbstract
                                ? "public override "
                                : "public new ";
                        }

                        member = HasDecoratedMember(iface);
                        if (member != null)
                        {
                            if (WithGeneratorAttr.GetPreventVirtual(member, out value))
                            {
                                return !value ? "public override " : "public new ";
                            }
                        }
                    }
                }
            }

            if (Parent.Symbol.IsSealed) return "public ";

            WithGeneratorAttr.GetPreventVirtual(Symbol, out value);
            return !value ? "public virtual " : "public ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has a member with an appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IFieldSymbol? HasMember(ITypeSymbol type)
    {
        return type.GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(x => x.Name == Symbol.Name);
    }

    /// <summary>
    /// Determines if the given type has a decorated member with an appropriate name, or not.
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
    /// Determines if the given type has a 'With' method, or not.
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
    /// Returns the value of the <see cref="WithGeneratorAttr.Specs"/>.
    /// This method is only called for implementation purposes.
    /// </summary>
    /// <returns></returns>
    string? GetSpecs()
    {
        if (GetSpecs(Parent.Symbol, out var value)) return value;

        foreach (var type in Parent.Symbol.AllBaseTypes()) if (GetSpecs(type, out value)) return value;
        foreach (var iface in Parent.Symbol.AllInterfaces) if (GetSpecs(iface, out value)) return value;

        return null;
    }

    bool GetSpecs(ITypeSymbol type, out string? value)
    {
        var member = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x => x.Name == Symbol.Name);
        if (member != null &&
            WithGeneratorAttr.GetSpecs(member, out value)) return true;

        if (WithGeneratorAttr.GetSpecs(type, out value)) return true;

        value = null;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the value of the <see cref="WithGeneratorAttr.IncludeUnderscores"/>.
    /// This method is only called for implementation purposes.
    /// </summary>
    /// <returns></returns>
    bool GetUnderscores()
    {
        if (GetUnderscores(Parent.Symbol, out var value)) return value;

        foreach (var type in Parent.Symbol.AllBaseTypes()) if (GetUnderscores(type, out value)) return value;
        foreach (var iface in Parent.Symbol.AllInterfaces) if (GetUnderscores(iface, out value)) return value;

        return false;
    }

    bool GetUnderscores(ITypeSymbol type, out bool value)
    {
        var member = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x => x.Name == Symbol.Name);
        if (member != null &&
            WithGeneratorAttr.GetIncludeUnderscores(member, out value)) return true;

        if (WithGeneratorAttr.GetIncludeUnderscores(type, out value)) return true;

        value = false;
        return false;
    }
}