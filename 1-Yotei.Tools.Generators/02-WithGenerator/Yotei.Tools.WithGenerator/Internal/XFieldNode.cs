namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XFieldNode : FieldNode, IXNode<IFieldSymbol>
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XFieldNode(IFieldSymbol symbol) : base(symbol) { }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }
    INamedTypeSymbol Host => ParentNode!.Symbol;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);

        if (Host.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }
        if (!Host.IsInterface &&
            !Symbol.IsWrittable) { Symbol.ReportError(TreeError.NotWrittable, context); r = false; }

        if (!IsInherited) // Member captured per-se...
        {
            if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
            if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }
        }
        else // Captured because its Host inherits members...
        {
            var ats = Host.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]).ToList();
            if (Attributes.Count == 0) { Host.ReportError(TreeError.NoAttributes, context); r = false; }
            if (ats.Count > 1) { Host.ReportError(TreeError.TooManyAttributes, context); r = false; }
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
        if (this.FindMethod(out _, Host, [])) return;

        // Dispatching...
        if (Host.IsInterface) EmitHostInterface(context, cb);
        else if (Host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    [SuppressMessage("", "IDE0060")]
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var type = this.FindReturnType(
            out var xtype, out var xnul, Host, [Host.AllInterfaces])
            ? xtype : Host;

        var options = type.ReturnOptions(Host);
        var rtype = type.EasyName(options);
        var rnull = xnul ? "?" : string.Empty;
        var modifiers = GetInterfaceModifiers();
        var argtype = this.SymbolType.EasyName(EasyTypeSymbol.Full);

        this.EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{this.MethodName}({argtype} value);");

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
                // Member existing...
                if (this.FindMember(out var member, out var at, type, []))
                {
                    value = "new ";
                    return true;
                }

                // Method existing...
                if (this.FindMethod(out var method, type, []))
                {
                    value = "new ";
                    return true;
                }

                // Requested...
                if (type.HasInheritsWithAttribute(out _) &&
                    this.FindMember(out _, out _, null, [type.AllInterfaces]))
                {
                    value = "new ";
                    return true;
                }

                // Try next...
                value = null;
                return false;
            },
            null, [Host.AllInterfaces]);

            // Returning found or default value...
            return found ? value : "public abstract ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    [SuppressMessage("", "IDE0060")]
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var type = this.FindReturnType(
            out var xtype, out var xnul, Host, [Host.AllBaseTypes, Host.AllInterfaces])
            ? xtype : Host;

        var options = type.ReturnOptions(Host);
        var rtype = type.EasyName(options);
        var rnull = xnul ? "?" : string.Empty;
        var modifiers = GetAbstractModifiers();
        var argtype = this.SymbolType.EasyName(EasyTypeSymbol.Full);

        this.EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{this.MethodName}({argtype} value);");

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
                // Member existing...
                if (this.FindMember(out var member, out var at, type, []))
                {
                    if (type.IsInterface) { value = $"public abstract "; return true; }

                    var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                    value = mvirt ? "public abstract override " : "public abstract new ";
                    return true;
                }

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
                if (type.HasInheritsWithAttribute(out _) &&
                    this.FindMember(out _, out _, null, [type.AllBaseTypes, type.AllInterfaces]))
                {
                    var mvirt = this.FindUseVirtual(out var temp, out _, out _,
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
            null, [Host.AllBaseTypes, Host.AllInterfaces]);

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
        var ctor = Host.FindCopyConstructor(strict: false);
        if (ctor == null) { Host.ReportError(TreeError.NoCopyConstructor, context); return; }

        var type = this.FindReturnType(out var xtype, out var xnul,
            Host, [Host.AllBaseTypes, Host.AllInterfaces])
            ? xtype : Host;

        var options = type.ReturnOptions(Host);
        var rtype = type.EasyName(options);
        var rnull = xnul ? "?" : string.Empty;
        var modifiers = GetRegularModifiers();
        var argtype = this.SymbolType.EasyName(EasyTypeSymbol.Full);

        this.EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{this.MethodName}({argtype} value)");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var host = Host.EasyName();
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

        /// <summary>
        /// Obtains the appropriate method modifiers, or null if any. If not null, ends with a
        /// space terminator.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetRegularModifiers()
        {
            var hsealed = Host.IsSealed || Symbol.IsSealed;
            var hvirt = this.FindUseVirtual(out var temp, out _, out _,
                Host, [Host.AllBaseTypes, Host.AllInterfaces])
                ? temp : true;

            // Using the host base types and interfaces, not the host itself...
            var found = Finder.Find(out string? value, (type, out value) =>
            {
                // Member existing...
                if (this.FindMember(out var member, out var at, type, []))
                {
                    if (type.IsInterface)
                    {
                        value = hsealed || !hvirt ? "public " : "public virtual ";
                        return true;
                    }

                    var mvirt = at.HasUseVirtual(out temp) ? temp : true;
                    value = mvirt ? "public override " : "public new ";
                    return true;
                }

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
                if (type.HasInheritsWithAttribute(out _) &&
                    this.FindMember(out _, out _, null, [type.AllBaseTypes, type.AllInterfaces]))
                {
                    var mvirt = this.FindUseVirtual(out temp, out _, out _,
                        type, [type.AllBaseTypes, type.AllInterfaces])
                        ? temp : true;

                    value = type.IsInterface
                        ? "public virtual "
                        : (hsealed || !mvirt ? $"public new " : $"public override ");

                    return true;
                }

                // Try next...
                value = null;
                return false;
            },
            null, [Host.AllBaseTypes, Host.AllInterfaces]);

            // Returning found or default value...
            return found ? value : ((hsealed || !hvirt) ? "public " : "public virtual ");
        }
    }
}