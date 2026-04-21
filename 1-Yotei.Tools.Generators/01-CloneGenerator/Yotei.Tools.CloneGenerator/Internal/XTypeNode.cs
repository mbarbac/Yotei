namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public class XTypeNode : TypeNode
{
    AttributeData Attribute = default!;

    /// <summary>
    /// Initializes a new instance.
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
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        if (Symbol.IsRecord) { TreeError.RecordsNotSupported.Report(Symbol, context); r = false; }

        if (Attributes.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
        else if (Attributes.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
        else Attribute = Attributes[0];

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected override bool OnEmitCore(ref TreeContext context, CodeBuilder cb)
    {
        // If already exist we're done...
        if (Helpers.TryFindMethod(Symbol, [], out _)) return true;

        // Otherwise, dispatching...
        if (Symbol.IsInterface) return EmitHostInterface(ref context, cb);
        else if (Symbol.IsAbstract) return EmitHostAbstract(ref context, cb);
        else return EmitHostRegular(ref context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    bool EmitHostInterface(ref TreeContext _, CodeBuilder cb)
    {
        var hasrtype = Attribute.HasReturnType(out var rtype, out var rnull);
        if (!hasrtype) { rtype = Symbol; rnull = false; }

        var options = rtype!.GetReturnOptions(Symbol);
        var strtype = rtype!.EasyName(options);
        var strnull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();

        CloneGenerator.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{strtype}{strnull} Clone();");

        return true;

        /// <summary>
        /// Obtains the appropriate modifiers (followed by an space separator), or null if any.
        /// </summary>
        string? GetModifiers()
        {
            if (!Attribute.HasUseVirtual(out var hvirt)) hvirt = true;
            var hsealed = Symbol.IsSealed;

            var found = Finder.Find(
                [Symbol.AllBaseTypes, Symbol.AllInterfaces], out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (Helpers.TryFindMethod(type, [], out var method))
                    {
                        // We need an explicit accessibility...
                        var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                        var str = dec.ToAccessibilityString(false); if (str == null) break;

                        value = dec == Accessibility.Public ? "new " : $"{str} new ";
                        return true;
                    }

                    // Method requested...
                    while (type.HasCloneableAttribute(out var _, out var _))
                    {
                        value = $"new ";
                        return true;
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
    /// Invoked when the host type is an abstract type.
    /// </summary>
    bool EmitHostAbstract(ref TreeContext _, CodeBuilder cb)
    {
        var hasrtype = Attribute.HasReturnType(out var rtype, out var rnull);
        if (!hasrtype) { rtype = Symbol; rnull = false; }

        var options = rtype!.GetReturnOptions(Symbol);
        var strtype = rtype!.EasyName(options);
        var strnull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();

        CloneGenerator.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{strtype}{strnull} Clone();");

        EmitExplicitInterfaces(cb);
        return true;

        /// <summary>
        /// Obtains the appropriate modifiers (followed by an space separator), or null if any.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetModifiers()
        {
            if (!Attribute.HasUseVirtual(out var hvirt)) hvirt = true;
            var hsealed = Symbol.IsSealed;

            var found = Finder.Find(
                [Symbol.AllBaseTypes, Symbol.AllInterfaces], out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (Helpers.TryFindMethod(type, [], out var method))
                    {
                        // We need an explicit accessibility...
                        var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                        var str = dec.ToAccessibilityString(false); if (str == null) break;

                        if (type.IsInterface) { value = $"{str} abstract "; return true; }
                        else if (type.IsAbstract) { value = $"{str} abstract override "; return true; }
                        else
                        {
                            var mvirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                            value = !mvirt ? $"{str} abstract new " : $"{str} abstract override ";
                            return true;
                        }
                    }

                    // Method requested...
                    while (type.HasCloneableAttribute(out var at, out var _))
                    {
                        if (type.IsInterface) { value = "public abstract "; return true; }
                        if (type.IsAbstract) { value = "public abstract override "; return true; }
                        else
                        {
                            // If appears in a base method, defer to it...
                            if (Helpers.TryFindMethod(null, [type.AllBaseTypes], out var _)) break;

                            // Otherwise, use the attribute...
                            var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                            value = !mvirt ? "public abstract new " : "public abstract override ";

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
    /// Invoked when the host type is a regular type.
    /// </summary>
    bool EmitHostRegular(ref TreeContext context, CodeBuilder cb)
    {
        var ctor = Symbol.FindCopyConstructor(strict: false);
        if (ctor == null) { TreeError.NoCopyConstructor.Report(Symbol, context.Context); return false; }

        var hasrtype = Attribute.HasReturnType(out var rtype, out var rnull);
        if (!hasrtype) { rtype = Symbol; rnull = false; }

        var options = rtype!.GetReturnOptions(Symbol);
        var strtype = rtype!.EasyName(options);
        var strnull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();

        CloneGenerator.EmitDocumentation(cb);
        cb.AppendLine($"{mods}{strtype}{strnull} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var host = Symbol.EasyName();
            cb.AppendLine($"var host = new {host}(this);");
            cb.AppendLine($"return host;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitExplicitInterfaces(cb);
        return true;

        /// <summary>
        /// Obtains the appropriate modifiers (followed by an space separator), or null if any.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetModifiers()
        {
            if (!Attribute.HasUseVirtual(out var hvirt)) hvirt = true;
            var hsealed = Symbol.IsSealed;

            var found = Finder.Find(
                [Symbol.AllBaseTypes, Symbol.AllInterfaces], out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (Helpers.TryFindMethod(type, [], out var method))
                    {
                        // We need an explicit accessibility...
                        var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                        var str = dec.ToAccessibilityString(false); if (str == null) break;

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

                    // Method requested...
                    while (type.HasCloneableAttribute(out var at, out _))
                    {
                        if (type.IsInterface)
                        {
                            value = hsealed || !hvirt ? $"public " : $"public virtual ";
                            return true;
                        }
                        else
                        {
                            // If appears in a base method, defer to it...
                            if (Helpers.TryFindMethod(null, [type.AllBaseTypes], out _)) break;

                            // Otherwise, use the attribute...
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
    /// Invoked to emit the interfaces that need explicit implementation.
    /// </summary>
    /// <param name="cb"></param>
    void EmitExplicitInterfaces(CodeBuilder cb)
    {
        var options = EasyTypeOptions.Full with { NullableStyle = EasyNullableStyle.None };

        var items = GetExplicitInterfaces();
        foreach (var item in items)
        {
            var iface = item.IFace.EasyName(options);
            var core = item.IFace.Name == nameof(ICloneable); // Special 'ICloneable' case...
            var rtype = core ? "object" : item.RType.EasyName(EasyTypeOptions.Full);
            if (item.RNullable && !rtype.EndsWith('?')) rtype += '?';

            cb.AppendLine();
            cb.AppendLine($"{rtype}");
            cb.AppendLine($"{iface}.Clone()");
            cb.AppendLine($"=> ({rtype})Clone();");
        }
    }

    /// <summary>
    /// Keeps track of an interface that needs explicit implementation.
    /// </summary>
    /// <param name="IFace">The interface itself.</param>
    /// <param name="RType">The method return type.</param>
    /// <param name="RNullable">Wheter the return type is nullable or not.</param>
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

        /// <summary>
        /// Tries to capture the given interface.
        /// </summary>
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x.IFace, iface));
            if (temp is not null) return;

            // Special case: ICloneable...
            if (iface.Name == nameof(ICloneable))
            {
                // We can use 'null' as rtype because in this special case it won't be used...
                var item = new Explicit(iface, null!, false);
                list.Add(item);
                return;
            }

            // Method already exists...
            if (Helpers.TryFindMethod(iface, [], out var method))
            {
                var rtype = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out var rnull);
                var item = new Explicit(iface, rtype, rnull);
                list.Add(item);
                return;
            }

            // Method is requested...
            if (iface.HasCloneableAttribute(out var at, out _))
            {
                var rtype = at.HasReturnType(out var xtype, out var rnull) ? xtype : iface;
                var item = new Explicit(iface, rtype, rnull);
                list.Add(item);
                return;
            }
        }
    }
}