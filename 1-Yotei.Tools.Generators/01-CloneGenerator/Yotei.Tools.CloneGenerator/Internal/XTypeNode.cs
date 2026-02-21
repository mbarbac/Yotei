namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal partial class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (Symbol.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); return false; }
        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); return false; }
        if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); return false; }

        return base.Validate(context);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicit implementation...
        if (XNode.FindMethod(out _, Symbol)) return;

        // Dispatching...
        if (Symbol.IsInterface) EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var type = XNode.FindReturnType(
            out var xtype, out var xnullable, out _, Symbol, Symbol.AllInterfaces)
            ? xtype : Symbol;

        var roptions = XNode.ReturnOptions(Symbol, type);
        var rtype = type.EasyName(roptions);
        var rnull = xnullable ? "?" : string.Empty;
        var modifiers = GetInterfaceModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// <br/> If not null, ends with a space separator.
    /// </summary>
    string? GetInterfaceModifiers()
    {
        throw null;
    }

    /*
     string? GetInterfaceModifiers()
    {
        var host = ParentNode!.Symbol;

        var found = Finder.Find((type, out value) =>
        {
            // Member in the base type...
            if (XNode.FindDecoratedMember<IPropertySymbol>(Symbol.Name, out _, out var at, type))
            {
                value = "new ";
                return true;
            }

            // Method in the base type...
            if (XNode.FindMethod(MethodName, Symbol.Type, out _, type))
            {
                value = "new ";
                return true;
            }

            // Try next...
            value = default;
            return false;
        },
        out string? value, null, host.AllInterfaces);

        return found ? value : null;
    }
     */

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var type = XNode.FindReturnType(
            out var xtype, out var xnullable, out _, Symbol, Symbol.AllInterfaces)
            ? xtype : Symbol;

        var roptions = XNode.ReturnOptions(Symbol, type);
        var rtype = type.EasyName(roptions);
        var rnull = xnullable ? "?" : string.Empty;
        var modifiers = GetInterfaceModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");

        EmitExplicitInterfaces(context, cb);
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// <br/> If not null, ends with a space separator.
    /// </summary>
    string? GetAbstractModifiers()
    {
        throw null;
    }

    /*
     [SuppressMessage("", "IDE0075")]
    string? GetAbstractModifiers()
    {
        var host = ParentNode!.Symbol;

        var found = Finder.Find((type, out value) =>
        {
            // Method existing...
            while (XNode.FindMethod(MethodName, Symbol.Type, out var method, type))
            {
                var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                var str = dec.ToAccessibilityString(); if (str == null) break;

                if (type.IsInterface) { value = $"{str} abstract "; return true; }

                var mvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                value = !mvirt ? $"{str} abstract new " : $"{str} abstract override ";
                return true;
            }

            // Member existing...
            if (XNode.FindDecoratedMember<IPropertySymbol>(Symbol.Name, out var member, out var at, type))
            {
                if (type.IsInterface) { value = $"public abstract "; return true; }

                var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                value = mvirt ? $"public abstract override " : $"public abstract new ";
                return true;
            }

            // Method requested...
            if (type.HasInheritsWithAttribute(out at))
            {
                var mvirt = XNode.FindUseVirtual<IPropertySymbol>(
                    Symbol.Name, out var temp, out _, out _, type, type.AllBaseTypes, type.AllInterfaces)
                    ? temp : true;

                value = mvirt ? $"public abstract override " : $"public abstract new ";
                return true;
            }

            // Try next...
            value = null;
            return false;
        },
        out string? value, null, host.AllBaseTypes, host.AllInterfaces);

        // Returning default one if needed...
        return found ? value : "public abstract ";
    }
     */

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Symbol.FindCopyConstructor(strict: false);
        if (ctor == null) { Symbol.ReportError(TreeError.NoCopyConstructor, context); return; }

        var type = XNode.FindReturnType(
            out var xtype, out var xnullable, out _, Symbol, Symbol.AllInterfaces)
            ? xtype : Symbol;

        var roptions = XNode.ReturnOptions(Symbol, type);
        var rtype = type.EasyName(roptions);
        var rnull = xnullable ? "?" : string.Empty;
        var modifiers = GetInterfaceModifiers();
        var hostname = Symbol.EasyName();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_host = new {hostname}(this);");
            cb.AppendLine("return v_host;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitExplicitInterfaces(context, cb);
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// <br/> If not null, ends with a space separator.
    /// </summary>
    string? GetRegularModifiers()
    {
        throw null;
    }

    /*
     [SuppressMessage("", "IDE0075")]
    string? GetRegularModifiers()
    {
        var host = ParentNode!.Symbol;
        var hsealed = host.IsSealed || Symbol.IsSealed;
        var hvirt = XNode.FindUseVirtual<IPropertySymbol>(
            Symbol.Name,
            out var temp, out _, out _, host, host.AllBaseTypes, host.AllInterfaces)
            ? temp : true;

        var found = Finder.Find((type, out value) =>
        {
            // Method existing...
            while (XNode.FindMethod(MethodName, Symbol.Type, out var method, type))
            {
                var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                var str = dec.ToAccessibilityString(); if (str == null) break;

                if (type.IsInterface)
                {
                    value = hsealed || !hvirt ? $"{str} " : $"{str} virtual ";
                    return true;
                }

                var mvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                value = hsealed || !mvirt ? $"{str} new " : $"{str} override ";
                return true;
            }

            // Member existing...
            if (XNode.FindDecoratedMember<IPropertySymbol>(Symbol.Name, out var member, out var at, type))
            {
                if (type.IsInterface)
                {
                    value = hsealed || !hvirt ? $"public " : $"public virtual ";
                    return true;
                }

                var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                value = mvirt ? $"public override " : $"public new ";
                return true;
            }

            // Method requested...
            if (type.HasInheritsWithAttribute(out at))
            {
                var mvirt = XNode.FindUseVirtual<IPropertySymbol>(
                    Symbol.Name, out var temp, out _, out _, type, type.AllBaseTypes, type.AllInterfaces)
                    ? temp : true;

                value = hsealed || !mvirt ? $"public new " : $"public override ";
                return true;
            }

            // Try next...
            value = null;
            return false;
        },
        out string? value, null, host.AllBaseTypes, host.AllInterfaces);

        // Returning default one if needed...
        return found
            ? value
            : (hsealed || !hvirt) ? "public " : "public virtual ";
    }
     */

    // ----------------------------------------------------

    /*
     record ExplicitInfo(
        INamedTypeSymbol RType, bool RNullable,
        INamedTypeSymbol IFace,
        INamedTypeSymbol MType);
     */

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation, if any.
    /// </summary>
    void EmitExplicitInterfaces(SourceProductionContext context, CodeBuilder cb)
    {
    }

    /*
     [SuppressMessage("", "IDE0060")]
    void EmitExplicitInterfaces(SourceProductionContext context, CodeBuilder cb)
    {
        var items = GetExplicitInterfaces();
        foreach (var item in items)
        {
            var rtype = item.RType.EasyName(EasyTypeSymbol.Full);
            if (item.RNullable && !rtype.EndsWith('?')) rtype += '?';
            var iface = item.IFace.EasyName(EasyTypeSymbol.Full with { NullableStyle = IsNullableStyle.None });
            var mtype = item.MType.EasyName(EasyTypeSymbol.Full);

            cb.AppendLine();
            cb.AppendLine($"{rtype}");
            cb.AppendLine($"{iface}.{MethodName}({mtype} value)");
            cb.AppendLine($"=> {MethodName}(value);");
        }
    }
     */

    /*
     /// <summary>
    /// Obtains the list of interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<ExplicitInfo> GetExplicitInterfaces()
    {
        var host = ParentNode!.Symbol;
        var comp = SymbolEqualityComparer.Default;
        List<ExplicitInfo> list = [];

        foreach (var iface in host.Interfaces) TryCapture(iface);
        return list;

        // Tries to capture the given interface...
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comp.Equals(x.IFace, iface));
            if (temp != null) return;

            // Method already defined in the interface...
            if (XNode.FindMethod(MethodName, Symbol.Type, out var method, iface))
            {
                var rtype = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out var rnull);
                var mtype = method.Parameters[0].Type;
                var item = new ExplicitInfo(rtype, rnull, iface, (INamedTypeSymbol)mtype);
                list.Add(item);
                return;
            }

            // Member in the interface...
            if (XNode.FindDecoratedMember<IPropertySymbol>(Symbol.Name, out var member, out var at, iface))
            {
                var found = at.HasReturnType(out var rtype, out var rnull);
                if (!found) { rtype = iface; rnull = false; }

                var item = new ExplicitInfo(rtype!, rnull, iface, (INamedTypeSymbol)member.Type);
                list.Add(item);
                return;
            }

            // Iface requesting implementation...
            if (iface.HasInheritsWithAttribute(out at))
            {
                var found = XNode.FindReturnType<IPropertySymbol>(
                    Symbol.Name, out var rtype, out var rnull, out _, iface, iface.AllInterfaces);
                if (!found) { rtype = iface; rnull = false; }

                found = XNode.FindDecoratedMember(Symbol.Name, out member, out _, null, iface.AllInterfaces);
                var mtype = found ? member!.Type : Symbol.Type;

                var item = new ExplicitInfo(rtype!, rnull, iface, (INamedTypeSymbol)mtype);
                list.Add(item);
                return;
            }
        }
    }
     */
}