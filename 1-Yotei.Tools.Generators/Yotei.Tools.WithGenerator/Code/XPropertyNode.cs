namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc/>
internal class XPropertyNode(
    SemanticModel model, PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
    : PropertyNode(model, syntax, symbol)
{
    XProperty XMember => _XMember ??= new XProperty(Symbol);
    XProperty? _XMember;

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;
        if (!XMember.Validate(context)) return false;

        return true;
    }

    /// <inheritdoc/>
    public override void Emit(
        SourceProductionContext context, CodeBuilder cb) => XMember.Emit(context, cb);
}

// ========================================================
class XProperty(IPropertySymbol symbol)
{
    public IPropertySymbol Symbol { get; } = symbol.ThrowWhenNull();
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// Validates this member.
    /// </summary>
    public bool Validate(SourceProductionContext context)
    {
        if (!context.PropertyHasGetter(Symbol)) return false;
        if (!Symbol.ContainingType.IsInterface() &&
            !context.PropertyHasSetter(Symbol)) return false;

        return true;
    }

    // ----------------------------------------------------

    void PrintDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the host type where the value of the decorated member has been
        /// replaced by the new given one.
        /// </summary>
        /// <param name ="{{ArgumentName}}"></param>
        /// <returns></returns>
        """);

    /// <summary>
    /// Emits the source code for this element.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb)
    {
    }

    /// <summary>
    /// Case: interface
    /// </summary>
    void PrintInterface(SourceProductionContext context, CodeBuilder cb)
    {
    }

    /// <summary>
    /// Case: abstract
    /// </summary>
    void PrintAbstract(SourceProductionContext context, CodeBuilder cb)
    {
    }

    /// <summary>
    /// Case: regular copy builder
    /// </summary>
    void PrintRegularCopy(SourceProductionContext context, CodeBuilder cb)
    {
    }

    /// <summary>
    /// Case: regular this builder
    /// </summary>
    void PrintRegularThis(SourceProductionContext context, CodeBuilder cb)
    {
    }

    /// <summary>
    /// Case: regular base builder
    /// </summary>
    void PrintRegularBase(SourceProductionContext context, CodeBuilder cb)
    {
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the interfaces that need to be implemented.
    /// </summary>
    void PrintInterfacesToImplement(CodeBuilder cb)
    {
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate modifiers, or null if any.
    /// <br/> If so, an space is added to the returned string.
    /// </summary>
    string? GetModifiers()
    {
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the method implemented by the type, or null if any.
    /// </summary>
    IMethodSymbol? GetNodeMethod(ITypeSymbol type, bool recursive = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

        if (item != null) return item;

        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return GetNodeMethod(parent, recursive);
        }

        return null;
    }

    /// <summary>
    /// Returns the property defined in the given type, or null.
    /// </summary>
    IPropertySymbol? GetNodeMember(ITypeSymbol type, bool recursive = false)
    {
        var item = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name);

        if (item != null) return item;

        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return GetNodeMember(parent, recursive);
        }

        return null;
    }

    /// <summary>
    /// Returns the property defined in the given type, or null.
    /// </summary>
    IPropertySymbol? GetNodeDecoratedMember(ITypeSymbol type, bool recursive = false)
    {
        var item = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.HasAttributes(WithGeneratorAttr.LongName));

        if (item != null) return item;

        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return GetNodeDecoratedMember(parent, recursive);
        }

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the effective value of the <see cref="WithGeneratorAttr.PreventVirtual"/> setting.
    /// </summary>
    bool GetPreventVirtual(ITypeSymbol type, out bool value)
    {
        // In the host type itself...
        var member = GetNodeDecoratedMember(type);
        if (member != null &&
            WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

        if (WithGeneratorAttr.GetPreventVirtual(type, out value)) return true;

        // It might be defined in the inheritance hierarchy...
        foreach (var parent in type.AllBaseTypes())
        {
            member = GetNodeDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        foreach (var parent in type.AllInterfaces)
        {
            member = GetNodeDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        // Not defined...
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the effective value of the <see cref="WithGeneratorAttr.Specs"/> setting.
    /// </summary>
    bool GetPreventSpecs(ITypeSymbol type, out string? value)
    {
        // In the host type itself...
        var member = GetNodeDecoratedMember(type);
        if (member != null &&
            WithGeneratorAttr.GetSpecs(member, out value)) return true;

        if (WithGeneratorAttr.GetSpecs(type, out value)) return true;

        // It might be defined in the inheritance hierarchy...
        foreach (var parent in type.AllBaseTypes())
        {
            member = GetNodeDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        foreach (var parent in type.AllInterfaces)
        {
            member = GetNodeDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        // Not defined...
        return false;
    }
}