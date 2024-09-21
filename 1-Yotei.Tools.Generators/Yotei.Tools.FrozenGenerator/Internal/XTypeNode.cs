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

    readonly string IFrozenListName = INameAttr.RemoveEnd("Attribute");
    readonly string FrozenListName = CNameAttr.RemoveEnd("Attribute");

    string AttributeClassName = null!;
    string KTypeName = null!;
    string TTypeName = null!;
    string SymbolName = null!;
    string BaseName = null!;

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
        AttributeClassName = attr.AttributeClass.Name;

        if (AttributeClassName == INameAttr)
            Template = Arity == 1
                ? typeof(IFrozenListTemplate<>)
                : typeof(IFrozenListTemplate<,>);

        if (AttributeClassName == CNameAttr)
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

        var atrname = AttributeClassName.RemoveEnd("Attribute");
        var bracket = Arity == 1 ? $"<{TTypeName}>" : $"<{KTypeName}, {TTypeName}>";

        BaseName = $"{FrozenNamespace}.{atrname}{bracket}";
        var head = base.GetHeader(context) + $" : {BaseName}";
        return head;
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        SymbolName = Symbol.EasyName();
        var iface = FindFrozenInterface();
        var iname = iface?.EasyName(EasyNameOptions.Full) ?? "";

        // Emitting 'Clone' if needed...
        if (GetCloneMethod(Symbol) == null)
        {
            cb.AppendLine("/// <inheritdoc cref=\"ICloneable.Clone\"/>");
            cb.AppendLine(GeneratedAttribute);
            cb.AppendLine(Symbol.IsInterface()
                ? $"new {SymbolName} Clone();"
                : $"public override {SymbolName} Clone() => new {SymbolName}(this);");

            if (!Symbol.IsInterface() && iface != null)
            {
                cb.AppendLine();
                cb.AppendLine($"{iname}");
                cb.AppendLine($"{iname}.Clone() => Clone();");
            }
        }

        // Using the methods defined by the template...
        var methods = Template
            .GetMembers()
            .OfType<MethodInfo>()
            .Where(x => x.DeclaringType == Template);

        var ixoptions = EasyNameOptions.Default with // Used for iface method names...
        {
            UseMemberArgumentsTypes = EasyNameOptions.Full,
            UseMemberArgumentsNames = true,
        };
        var cxoptions = ixoptions with // Used for base method invocations...
        {
            UseMemberArgumentsTypes = null
        };
        var head = Template.EasyName().Replace("Template", "");
        if (!head.StartsWith("I")) head = 'I' + head;

        foreach (var method in methods)
        {
            var name = method.EasyName(ixoptions);
            name = name.Replace("K ", $"{KTypeName} ");     // K key...
            name = name.Replace("<K", $"<{KTypeName}");     // IComparer<K> comparer...
            name = name.Replace("T ", $"{TTypeName} ");     // T item...
            name = name.Replace("T>", $"{TTypeName}>");     // IEnumerable<T> range...            

            if (Symbol.IsInterface())
            {
                var core = method.EasyName(EasyNameOptions.Default);
                core = $"{head}.{core}";
                core = core.Replace('<', '{').Replace('>', '}');
                core = $"/// <inheritdoc cref=\"{core}\"/>";

                cb.AppendLine();
                cb.AppendLine(core);
                cb.AppendLine(GeneratedAttribute);

                cb.AppendLine($"new {SymbolName} {name};");
            }

            else
            {
                var core = method.EasyName(EasyNameOptions.Default);
                var temp = iface?.EasyName() ?? head;
                core = $"{temp}.{core}";
                core = core.Replace('<', '{').Replace('>', '}');
                core = $"/// <inheritdoc cref=\"{core}\"/>";
                cb.AppendLine();
                cb.AppendLine(core);
                cb.AppendLine(GeneratedAttribute);

                var args = method.EasyName(cxoptions);                
                cb.AppendLine($"public override {SymbolName} {name}");
                cb.AppendLine($"=> ({SymbolName})base.{args};");

                if (iface != null)
                {
                    cb.AppendLine();
                    cb.AppendLine($"{iname}");
                    cb.AppendLine($"{iname}.{name}");
                    cb.AppendLine($"=> {args};");
                }
            }
        }
    }

    // -----------------------------------------------------

    /// <summary>
    /// Finds the 'IFrozenList' interface to reimplement.
    /// </summary>
    INamedTypeSymbol? FindFrozenInterface()
    {
        if (!Symbol.IsInterface()) // This method only makes sense for classes, not interfaces.
        {
            INamedTypeSymbol? item = null;
            foreach (var iface in Symbol.Interfaces)
            {
                if (IsFrozen(iface, out var temp)) return iface;
                if (temp != null && item == null) item = temp;
            }
            if (item != null) return item;
        }
        return null;

        bool IsFrozen(INamedTypeSymbol iface, out INamedTypeSymbol? type)
        {
            // Might be decorated with any 'Frozen' attribute...
            var ats = iface.GetAttributes();
            foreach (var at in ats)
            {
                if (at.AttributeClass != null && at.AttributeClass.Name.Contains(CNameAttr))
                {
                    type = iface;
                    return true;
                }
            }

            // Or might implement 'IFrozenList'...
            if (iface.Name.StartsWith(IFrozenListName))
            {
                type = iface;
                return true;
            }

            // Childs...
            foreach (var child in iface.Interfaces)
                if (IsFrozen(child, out type)) return true;

            // Not found...
            type = null;
            return false;
        }
    }

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

    // -----------------------------------------------------

    string GeneratedVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string GeneratedAttribute => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(FrozenGenerator)}}", "{{GeneratedVersion}}")]
        """;
}