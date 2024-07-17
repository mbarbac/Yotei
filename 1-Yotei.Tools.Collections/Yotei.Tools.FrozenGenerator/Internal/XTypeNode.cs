namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    AttributeData Attribute = null!;
    INamedTypeSymbol AttributeClass = null!;
    Type Template = null!;
    INamedTypeSymbol KType = null!;
    INamedTypeSymbol TType = null!;

    // -----------------------------------------------------

    static bool NoAttributes(SourceProductionContext context, INamedTypeSymbol symbol)
    {
        context.ReportDiagnostic(FrozenDiagnostics.NoAttributes(symbol));
        return false;
    }

    static bool ManyAttributes(SourceProductionContext context, INamedTypeSymbol symbol)
    {
        context.ReportDiagnostic(FrozenDiagnostics.ManyAttributes(symbol));
        return false;
    }

    static bool InvalidAttribute(SourceProductionContext context, INamedTypeSymbol symbol)
    {
        context.ReportDiagnostic(FrozenDiagnostics.InvalidAttribute(symbol));
        return false;
    }

    static bool NoCopyConstructor(SourceProductionContext context, INamedTypeSymbol symbol)
    {
        context.ReportDiagnostic(TreeDiagnostics.NoCopyConstructor(symbol));
        return false;
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (Candidate != null)
        {
            if (Candidate.Attributes.Length > 1) return ManyAttributes(context, Symbol);
            Attribute = Candidate.Attributes[0];
        }
        else
        {
            var attrs = Symbol.GetAttributes(typeof(IFrozenListAttribute<>));
            attrs.AddRange(Symbol.GetAttributes(typeof(IFrozenListAttribute<,>)));
            attrs.AddRange(Symbol.GetAttributes(typeof(FrozenListAttribute<>)));
            attrs.AddRange(Symbol.GetAttributes(typeof(FrozenListAttribute<,>)));

            if (attrs.Count > 1) return ManyAttributes(context, Symbol);
            Attribute = attrs[0];
        }
        if (Attribute == null) return NoAttributes(context, Symbol);

        AttributeClass = Attribute.AttributeClass!;
        if (AttributeClass == null) return InvalidAttribute(context, Symbol);

        KType = GetKType()!;
        TType = GetTType()!;
        if (KType is null && TType is null) return InvalidAttribute(context, Symbol);

        Template = AttributeClass switch
        {
            { Name: nameof(IFrozenListAttribute<int>), Arity: 1 } => typeof(Templates.IFrozenList<>),
            { Name: nameof(IFrozenListAttribute<int>), Arity: 2 } => typeof(Templates.IFrozenList<,>),
            { Name: nameof(FrozenListAttribute<int>), Arity: 1 } => typeof(Templates.FrozenList<>),
            { Name: nameof(FrozenListAttribute<int>), Arity: 2 } => typeof(Templates.FrozenList<,>),
            _ => null!
        };
        if (Template is null) return InvalidAttribute(context, Symbol);

        if (!Symbol.IsInterface())
        {
            var ctor = Symbol.GetCopyConstructor();
            if (ctor == null) return NoCopyConstructor(context, Symbol);
        }

        return base.Validate(context);

        // Gets the 'K' type, if any...
        INamedTypeSymbol? GetKType()
        {
            if (AttributeClass.Arity == 2) return AttributeClass.TypeArguments[0] as INamedTypeSymbol;
            return null;
        }

        // Gets the 'T' type, if any...
        INamedTypeSymbol? GetTType()
        {
            if (AttributeClass.Arity == 1) return AttributeClass.TypeArguments[0] as INamedTypeSymbol;
            if (AttributeClass.Arity == 2) return AttributeClass.TypeArguments[1] as INamedTypeSymbol;
            return null;
        }
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var kname = KType?.EasyName(EasyNameOptions.Full) ?? "";
        var tname = TType?.EasyName(EasyNameOptions.Full) ?? "";

        var arity = Template.GetGenericArguments().Length;
        var bracket = arity == 1 ? $"<{tname}>" : $"<{kname}, {tname}>";
        var name = Template.EasyName(EasyNameOptions.Empty with { UseTypeName = true });
        name = $"Yotei.Tools.{name}{bracket}";

        var head = base.GetHeader(context);
        head += $" : {name}";
        return head;
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        var iface = Symbol.Interfaces.Length > 0 ? Symbol.Interfaces[0] : null;
        var iname = iface?.EasyName(EasyNameOptions.Full);

        var hname = Symbol.EasyName(EasyNameOptions.Default);
        var kname = KType?.EasyName(EasyNameOptions.Full) ?? "";
        var tname = TType?.EasyName(EasyNameOptions.Full) ?? "";

        var arity = Template.GetGenericArguments().Length;
        var bracket = arity == 1 ? $"<{tname}>" : $"<{kname}, {tname}>";
        var pname = Template.EasyName(EasyNameOptions.Empty with { UseTypeName = true });
        pname = $"Yotei.Tools.{pname}{bracket}";

        cb.AppendLine("/// <inheritdoc cref=\"ICloneable.Clone\"/>");
        cb.AppendLine(GeneratedAttribute);
        cb.AppendLine(Symbol.IsInterface()
            ? $"new {hname} Clone();"
            : $"public override {hname} Clone() => new {hname}(this);");

        if (!Symbol.IsInterface() && iface != null)
        {
            cb.AppendLine();
            cb.AppendLine($"{iname}");
            cb.AppendLine($"{iname}.Clone() => Clone();");
        }

        var ioptions = EasyNameOptions.Default with
        {
            UseMemberArgumentsTypes = EasyNameOptions.Full,
            UseMemberArgumentsNames = true,
        };
        var toptions = ioptions with { UseMemberArgumentsTypes = null };

        var methods = Template.GetMembers()
            .OfType<MethodInfo>().Where(x => x.DeclaringType == Template);

        foreach (var method in methods)
        {
            var name = method.EasyName(ioptions);
            name = name.Replace("T ", $"{tname} ");
            name = name.Replace("T> ", $"{tname}>");
            name = name.Replace("T> ", $"{tname}>");
            name = name.Replace("K ", $"{kname} ");

            if (Symbol.IsInterface())
            {
                var head = $"{pname}.{name}".Replace('<', '{').Replace('>', '}');
                head = $"/// <inheritdoc cref=\"{head}\"/>";
                cb.AppendLine();
                cb.AppendLine(head);
                cb.AppendLine(GeneratedAttribute);

                cb.AppendLine($"new {hname} {name};");
            }
            else
            {
                var args = method.EasyName(toptions);

                cb.AppendLine();
                cb.AppendLine("/// <inheritdoc/>");
                cb.AppendLine(GeneratedAttribute);

                cb.AppendLine($"public override {hname} {name} => ({hname})base.{args}; ");

                if (iface != null)
                {
                    cb.AppendLine();
                    cb.AppendLine($"{iname}");
                    cb.AppendLine($"{iname}.{name} => {args};");
                }
            }
        }
    }

    // -----------------------------------------------------

    string GeneratedVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string GeneratedAttribute => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(FrozenGenerator)}}", "{{GeneratedVersion}}")]
        """;
}