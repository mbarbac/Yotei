using System.Text.RegularExpressions;

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XPropertyNode(IPropertySymbol symbol) : base(symbol) { }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);
        var host = ParentNode!.Symbol;

        if (host.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }
        if (Symbol.IsIndexer) { Symbol.ReportError(TreeError.IndexerNotSupported, context); r = false; }
        if (!Symbol.HasGetter) { Symbol.ReportError(TreeError.NoGetter, context); r = false; }
        if (!Symbol.HasSetter && !host.IsInterface) { Symbol.ReportError(TreeError.NoSetter, context); r = false; }
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var host = ParentNode!.Symbol;

        // Intercepting explicitly implemented...
        if (FinderMethod(out _, host)) return;

        // Capturing working data...
        if (!CaptureWorkingData(context)) return;

        // Dispatching...
        if (host.IsInterface) EmitHostInterface(context, cb);
        else if (host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    /// <summary>
    /// Captures working data at emit time. Returns true if generation can continue, or false
    /// otherwise.
    /// </summary>
    bool CaptureWorkingData(SourceProductionContext context)
    {
        var host = ParentNode!.Symbol;

        MethodName = $"With{Symbol.Name}";
        ArgumentName = $"v_{Symbol.Name}";
        ReturnType = host;
        ReturnNullable = false;
        ReturnOptions = EasyTypeSymbol.Default;

        // Finding the source attribute...
        AttributeData at;

        if (!IsInherited) // Captured per-se...
        {
            if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); return false; }
            if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); return false; }
            at = Attributes[0];
        }
        else // Captured by the host inheriting members...
        {
            var ats = host.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]).ToList();
            if (ats.Count == 0) { host.ReportError(TreeError.NoAttributes, context); return false; }
            if (ats.Count > 1) { host.ReportError(TreeError.TooManyAttributes, context); return false; }
            at = ats[0];
        }

        // Return type...
        if (XNode.FindReturnTypeAt(at, out var type, out var nullable))
        {
            ReturnType = type;
            ReturnNullable = nullable;

            var same = SymbolEqualityComparer.Default.Equals(host, type);
            if (!same) ReturnOptions = EasyTypeSymbol.Full with
            { NullableStyle = IsNullableStyle.None };
        }

        // Finishing...
        return true;
    }

    string MethodName = default!;
    string ArgumentName = default!;
    INamedTypeSymbol ReturnType = default!;
    bool ReturnNullable = default;
    EasyTypeSymbol ReturnOptions = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);
        var modifiers = GetInterfaceModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({argtype} {ArgumentName});");
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// </summary>
    string? GetInterfaceModifiers()
    {
        var host = ParentNode!.Symbol;
        var found = Finder.Find((type, out value) =>
        {
            // Easy case:
            // Because the host is an interface, it can only derive from another interface. So,
            // when a base method exist, or when it shall exist because the member was decorated,
            // then we just need to use 'new'.

            if (FinderMethod(out _, type) || XNode.FinderWithAttribute(out _, out _, Symbol.Name, type))
            {
                value = "new ";
                return true;
            }
            value = default;
            return false;
        },
        out string? value, null, host.AllInterfaces);
        return found ? value : null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);
        var modifiers = GetAbstractModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({argtype} {ArgumentName});");

        EmitExplicitInterfaces(context, cb);
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// </summary>
    [SuppressMessage("", "IDE0018")]
    string? GetAbstractModifiers()
    {
        var host = ParentNode!.Symbol;
        var found = false;
        string? value = null;

        // By base method:
        // If such exist, it dictates the modifiers of the one to be generated for this host.
        found = Finder.Find((type, out value) =>
        {
            while (FinderMethod(out var method, type))
            {
                var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                var str = dec.ToAccessibilityString(); if (str == null) break;

                if (type.IsInterface) { value = $"{str} abstract "; return true; }

                var methodvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                value = methodvirt ? $"{str} abstract override " : $"{str} abstract new ";
                return true;
            }
            // Try next...
            value = null;
            return false;
        },
        out value, null, host.AllBaseTypes, host.AllInterfaces);

        // By requested member:
        // We shall assume a method is generated (maybe not yet), so it dictates the modifiers of
        // the one to be generated for this host.
        if (!found) found = Finder.Find((type, out value) =>
        {
            while (FinderDecoratedMember(out var member, type))
            {
                if (type.IsInterface) { value = $"public abstract "; return true; }

                if (!XNode.FinderUseVirtual(
                    out var membervirt,
                    Symbol.Name, type, type.AllBaseTypes, type.AllInterfaces)) membervirt = true;

                value = membervirt ? $"public abstract override " : $"public abstract new ";
                return true;
            }
            // Try next...
            value = null;
            return false;
        },
        out value, null, host.AllBaseTypes, host.AllInterfaces);

        // Finishing...
        return found ? value : "public abstract ";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var host = ParentNode!.Symbol;
        var ctor = host.FindCopyConstructor(strict: false);
        if (ctor == null) { host.ReportError(TreeError.NoCopyConstructor, context); return; }

        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);
        var modifiers = GetRegularModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({argtype} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var hostname = host.EasyName();
            cb.AppendLine($"var v_host = new {hostname}(this)");
            cb.AppendLine("{");
            cb.IndentLevel++;
            {
                cb.AppendLine($"{Symbol.Name} = {ArgumentName}");
            }
            cb.IndentLevel--;
            cb.AppendLine("};");
            cb.AppendLine("return v_host;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitExplicitInterfaces(context, cb);
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// </summary>
    [SuppressMessage("", "IDE0018")]
    string? GetRegularModifiers()
    {
        var host = ParentNode!.Symbol;
        var found = false;
        string? value = null;
        var issealed = Symbol.IsSealed || host.IsSealed;

        if (!XNode.FinderUseVirtual(
            out var hostvirt,
            Symbol.Name, host, host.AllBaseTypes, host.AllInterfaces)) hostvirt = true;

        // By base method:
        // If such exist, it dictates the modifiers of the one to be generated for this host.
        found = Finder.Find((type, out value) =>
        {
            while (FinderMethod(out var method, type))
            {
                var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                var str = dec.ToAccessibilityString(); if (str == null) break;

                if (type.IsInterface)
                {
                    value = !hostvirt || issealed ? $"{str} " : $"{str} virtual ";
                    return true;
                }

                var methodvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                value = !methodvirt || issealed ? $"{str} new " : $"{str} override ";
                return true;
            }
            // Try next...
            value = null;
            return false;
        },
        out value, null, host.AllBaseTypes, host.AllInterfaces);

        // By requested member:
        // We shall assume a method is generated (maybe not yet), so it dictates the modifiers of
        // the one to be generated for this host.
        if (!found) found = Finder.Find((type, out value) =>
        {
            while (FinderDecoratedMember(out var member, type))
            {
                if (type.IsInterface)
                {
                    value = !hostvirt || issealed ? $"public " : $"public virtual ";
                    return true;
                }

                if (!XNode.FinderUseVirtual(
                    out var membervirt,
                    Symbol.Name, type, type.AllBaseTypes, type.AllInterfaces)) membervirt = true;

                value = membervirt ? $"public override " : $"public new ";
                return true;
            }
            // Try next...
            value = null;
            return false;
        },
        out value, null, host.AllBaseTypes, host.AllInterfaces);

        // Finishing...
        return found ? value : (!hostvirt || issealed ? "public " :"public virtual ");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation, if any.
    /// </summary>
    void EmitExplicitInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        var ifaces = GetExplicitInterfaces();
        foreach (var item in ifaces)
        {
            var iface = item.IFace;
            var mtype = item.MemberType;

            var ifacename = iface.EasyName(EasyTypeSymbol.Full with { NullableStyle = IsNullableStyle.None });
            var nullable = ReturnNullable ? "?" : string.Empty;
            var mtypename = mtype.EasyName(EasyTypeSymbol.Full);

            cb.AppendLine();
            cb.AppendLine($"{ifacename}{nullable}");
            cb.AppendLine($"{ifacename}.{MethodName}({mtypename} value)");
            cb.AppendLine($"=> {MethodName}(value);");
            cb.AppendLine($"");
        }
    }

    /// <summary>
    /// Represents a explicit interface along with the type of the member at its level.
    /// </summary>
    record Explicit(INamedTypeSymbol IFace, INamedTypeSymbol MemberType);

    /// <summary>
    /// Obtains a list with the interfaces that need explicit implementation, along with the
    /// actual type of the member.
    /// </summary>
    List<Explicit> GetExplicitInterfaces()
    {
        var host = ParentNode!.Symbol;
        var comparer = SymbolEqualityComparer.Default;
        List<Explicit> list = [];

        foreach (var iface in host.Interfaces) TryCaptureAt(iface);
        return list;

        bool TryCaptureAt(INamedTypeSymbol iface)
        {
            var found = false;
            INamedTypeSymbol? membertype = null;

            foreach (var child in iface.Interfaces)
                if (TryCaptureAt(child)) { membertype = child; found = true; }

            if (!found)
                if (FinderDecoratedMember(out var temp, iface))
                { membertype = (INamedTypeSymbol)temp.Type; found = true; }

            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x.IFace, iface));
                if (temp == null) list.Add(new(iface, membertype!));
            }

            return found;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a compatible decorated member at the given type, if not null, and at the types
    /// of the given arrays.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    bool FinderDecoratedMember(
        [NotNullWhen(true)] out IPropertySymbol? value,
        INamedTypeSymbol? type, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            var member = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == Symbol.Name);

            if (member != null)
            {
                var at = member.GetAttributes([typeof(WithAttribute), typeof(WithAttribute<>)]);
                value = at == null ? null : member;
                return at != null;
            }

            value = null;
            return false;
        },
        out value, type, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a suitable method at the given type, if not null, and at the types of the
    /// given arrays.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    bool FinderMethod(
        [NotNullWhen(true)] out IMethodSymbol? value,
        INamedTypeSymbol? type, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == MethodName &&
                x.Parameters.Length == 1 &&
                Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

            return value != null;
        },
        out value, type, chains);
    }
}