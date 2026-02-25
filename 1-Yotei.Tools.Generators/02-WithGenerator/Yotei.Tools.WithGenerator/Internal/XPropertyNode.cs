#pragma warning disable IDE0075

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode, IXNode<IPropertySymbol>
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
    INamedTypeSymbol Host => ParentNode!.Symbol;
    AttributeData Attribute = default!;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);

        if (Host.IsRecord) { Host.ReportError(TreeError.RecordsNotSupported, context); r = false; }
        if(Symbol.IsIndexer) { Symbol.ReportError(TreeError.IndexerNotSupported, context); r = false; }
        if (!Symbol.HasGetter) { Symbol.ReportError(TreeError.NoGetter, context); r = false; }
        if (!Symbol.HasSetter && !Host.IsInterface) { Symbol.ReportError(TreeError.NoSetter, context); r = false; }

        if (IsInherited)
        {
            if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
            if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }
            Attribute = Attributes[0];
        }
        else
        {
            var ats = Host.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]).ToList();
            if (Attributes.Count == 0) { Host.ReportError(TreeError.NoAttributes, context); r = false; }
            if (ats.Count > 1) { Host.ReportError(TreeError.TooManyAttributes, context); r = false; }
            Attribute = ats[0];
        }

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
        // Intercepting explicitly implemented...
        if (this.FindMethod(Host, [], out _)) return;

        // Dispatching...
        if (Host.IsInterface) EmitHostInterface(context, cb);
        else if (Host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        /*
        var rtype = Symbol;
        var rnull = false;
        if (Attribute.HasReturnType(out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = rtype.ReturnOptions(Symbol);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{stype}{snull} Clone();");
        */
        throw null;

        /// <summary>
        /// Obtains the appropriate method modifiers, followed by a space separator, or null,
        /// </summary>
        string? GetModifiers()
        {
            /*
            // If appear in base types, modifiers must adapt...
            var found = Finder.Find(
                null, [Symbol.AllBaseTypes, Symbol.AllInterfaces],
                out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (this.FindMethod(type, [], out var method))
                    {
                        var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                        var str = dec.ToAccessibilityString(); if (str == null) break;

                        value = $"{str} new ";
                        return true;
                    }

                    // Requested...
                    while (type.HasCloneableAttribute(out _))
                    {
                        value = $"new ";
                        return true;
                    }

                    // Try next...
                    value = null;
                    return false;
                });

            // Finishing...
            return found ? value : null;
            */
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        /*
        var rtype = Symbol;
        var rnull = false;
        if (Attribute.HasReturnType(out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = rtype.ReturnOptions(Symbol);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{stype}{snull} Clone();");

        EmitExplicitInterfaces(context, cb);
        */
        throw null;

        /// <summary>
        /// Obtains the appropriate method modifiers, followed by a space separator, or null,
        /// Notes:
        /// - abstract new: only valid when the base class is concrete, not an abstract one
        /// </summary>
        string? GetModifiers()
        {
            /*
            // If appear in base types, modifiers must adapt...
            var found = Finder.Find(
                null, [Symbol.AllBaseTypes, Symbol.AllInterfaces],
                out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (this.FindMethod(type, [], out var method))
                    {
                        var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                        var str = dec.ToAccessibilityString(); if (str == null) break;

                        if (type.IsInterface) { value = $"{str} abstract "; return true; }
                        else if (type.IsAbstract) { value = $"{str} abstract override "; return true; }
                        else
                        {
                            var mvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                            value = !mvirt
                                ? $"{str} abstract new "
                                : $"{str} abstract override ";

                            return true;
                        }
                    }

                    // Requested...
                    while (type.HasCloneableAttribute(out var at))
                    {
                        if (type.IsInterface) { value = $"public abstract "; return true; }
                        if (type.IsAbstract) { value = $"public abstract override "; return true; }
                        else
                        {
                            // If appear in a base method, let's defer to it...
                            if (this.FindMethod(null, [type.AllBaseTypes], out _)) break;

                            var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                            value = !mvirt
                                ? $"public abstract new "
                                : $"public abstract override ";

                            return true;
                        }
                    }

                    // Try next...
                    value = null;
                    return false;
                });

            // Finishing...
            return found ? value : "public abstract ";
            */
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Host.FindCopyConstructor(strict: false);
        if (ctor == null) { Host.ReportError(TreeError.NoCopyConstructor, context); return; }

        var rtype = Host;
        var rnull = false;
        if (Attribute.HasReturnType(out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = rtype.ReturnOptions(Host);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();
        var argtype = this.SymbolType.EasyName(EasyTypeSymbol.Full);

        this.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{stype}{snull}");
        cb.AppendLine($"{this.MethodName}({argtype} value)");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var host = Symbol.EasyName();
            cb.AppendLine($"var host = new {host}(this)");
            cb.AppendLine("{");
            cb.IndentLevel++;
            {
                cb.AppendLine($"{this.SymbolName} = value");
            }
            cb.IndentLevel--;
            cb.AppendLine("};");
            cb.AppendLine($"return host;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitExplicitInterfaces(context, cb);

        /// <summary>
        /// Obtains the appropriate method modifiers, followed by a space separator, or null,
        /// </summary>
        string? GetModifiers()
        {
            var hasv = Attribute.HasUseVirtual(out var xvirt);
            var hvirt = hasv ? xvirt : true;
            var hsealed = Host.IsSealed || Symbol.IsSealed;

            // If appear in base types, modifiers must adapt...
            var found = Finder.Find(
                null, [Host.AllBaseTypes, Host.AllInterfaces],
                out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (this.FindMethod(type, [], out var method))
                    {
                        var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                        var str = dec.ToAccessibilityString(); if (str == null) break;

                        if (type.IsInterface)
                        {
                            value = hsealed || !hvirt ? $"{str} " : $"{str} virtual ";
                            return true;
                        }

                        var mvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                        value = mvirt
                            ? (!hvirt ? $"{str} new " : $"{str} override ")
                            : (!hvirt ? $"{str} new " : $"{str} new virtual ");

                        return true;
                    }

                    // Existing member...
                    while (this.FindMember(type, [], out var member, out var at))
                    {
                        if (type.IsInterface)
                        {
                            value = hsealed || !hvirt ? $"public " : $"public virtual ";
                            return true;
                        }

                        var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                        value = mvirt
                            ? (!hvirt ? $"public new " : $"public override ")
                            : (!hvirt ? $"public new " : $"public new virtual ");

                        return true;
                    }

                    // Requested...
                    while (type.HasInheritsWithAttribute(out var at))
                    {
                        if (type.IsInterface)
                        {
                            value = hsealed || !hvirt ? $"public " : $"public virtual ";
                            return true;
                        }

                        // If appear in a base method, let's defer to it...
                        if (this.FindMethod(null, [type.AllBaseTypes], out _) ||
                            this.FindMember(null, [type.AllBaseTypes], out _, out _))
                            break;

                        var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                        value = mvirt
                            ? (!hvirt ? $"public new " : $"public override ")
                            : (!hvirt ? $"public new " : $"public new virtual ");

                        return true;
                    }

                    // Try next...
                    value = null;
                    return false;
                });

            // Finishing...
            return found ? value : (hsealed || !hvirt ? "public " : "public virtual ");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation, if any.
    /// </summary>
    [SuppressMessage("", "IDE0060")]
    void EmitExplicitInterfaces(SourceProductionContext context, CodeBuilder cb)
    {
        var mname = this.MethodName;
        var items = GetExplicitInterfaces();

        foreach (var item in items)
        {
            var iface = item.IFace.EasyName(EasyTypeSymbol.Full with { NullableStyle = IsNullableStyle.None });
            var rtype = item.RType.EasyName(EasyTypeSymbol.Full);
            if (item.RNullable && !rtype.EndsWith('?')) rtype += '?';
            var atype = item.ArgType.EasyName(EasyTypeSymbol.Full);

            cb.AppendLine();
            cb.AppendLine($"{rtype}");
            cb.AppendLine($"{iface}.{mname}({atype} value)");
            cb.AppendLine($"=> ({rtype}){mname}(value);");
        }
    }

    // ----------------------------------------------------

    record Explicit(
        INamedTypeSymbol IFace,
        INamedTypeSymbol RType, bool RNullable, INamedTypeSymbol ArgType);

    /// <summary>
    /// Obtains the list of interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<Explicit> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        List<Explicit> list = [];
        foreach (var iface in Host.Interfaces) TryCapture(iface);
        return list;

        // Tries to capture the given interface...
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x.IFace, iface));
            if (temp != null) return;

            // Existing method...
            if (this.FindMethod(iface, [], out var method))
            {
                var rtype = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out var rnull);
                var atype = (INamedTypeSymbol)method.Parameters[0].Type;
                var item = new Explicit(iface, rtype, rnull, atype);
                list.Add(item);
                return;
            }

            // Existing member...
            if (this.FindMember(iface, [], out var member, out var at))
            {
                var rtype = ((INamedTypeSymbol)member.Type).UnwrapNullable(out var rnull);
                var atype = (INamedTypeSymbol)member.Type;
                var item = new Explicit(iface, rtype, rnull, atype);
                list.Add(item);
                return;
            }

            // Requested...
            if (iface.HasWithAttribute(out at))
            {
                var rtype = at.HasReturnType(out var xtype, out var rnull) ? xtype : iface;
                var item = new Explicit(iface, rtype, rnull, iface);
                list.Add(item);
                return;
            }
        }
    }
}