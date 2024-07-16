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
    INamedTypeSymbol? OverrideType = null;

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

        OverrideType = GetOverrideType();

        return base.Validate(context);

        INamedTypeSymbol? GetKType()
        {
            if (AttributeClass.Arity == 2) return AttributeClass.TypeArguments[0] as INamedTypeSymbol;
            return null;
        }

        INamedTypeSymbol? GetTType()
        {
            if (AttributeClass.Arity == 1) return AttributeClass.TypeArguments[0] as INamedTypeSymbol;
            if (AttributeClass.Arity == 2) return AttributeClass.TypeArguments[1] as INamedTypeSymbol;
            return null;
        }

        INamedTypeSymbol? GetOverrideType()
        {
            if (Attribute.GetNamedArgument("OverrideType", out var item))
            {
            }
            return null;
        }
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var kname = KType?.EasyName(EasyNameOptions.Full) ?? "";
        var tname = TType?.EasyName(EasyNameOptions.Full) ?? "";

        var name = Template.EasyName(EasyNameOptions.Full);
        name = name.Replace(".FrozenGenerator.Templates", "");
        name = name.Replace("<T>", $"<{tname}>");
        name = name.Replace("<K, T>", $"<{kname}, {tname}>");

        var head = base.GetHeader(context);
        head += $" : {name}";
        return head;
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        var hname = Symbol.EasyName(EasyNameOptions.Default);
        var kname = KType?.EasyName(EasyNameOptions.Full) ?? "";
        var tname = TType?.EasyName(EasyNameOptions.Full) ?? "";

        var pname = Template.EasyName(EasyNameOptions.Full);
        pname = pname.Replace(".FrozenGenerator.Templates", "");
        pname = pname.Replace("<T>", $"<{tname}>");
        pname = pname.Replace("<K, T>", $"<{kname}, {tname}>");

        var options = EasyNameOptions.Default with
        {
            UseMemberArgumentsTypes = EasyNameOptions.Full,
            UseMemberArgumentsNames = true,
        };
        var methods = Template.GetMembers().OfType<MethodInfo>();
        var done = false;
        foreach (var method in methods)
        {
            var name = method.EasyName(options);
            name = name.Replace("T ", $"{tname} ");
            name = name.Replace("T> ", $"{tname}>");
            name = name.Replace("T> ", $"{tname}>");
            name = name.Replace("K ", $"{kname} ");

            if (done) cb.AppendLine();
            done = true;

            var head = $"{pname}.{name}".Replace('<', '{').Replace('>', '}');
            head = $"/// <inheritdoc cref=\"{head}\"/>";
            cb.AppendLine(head);
            cb.AppendLine(GeneratedAttribute);

            if (Symbol.IsInterface()) cb.AppendLine($"new {hname} {name};");
            else
            {
            }
        }
    }

    // -----------------------------------------------------

    string GeneratedVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string GeneratedAttribute => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(FrozenGenerator)}}", "{{GeneratedVersion}}")]
        """;
}