#pragma warning disable IDE0075

namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal partial class XTypeNode : TypeNode, IXNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }
    AttributeData Attribute = default!;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        if (Symbol.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }

        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
        else if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }
        else Attribute = Attributes[0];

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnEmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicitly implemented...
        if (this.FindMethod(Symbol, [], out _)) return true;

        // Dispatching...
        if (Symbol.IsInterface) return EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) return EmitHostAbstract(context, cb);
        else return EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    [SuppressMessage("", "IDE0060")]
    bool EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var rtype = Symbol;
        var rnull = false;
        if (Attribute.HasReturnType(out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = rtype.ReturnOptions(Symbol);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{stype}{snull} Clone();");

        return true;

        /// <summary>
        /// Obtains the appropriate method modifiers, followed by a space separator, or null,
        /// </summary>
        string? GetModifiers()
        {
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

                        value = dec == Accessibility.Public ? "new " : $"{str} new ";
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
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    bool EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
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
        return true;

        /// <summary>
        /// Obtains the appropriate method modifiers, followed by a space separator, or null,
        /// </summary>
        string? GetModifiers()
        {
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
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    bool EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Symbol.FindCopyConstructor(strict: false);
        if (ctor == null) { Symbol.ReportError(TreeError.NoCopyConstructor, context); return false; }

        var rtype = Symbol;
        var rnull = false;
        if (Attribute.HasReturnType(out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = rtype.ReturnOptions(Symbol);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{stype}{snull} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var host = Symbol.EasyName();
            cb.AppendLine($"var host = new {host}(this);");
            cb.AppendLine($"return host;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitExplicitInterfaces(context, cb);
        return true;

        /// <summary>
        /// Obtains the appropriate method modifiers, followed by a space separator, or null,
        /// </summary>
        string? GetModifiers()
        {
            var hasv = Attribute.HasUseVirtual(out var xvirt);
            var hvirt = hasv ? xvirt : true;
            var hsealed = Symbol.IsSealed;

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

                        if (type.IsInterface)
                        {
                            value = hsealed || !hvirt ? $"{str} " : $"{str} virtual ";
                            return true;
                        }
                        else
                        {
                            var mvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                            value = mvirt
                                ? (!hvirt ? $"{str} new " : $"{str} override ")
                                : (!hvirt ? $"{str} new " : $"{str} new virtual ");

                            return true;
                        }
                    }

                    // Requested...
                    while (type.HasCloneableAttribute(out var at))
                    {
                        if (type.IsInterface)
                        {
                            value = hsealed || !hvirt ? $"public " : $"public virtual ";
                            return true;
                        }
                        else
                        {
                            // If appear in a base method, let's defer to it...
                            if (this.FindMethod(null, [type.AllBaseTypes], out _)) break;

                            var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                            value = mvirt
                                ? (!hvirt ? $"public new " : $"public override ")
                                : (!hvirt ? $"public new " : $"public new virtual ");

                            return true;
                        }
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
        var items = GetExplicitInterfaces();
        foreach (var item in items)
        {
            var iface = item.IFace.EasyName(EasyTypeSymbol.Full with { NullableStyle = IsNullableStyle.None });
            var core = item.IFace.Name == "ICloneable";
            var rtype = core ? "object" : item.RType.EasyName(EasyTypeSymbol.Full);
            if (item.RNullable && !rtype.EndsWith('?')) rtype += '?';

            cb.AppendLine();
            cb.AppendLine($"{rtype}");
            cb.AppendLine($"{iface}.Clone()");
            cb.AppendLine($"=> ({rtype})Clone();");
        }
    }

    // ----------------------------------------------------

    record Explicit(INamedTypeSymbol IFace, INamedTypeSymbol RType, bool RNullable);

    /// <summary>
    /// Obtains the list of interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<Explicit> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        List<Explicit> list = [];
        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        return list;

        // Tries to capture the given interface...
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x.IFace, iface));
            if (temp != null) return;

            // Special case: ICloneable..
            if (iface.Name == "ICloneable")
            {
                var item = new Explicit(iface, null!, false); // 'null' will be intercepted later
                list.Add(item);
                return;
            }

            // Method existing...
            if (this.FindMethod(iface, [], out var method))
            {
                var rtype = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out var rnull);
                var item = new Explicit(iface, rtype, rnull);
                list.Add(item);
                return;
            }

            // Requested...
            if (iface.HasCloneableAttribute(out var at))
            {
                var rtype = at.HasReturnType(out var xtype, out var rnull) ? xtype : iface;
                var item = new Explicit(iface, rtype, rnull);
                list.Add(item);
                return;
            }
        }
    }
}