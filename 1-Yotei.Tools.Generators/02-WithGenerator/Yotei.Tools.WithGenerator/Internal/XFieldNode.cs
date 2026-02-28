#pragma warning disable IDE0075

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
    AttributeData Attribute = default!;
    INamedTypeSymbol Host => ParentNode!.Symbol;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        if (Host.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }
        if (!Symbol.IsWrittable) { Symbol.ReportError(TreeError.NotWrittable, context); r = false; }

        if (IsInherited)
        {
            var ats = Host.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]).ToList();
            if (ats.Count == 0) { Host.ReportError(TreeError.NoAttributes, context); r = false; }
            else if (ats.Count > 1) { Host.ReportError(TreeError.TooManyAttributes, context); r = false; }
            else Attribute = ats[0];
        }
        else
        {
            if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
            else if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }
            else Attribute = Attributes[0];
        }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnEmit(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicitly implemented...
        if (this.FindMethod(Host, [], out _)) return true;

        // Dispatching...
        if (Host.IsInterface) return EmitHostInterface(context, cb);
        else if (Host.IsAbstract) return EmitHostAbstract(context, cb);
        else return EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    [SuppressMessage("", "IDE0060")]
    bool EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var rtype = Host;
        var rnull = false;
        if (Attribute.HasReturnType(out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = rtype.ReturnOptions(Host);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;
        var sarg = this.SymbolType.EasyName(EasyTypeSymbol.Full);
        var mods = GetModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{stype}{snull}");
        cb.AppendLine($"{this.MethodName}({sarg} value);");

        return true;

        /// <summary>
        /// Obtains the appropriate method modifiers, followed by a space separator, or null,
        /// </summary>
        string? GetModifiers()
        {
            // If appear in base types, modifiers must adapt...
            var found = Finder.Find(
                null, [Host.AllInterfaces],
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

                    // Existing member...
                    while (this.FindMember(type, [], out _, out _))
                    {
                        value = $"new ";
                        return true;
                    }

                    // Requested...
                    while (type.HasInheritsWithAttribute(out _))
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
    [SuppressMessage("", "IDE0060")]
    bool EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var rtype = Host;
        var rnull = false;
        if (Attribute.HasReturnType(out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = rtype.ReturnOptions(Host);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;
        var sarg = this.SymbolType.EasyName(EasyTypeSymbol.Full);
        var mods = GetModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{stype}{snull}");
        cb.AppendLine($"{this.MethodName}({sarg} value);");

        return true;

        /// <summary>
        /// Obtains the appropriate method modifiers, followed by a space separator, or null,
        /// </summary>
        string? GetModifiers()
        {
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

                    // Existing member...
                    while (this.FindMember(type, [], out var member, out var at))
                    {
                        if (type.IsInterface) { value = $"public abstract "; return true; }

                        var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                        value = !mvirt
                            ? $"public abstract new "
                            : $"public abstract override ";
                    }

                    // Requested...
                    while (type.HasInheritsWithAttribute(out var at))
                    {
                        if (type.IsInterface) { value = $"public abstract "; return true; }
                        if (type.IsAbstract) { value = $"public abstract override "; return true; }
                        else
                        {
                            // If appear in a base method, let's defer to it...
                            if (this.FindMethod(null, [type.AllBaseTypes], out _) ||
                                this.FindMember(null, [type.AllBaseTypes], out _, out _))
                                break;

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
        var ctor = Host.FindCopyConstructor(strict: false);
        if (ctor == null) { Host.ReportError(TreeError.NoCopyConstructor, context); return false; }

        var rtype = Host;
        var rnull = false;
        if (Attribute.HasReturnType(out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = rtype.ReturnOptions(Host);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;
        var sarg = this.SymbolType.EasyName(EasyTypeSymbol.Full);
        var mods = GetModifiers();

        this.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{stype}{snull}");
        cb.AppendLine($"{this.MethodName}({sarg} value)");
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

        return true;

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
                        else
                        {
                            var mvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                            value = mvirt
                                ? (!hvirt ? $"{str} new " : $"{str} override ")
                                : (!hvirt ? $"{str} new " : $"{str} new virtual ");

                            return true;
                        }
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
                        else
                        {
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
                    }

                    // Try next...
                    value = default;
                    return false;
                });


            // Finishing...
            return found ? value : (hsealed || !hvirt ? "public " : "public virtual ");
        }
    }
}