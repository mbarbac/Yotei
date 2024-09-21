namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    int Arity = 0;
    INamedTypeSymbol KType = null!;
    INamedTypeSymbol TType = null!;

    Type Template = null!;
    const string INameAttr = nameof(IFrozenListAttribute);
    const string CNameAttr = nameof(FrozenListAttribute);
    const string FrozenNamespace = "Yotei.Tools";

    string BName = null!;
    string KTypeName = null!;
    string TTypeName = null!;
    string SymbolName = null!;

    // -----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        AttributeData attr;

        // Capturing the unique attribute...
        if (Candidate != null)
        {
            if (Candidate.Attributes.Length == 0) return context.NoAttributes(Symbol);
            if (Candidate.Attributes.Length > 1) return context.ManyAttributes(Symbol);
            attr = Candidate.Attributes[0];
        }
        else
        {
            var attrs = Symbol.GetAttributes(typeof(IFrozenListAttribute<>));
            attrs.AddRange(Symbol.GetAttributes(typeof(IFrozenListAttribute<,>)));

            if (attrs.Count == 0) return context.NoAttributes(Symbol);
            if (attrs.Count > 1) return context.ManyAttributes(Symbol);
            attr = attrs[0];
        }

        // Capturing arity...
        if (attr.AttributeClass == null) return context.InvalidAttribute(Symbol);
        Arity = attr.AttributeClass.Arity;

        // Capturing KType (if any) and TType...
        if (Arity == 0)
        {
            if (attr.ConstructorArguments.Length == 1)
            {
                if ((TType = GetType(attr.ConstructorArguments[0])) == null) return context.InvalidAttribute(Symbol);
                Arity = 1;
            }
            else if (attr.ConstructorArguments.Length == 2)
            {
                if ((KType = GetType(attr.ConstructorArguments[0])) == null) return context.InvalidAttribute(Symbol);
                if ((TType = GetType(attr.ConstructorArguments[1])) == null) return context.InvalidAttribute(Symbol);
                Arity = 2;
            }
            else return context.InvalidAttribute(Symbol);

            static INamedTypeSymbol GetType(TypedConstant item)
                => !item.IsNull && item.Kind == TypedConstantKind.Type
                ? (INamedTypeSymbol)item.Value!
                : null!;
        }
        else if (Arity == 1)
        {
            TType = (attr.AttributeClass.TypeArguments[0] as INamedTypeSymbol)!;
        }
        else if (Arity == 2)
        {
            KType = (attr.AttributeClass.TypeArguments[0] as INamedTypeSymbol)!;
            TType = (attr.AttributeClass.TypeArguments[1] as INamedTypeSymbol)!;
        }
        else return context.InvalidAttribute(Symbol);

        // Capturing the template to use...
        BName = attr.AttributeClass.Name;

        if (BName == INameAttr)
            Template = Arity == 1
                ? typeof(IFrozenListTemplate<>)
                : typeof(IFrozenListTemplate<,>);

        if (BName == CNameAttr)
            Template = Arity == 1
                ? typeof(FrozenListTemplate<>)
                : typeof(FrozenListTemplate<,>);

        if (Template == null) return context.InvalidAttribute(Symbol);

        // Validating copy constructor, if needed...
        if (!Symbol.IsInterface())
        {
            var method = GetCloneMethod(Symbol);
            if (method == null)
            {
                var cons = Symbol.GetCopyConstructor();
                if (cons == null)
                {
                    context.ReportDiagnostic(TreeDiagnostics.NoCopyConstructor(Symbol));
                    return false;
                }
            }
        }

        // Finishing...
        return base.Validate(context);
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        KTypeName = KType?.EasyName(EasyNameOptions.Full) ?? string.Empty;
        TTypeName = TType?.EasyName(EasyNameOptions.Full) ?? string.Empty;

        var tname = BName.RemoveEnd("Attribute");
        var bracket = Arity == 1 ? $"<{TTypeName}>" : $"<{KTypeName}, {TTypeName}>";
        var name = $"{FrozenNamespace}.{tname}{bracket}";

        var head = base.GetHeader(context) + $" : {name}";
        return head;
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        SymbolName = Symbol.EasyName();

        // Emitting 'Clone' if needed...
        if (GetCloneMethod(Symbol) == null)
        {
            cb.AppendLine("/// <inheritdoc cref=\"ICloneable.Clone\"/>");
            cb.AppendLine(GeneratedAttribute);
            cb.AppendLine(Symbol.IsInterface()
                ? $"new {SymbolName} Clone();"
                : $"public override {SymbolName} Clone() => new {SymbolName}(this);");
        }

        // Using the methods defined by the template...
        var methods = Template
            .GetMembers()
            .OfType<MethodInfo>()
            .Where(x => x.DeclaringType == Template);

        var methodOptions = EasyNameOptions.Default with
        {
            UseMemberArgumentsTypes = EasyNameOptions.Full,
            UseMemberArgumentsNames = true,
        };

        foreach (var method in methods)
        {
            var name = method.EasyName(methodOptions);
            name = name.Replace("<K,", $"<{KTypeName},");
            name = name.Replace("T>", $"{TTypeName}>");

        }
    }

    // -----------------------------------------------------

    string GeneratedVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string GeneratedAttribute => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(FrozenGenerator)}}", "{{GeneratedVersion}}")]
        """;

    // -----------------------------------------------------

    /// <summary>
    /// Gets the declared clone method at the given type, or null if any.
    /// </summary>
    static IMethodSymbol? GetCloneMethod(INamedTypeSymbol type)
    {
        return type
            .GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0);
    }

    /// <summary>
    /// Gets the declared clone method starting at the given type and through its inheritance chains,
    /// or null if any.
    /// </summary>
    static IMethodSymbol? GetCloneMethod(INamedTypeSymbol type, out INamedTypeSymbol? core)
    {
        var method = GetCloneMethod(type);
        if (method != null)
        {
            core = type;
            return method;
        }

        var tbase = type.BaseType;
        while (tbase != null)
        {
            method = GetCloneMethod(tbase);
            if (method != null)
            {
                core = tbase;
                return method;
            }
        }

        foreach (var iface in type.AllInterfaces)
        {
            method = GetCloneMethod(iface);
            if (method != null)
            {
                core = iface;
                return method;
            }
        }

        core= null;
        return null;
    }
}