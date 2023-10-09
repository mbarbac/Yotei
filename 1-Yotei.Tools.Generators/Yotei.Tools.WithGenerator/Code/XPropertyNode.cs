using System.Data;

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }

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

        // Needs valid getter and setter...
        if (Symbol.GetMethod == null)
        {
            context.ErrorPropertyNoGetter(Symbol);
            return false;
        }
        if (Symbol.SetMethod == null && !Parent.IsInterface)
        {
            context.ErrorPropertyNoSetter(Symbol);
            return false;
        }

        // Passed...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the appropriate documentation.
    /// </summary>
    void PrintDocumentation(CodeBuilder cb, string name) => cb.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the hosting type where the value of the decorated member
        /// has been replaced by the new given one.
        /// </summary>
        /// <param name="{{name}}"></param>
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
        
        var valueName = $"v_{Symbol.Name}";
        var parentType = Parent.Symbol.FullyQualifiedName(addNullable: false);
        var memberType = Symbol.Type.FullyQualifiedName(addNullable: true);
        var modifiers = GetModifiers();
        PrintDocumentation(cb, valueName);

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

            var specs = WithGeneratorAttr.GetSpecs(Symbol);
            specs ??= WithGeneratorAttr.GetSpecs(Parent.Symbol);

            var builder = new TypeBuilder(context, Parent.Symbol);
            var underscores = IncludeUnderscores();
            var receiver = "v_temp";
            var enforced = new EnforcedMember(Symbol, valueName);
            var code = builder.GetCode(receiver, specs, enforced, underscores);

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
            else if (code == "base")
            {
                cb.AppendLine($"var {receiver} = base.{MethodName}({valueName});");
                cb.AppendLine($"return ({Parent.Symbol.Name}){receiver};");
            }
            else
            {
                cb.AppendLine(code);
                cb.AppendLine($"return {receiver};");
            }

            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        // Interfaces to implement...
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            parentType = iface.FullyQualifiedName(addNullable: false);
            memberType = GetMemberTypeOn(iface);

            cb.AppendLine();
            cb.AppendLine($"{parentType}");
            cb.AppendLine($"{parentType}.{MethodName}({memberType} value) => {MethodName}(value);");
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
    /// Returns a list with the interfaces that need implementation. This method is only called
    /// for implementation purposes.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetInterfacesToImplement()
    {
        var list = new NoDuplicatesList<ITypeSymbol>()
        {
            ThrowDuplicates = false,
            Comparer = SymbolEqualityComparer.Default.Equals,
        };

        foreach (var iface in Parent.Symbol.Interfaces) Populate(iface);
        return list.ToList();

        // Recursive...
        bool Populate(ITypeSymbol iface)
        {
            bool done = false;

            if (HasDecoratedMember(iface) != null) done = true;
            if (HasMethod(iface) != null) done = true;

            foreach (var child in iface.Interfaces)
            {
                if (Populate(child)) done = true;
            }

            if (done) list.Add(iface);
            return done;
        }
    }

    /// <summary>
    /// Returns the member type on the given interface
    /// </summary>
    /// <param name="iface"></param>
    /// <returns></returns>
    string GetMemberTypeOn(ITypeSymbol iface)
    {
        var member = HasMember(iface);
        if (member != null) return member.Type.FullyQualifiedName(addNullable: true);

        foreach (var child in iface.AllInterfaces)
        {
            member = HasMember(child);
            if (member != null) return member.Type.FullyQualifiedName(addNullable: true);
        }

        return Symbol.Type.FullyQualifiedName(addNullable: true);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has a member with an appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IPropertySymbol? HasMember(ITypeSymbol type)
    {
        return type.GetMembers()
            .OfType<IPropertySymbol>()
            .FirstOrDefault(x => x.Name == Symbol.Name);
    }

    /// <summary>
    /// Determines if the given type has a decorated member with an appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IPropertySymbol? HasDecoratedMember(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x => 
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

    // ----------------------------------------------------

    /// <summary>
    /// Determines the value of the <see cref="CloneableAttr.IncludeUnderscore"/> setting.
    /// </summary>
    /// <returns></returns>
    bool IncludeUnderscores()
    {
        return !Parent.IsInterface && Recursive(Parent.Symbol);

        bool Recursive(ITypeSymbol type)
        {
            var member = HasDecoratedMember(type);
            if (member != null &&
                WithGeneratorAttr.GetIncludeUnderscores(member)) return true;

            if (type.HasAttributes(WithGeneratorAttr.LongName) &&
                WithGeneratorAttr.GetIncludeUnderscores(type))
                return true;

            return type.BaseType != null && Recursive(type.BaseType);
        }
    }
}