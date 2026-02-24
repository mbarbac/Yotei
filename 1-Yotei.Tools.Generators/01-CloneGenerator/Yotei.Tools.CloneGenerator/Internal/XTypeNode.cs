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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);

        if (Symbol.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }
        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
        if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicitly implemented...
        if (this.FindMethod(out _, Symbol, [])) return;

        // Dispatching...
        if (Symbol.IsInterface) EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    [SuppressMessage("", "IDE0060")]
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var type = this.FindReturnType(out var xtype, out var xnull,
            Symbol, [Symbol.AllInterfaces])
            ? xtype : Symbol;

        var options = type.ReturnOptions(Symbol);
        var rtype = type.EasyName(options);
        var rnull = xnull ? "?" : string.Empty;
        var modifiers = GetInterfaceModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");

        /// <summary>
        /// Obtains the appropriate method modifiers, or null if any. If not null, ends with a
        /// space terminator.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetInterfaceModifiers()
        {
            // Using the host base types and interfaces, not the host itself...
            var found = Finder.Find(out string? value, (type, out value) =>
            {
                // Method existing...
                if (this.FindMethod(out var method, type, []))
                {
                    value = "new ";
                    return true;
                }

                // Requested...
                if (type.HasCloneableAttribute(out _))
                {
                    value = "new ";
                    return true;
                }

                // Try next...
                value = null;
                return false;
            },
            null, [Symbol.AllInterfaces]);

            // Returning found or default value...
            return found ? value : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var type = this.FindReturnType(out var xtype, out var xnull,
            Symbol, [Symbol.AllBaseTypes, Symbol.AllInterfaces])
            ? xtype : Symbol;

        var options = type.ReturnOptions(Symbol);
        var rtype = type.EasyName(options);
        var rnull = xnull ? "?" : string.Empty;
        var modifiers = GetAbstractModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");

        EmitExplicitInterfaces(context, cb);

        /// <summary>
        /// Obtains the appropriate method modifiers, or null if any. If not null, ends with a
        /// space terminator.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetAbstractModifiers()
        {
            // Using the host base types and interfaces, not the host itself...
            var found = Finder.Find(out string? value, (type, out value) =>
            {
                // Method existing...
                while (this.FindMethod(out var method, type, []))
                {
                    var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                    var str = dec.ToAccessibilityString(); if (str == null) break;

                    if (type.IsInterface) { value = $"{str} abstract "; return true; }

                    var mvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                    value = !mvirt ? $"{str} abstract new " : $"{str} abstract override ";
                    return true;
                }

                // Requested...
                if (type.HasCloneableAttribute(out _))
                {
                    var mvirt = this.FindUseVirtual(out var temp, out _,
                        type, [type.AllBaseTypes, type.AllInterfaces])
                        ? temp : true;

                    value = type.IsInterface
                        ? "public abstract "
                        : (!mvirt ? $"public abstract new " : $"public abstract override ");

                    return true;
                }

                // Try next...
                value = null;
                return false;
            },
            null, [Symbol.AllBaseTypes, Symbol.AllInterfaces]);

            // Returning found or default value...
            return found ? value : "public abstract ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Symbol.FindCopyConstructor(strict: false);
        if (ctor == null) { Symbol.ReportError(TreeError.NoCopyConstructor, context); return; }

        var type = this.FindReturnType(out var xtype, out var xnull,
            Symbol, [Symbol.AllBaseTypes, Symbol.AllInterfaces])
            ? xtype : Symbol;

        var options = type.ReturnOptions(Symbol);
        var rtype = type.EasyName(options);
        var rnull = xnull ? "?" : string.Empty;
        var modifiers = GetRegularModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone()");
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

        /// <summary>
        /// Obtains the appropriate method modifiers, or null if any. If not null, ends with a
        /// space terminator.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetRegularModifiers()
        {
            if (Symbol.Name == "RType4C") { } // DEBUG-ONLY

            var hsealed = Symbol.IsSealed || Symbol.IsSealed;
            var hvirt = this.FindUseVirtual(out var temp, out _,
                Symbol, [Symbol.AllBaseTypes, Symbol.AllInterfaces])
                ? temp : true;

            // Using the host base types and interfaces, not the host itself...
            var found = Finder.Find(out string? value, (type, out value) =>
            {
                // Method existing...
                while (this.FindMethod(out var method, type, []))
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

                // Requested...
                if (type.HasCloneableAttribute(out _))
                {
                    if (type.IsInterface)
                    {
                        value = hsealed || !hvirt ? $"public " : $"public virtual ";
                        return true;
                    }

                    var mvirt = this.FindUseVirtual(out temp, out _,
                        type, [type.AllBaseTypes, type.AllInterfaces]) ? temp : true;

                    value = (hsealed || !mvirt) ? $"public new " : $"public override ";
                    return true;
                }

                // Try next...
                value = null;
                return false;
            },
            null, [Symbol.AllBaseTypes, Symbol.AllInterfaces]);

            // Returning found or default value...
            return found ? value : ((hsealed || !hvirt) ? "public " : "public virtual ");
        }
    }

    // ----------------------------------------------------

    record Explicit(INamedTypeSymbol IFace, INamedTypeSymbol RType, bool RNullable);

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

            // ICloneable...
            if (iface.Name == "ICloneable")
            {
                var item = new Explicit(iface, null!, false); // 'null' will be intercepted later
                list.Add(item);
                return;
            }

            // Method existing...
            if (this.FindMethod(out var method, iface, []))
            {
                var rtype = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out var rnull);
                var item = new Explicit(iface, rtype, rnull);
                list.Add(item);
                return;
            }

            // Requested...
            if (iface.HasCloneableAttribute(out var at))
            {
                var rtype = this.FindReturnType(out var xtype, out var rnull,
                    iface, [iface.AllInterfaces])
                    ? xtype : iface;

                var item = new Explicit(iface, rtype, rnull);
                list.Add(item);
                return;
            }
        }
    }
}