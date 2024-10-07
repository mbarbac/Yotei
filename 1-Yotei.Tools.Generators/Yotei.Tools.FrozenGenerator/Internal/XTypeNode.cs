using System.Diagnostics.Contracts;

namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    const string FrozenNamespace = "Yotei.Tools";
    const string IFrozenListName = "IFrozenList";
    const string FrozenListName = "FrozenList";
    const string IFrozenAttributeName = IFrozenListName + "Attribute";
    const string FrozenAttributeName = FrozenListName + "Attribute";

    AttributeData AttributeData = null!;
    INamedTypeSymbol AttributeClass = null!;
    int Arity = 0;
    INamedTypeSymbol KType = null!;
    INamedTypeSymbol TType = null!;
    
    Type Template = null!;
    IMethodSymbol? CloneMethod = null;
    string KTypeName = null!;
    string TTypeName = null!;
    string BaseName = null!;
    string SymbolName = null!;

    // -----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        // Capturing the unique allowed attribute...
        if (Candidate != null)
        {
            if (Candidate.Attributes.Length == 0) return context.NoAttributes(Symbol);
            if (Candidate.Attributes.Length > 1) return context.ManyAttributes(Symbol);
            AttributeData = Candidate.Attributes[0];
        }
        else
        {
            var ats = Symbol.GetAttributes().Where(x =>
                x.AttributeClass != null &&
                x.AttributeClass.Name.Contains(FrozenAttributeName))
                .ToArray();

            if (ats.Length == 0) return context.NoAttributes(Symbol);
            if (ats.Length > 1) return context.ManyAttributes(Symbol);
            AttributeData = ats[0];
        }

        // Capturing attribute class...
        if ((AttributeClass = AttributeData.AttributeClass!) == null) return context.InvalidAttribute(Symbol);

        // Capturing element's TType and KType (if any), along with the effective arity...
        Arity = AttributeClass.Arity;
        if (Arity == 0)
        {
            var args = AttributeData.ConstructorArguments;

            if (args.Length == 1)
            {
                if ((TType = GetType(args[0])) == null) return context.InvalidAttribute(Symbol);
                Arity = 1;
            }
            else if (args.Length == 2)
            {
                if ((KType = GetType(args[0])) == null) return context.InvalidAttribute(Symbol);
                if ((TType = GetType(args[1])) == null) return context.InvalidAttribute(Symbol);
                Arity = 2;
            }
            else return context.InvalidAttribute(Symbol);

            static INamedTypeSymbol GetType(TypedConstant item)
            {
                return !item.IsNull && item.Kind == TypedConstantKind.Type
                    ? (INamedTypeSymbol)item.Value!
                    : null!;
            }
        }
        else if (Arity == 1)
        {
            TType = (AttributeClass.TypeArguments[0] as INamedTypeSymbol)!;
        }
        else if (Arity == 2)
        {
            KType = (AttributeClass.TypeArguments[0] as INamedTypeSymbol)!;
            TType = (AttributeClass.TypeArguments[1] as INamedTypeSymbol)!;
        }
        else return context.InvalidAttribute(Symbol);

        // Capturing the template to use...
        var name = AttributeClass.Name;

        if (name == IFrozenAttributeName)
            Template = Arity == 1
                ? typeof(IFrozenListTemplate<>)
                : typeof(IFrozenListTemplate<,>);

        if (name == FrozenAttributeName)
            Template = Arity == 1
                ? typeof(FrozenListTemplate<>)
                : typeof(FrozenListTemplate<,>);

        if (Template == null) return context.InvalidAttribute(Symbol);

        // Validating copy constructor to emit 'Clone', if needed...
        CloneMethod = GetCloneMethod(Symbol);

        // Validating copy constructor in case we need to emit...
        if (CloneMethod == null && !Symbol.IsInterface())
        {            
            var cons = Symbol.GetCopyConstructor();
            if (cons == null)
            {
                context.ReportDiagnostic(TreeDiagnostics.NoCopyConstructor(Symbol));
                return false;
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

        var atrname = AttributeClass.Name.RemoveEnd("Attribute");
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
        var iface = Symbol.IsInterface() ? null : FindImplementable();
        var iname = iface?.EasyName(EasyNameOptions.Full);

        // Emitting 'Clone' method is needed...
        if (CloneMethod == null)
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

        // Options for iface methods' names...
        var ixoptions = EasyNameOptions.Default with
        {
            UseMemberArgumentsTypes = EasyNameOptions.Full,
            UseMemberArgumentsNames = true,
        };

        // Options for base method invocation...
        var cxoptions = ixoptions with { UseMemberArgumentsTypes = null };

        // Head variable for documenting iface...
        var head = Template.EasyName().Replace("Template", "");
        if (!head.StartsWith("I")) head = 'I' + head;

        // Used to prevent emitting already defined ones...
        var methods = Symbol.GetMembers().OfType<IMethodSymbol>().ToArray();

        // Iterating through the captured template methods, declared in the template...
        var templates = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);
        foreach (var template in templates)
        {
            if (!CanEmit(template, methods)) continue;

            var name = template.EasyName(ixoptions);
            name = name.Replace("K ", $"{KTypeName} ");     // K key...
            name = name.Replace("<K", $"<{KTypeName}");     // IComparer<K> comparer...
            name = name.Replace("T ", $"{TTypeName} ");     // T item...
            name = name.Replace("T>", $"{TTypeName}>");     // IEnumerable<T> range...

            if (Symbol.IsInterface())
            {
                var core = template.EasyName(EasyNameOptions.Default);
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
                var core = template.EasyName(EasyNameOptions.Default);
                var temp = iface?.EasyName() ?? head;
                core = $"{temp}.{core}";
                core = core.Replace('<', '{').Replace('>', '}');
                core = $"/// <inheritdoc cref=\"{core}\"/>";
                cb.AppendLine();
                cb.AppendLine(core);
                cb.AppendLine(GeneratedAttribute);

                var args = template.EasyName(cxoptions);
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

    /// <summary>
    /// Determines if the given template method can be emitted, based upon the list of declared
    /// ones.
    /// </summary>
    bool CanEmit(MethodInfo template, IMethodSymbol[] declaredmethods)
    {
        foreach (var declared in declaredmethods)
        {
            var tname = template.Name;
            var dname = declared.Name;
            if (tname != dname) continue; // Name differ...

            var tpars = template.GetParameters();
            var dpars = declared.Parameters;
            if (tpars.Length != dpars.Length) continue; // Number of parameters not the same...

            var count = tpars.Length;
            for (int i = 0; i < tpars.Length; i++)
            {
                var tpar = tpars[i];
                var dpar = dpars[i];

                // Template parameter not being a generic one...
                if (!tpar.ParameterType.IsGenericParameter)
                {
                    var ttype = tpar.ParameterType;
                    var dtype = dpar.Type;

                    if (dtype.Match(ttype)) count--;
                }

                // Template parameter being a generic one... that corresponds to a declared one that
                // may not be so. Also, we we know that KType and TType, if not null, are concrete ones
                // due to C# syntax rules.
                else
                {
                    var match = false;
                    var dtype = dpar.Type;

                    if (KType != null && SymbolComparer.Default.Equals(dtype, KType)) match = true;
                    if (TType != null && SymbolComparer.Default.Equals(dtype, TType)) match = true;

                    if (match) count--;
                }
            }

            if (count == 0) return false; // All parameters the same...
        }

        return true;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Finds the 'IFrozenList' interface to reimplement. To be invoked for non-interfaces only.
    /// </summary>
    /// <returns></returns>
    INamedTypeSymbol? FindImplementable()
    {
        INamedTypeSymbol? item = null;
        foreach (var iface in Symbol.Interfaces)
        {
            if (IsFrozenAlike(iface, out var temp)) return iface;
            if (temp != null && item == null) item = temp;
        }
        return item;

        // Determines if iface is a frozen-alike one...
        static bool IsFrozenAlike(INamedTypeSymbol iface, out INamedTypeSymbol? type)
        {
            // Might be decorated with any 'Frozen' attribute...
            var ats = iface.GetAttributes();
            foreach (var at in ats)
            {
                if (at.AttributeClass != null && at.AttributeClass.Name.Contains(FrozenAttributeName))
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
                if (IsFrozenAlike(child, out type)) return true;

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

        core = null;
        return null;
    }

    // -----------------------------------------------------

    string GeneratedVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string GeneratedAttribute => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(FrozenGenerator)}}", "{{GeneratedVersion}}")]
        """;
}