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
        if (!TypeNode.ValidateNotRecord(context, Parent.Symbol)) return false;

        // Needs to be writable
        if (!Parent.IsInterface && (
            Symbol.IsConst || Symbol.IsReadOnly || Symbol.HasConstantValue))
        {
            context.ErrorFieldNotWritable(Symbol);
            return false;
        }

        // Passed...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the appropriate documentation.
    /// </summary>
    void PrintDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the hosting type where the value of the decorated member
        /// has been replaced by the new given one.
        /// </summary>
        /// <returns></returns>
        """);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        // Initiating...
        if (HasMethod(Parent.Symbol) != null) return;
        PrintDocumentation(cb);

        var modifiers = GetModifiers();
        var valueName = $"v_{Symbol.Name}";
        var parentType = Parent.Symbol.FullyQualifiedName(addNullable: false);
        var memberType = Symbol.Type.FullyQualifiedName(addNullable: true);

        // Interfaces...
        if (Parent.IsInterface)
        {
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName});");
            return;
        }

        // Implementation...
        if (Parent.IsAbstract)
        {
            modifiers = "public abstract ";
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName});");
        }
        else
        {
            cb.AppendLine($"{modifiers}{parentType}");
            cb.AppendLine($"{MethodName}({memberType} {valueName})");
            cb.AppendLine("{");
            cb.IndentLevel++;

            cb.AppendLine("throw new NotImplementedException();");

            cb.IndentLevel--;
            cb.AppendLine("}");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string with the method modifiers, followed by a 'space' if needed.
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
            foreach (var type in Parent.Symbol.AllBaseTypes()) // First-pass...
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
                    return !WithGeneratorAttr.GetPreventVirtual(member)
                        ? "public override "
                        : "public new ";
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
                            return !WithGeneratorAttr.GetPreventVirtual(member)
                                ? "public override "
                                : "public new ";
                        }
                    }
                }
            }

            if (Parent.Symbol.IsSealed) return "public ";

            return !WithGeneratorAttr.GetPreventVirtual(Symbol)
                ? "public virtual "
                : "public ";
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
    /// Determines if the given type has a 'With' method already defined, with the appropriate
    /// name and compatible arguments, or not.
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
}