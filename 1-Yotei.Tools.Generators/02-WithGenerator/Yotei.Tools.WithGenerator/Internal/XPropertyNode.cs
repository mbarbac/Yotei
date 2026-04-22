using System.Xml;

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public class XPropertyNode : PropertyNode, IXNode<IPropertySymbol>
{
    AttributeData Attribute = default!;
    INamedTypeSymbol Host => Parent!.Symbol;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XPropertyNode(IPropertySymbol symbol) : base(symbol) { }

    /// <summary>
    /// Determines if this instance was captured for an inherited member, or not.
    /// </summary>
    public bool Inherited { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        if (Host.IsRecord) { TreeError.RecordsNotSupported.Report(Symbol, context); r = false; }

        if (Symbol.IsIndexer) { TreeError.IndexerNotSupported.Report(Symbol, context); r = false; }
        if (!Symbol.HasGetter) { TreeError.NoGetter.Report(Symbol, context); r = false; }
        if (!Symbol.HasSetter &&
            !Symbol.ContainingType.IsInterface) { TreeError.NoSetter.Report(Symbol, context); r = false; }

        if (Inherited) // Member is injected by the host inheriting it...
        {
            var ats = Host.GetAttributes([typeof(InheritsWithAttribute)]).ToList();
            if (ats.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
            else if (ats.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
            else Attribute = ats[0];
        }
        else // Member captured by itself...
        {
            if (Attributes.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
            else if (Attributes.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
            else Attribute = Attributes[0];
        }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnEmit(ref TreeContext context, CodeBuilder cb)
    {
        // If already exist we're done...
        if (XNode.TryFindMethod(this.MethodName, this.MemberType, Host, [], out _)) return true;

        // Otherwise, dispatching...
        if (Host.IsInterface) return EmitHostInterface(ref context, cb);
        else if (Host.IsAbstract) return EmitHostAbstract(ref context, cb);
        else return EmitHostRegular(ref context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    bool EmitHostInterface(ref TreeContext _, CodeBuilder cb)
    {
        if (!Attribute.HasReturnType(out var rtype, out var rnull)) { rtype = Host; rnull = false; }
        var options = rtype!.GetReturnOptions(Host);
        var strtype = rtype!.EasyName(options);
        var strnull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();
        var strarg = this.MemberType.EasyName(EasyTypeOptions.Full);

        WithGenerator.EmitDocumentation(cb, this.MemberName);
        cb.AppendLine($"{mods}{strtype}{strnull}");
        cb.AppendLine($"{this.MethodName}({strarg} value);");

        return true;

        /// <summary>
        /// Obtains the appropriate modifiers (followed by an space separator), or null if any.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetModifiers()
        {
            if (!Attribute.HasUseVirtual(out var hvirt)) hvirt = true;
            var hsealed = Symbol.IsSealed;
            var membername = this.MemberName;
            var methodname = this.MethodName;
            var argtype = this.MemberType;

            var found = Finder.Find(
                [Host.AllBaseTypes, Host.AllInterfaces], out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (XNode.TryFindMethod(methodname, argtype, type, [], out var method))
                    {
                        // We need an explicit accessibility...
                        var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                        var str = dec.ToAccessibilityString(false); if (str == null) break;

                        value = dec == Accessibility.Public ? "new " : $"{str} new ";
                        return true;
                    }

                    // Existing member...
                    while (XNode.TryFindMember<IPropertySymbol>(membername, type, [], out var member, out var _))
                    {
                        value = $"new ";
                        return true;
                    }

                    // Method requested...
                    while (type.HasInheritsWithAttribute(out var _))
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
    /// Invoked when the host type is an interface.
    /// </summary>
    bool EmitHostAbstract(ref TreeContext _, CodeBuilder cb)
    {
        if (!Attribute.HasReturnType(out var rtype, out var rnull)) { rtype = Host; rnull = false; }
        var options = rtype!.GetReturnOptions(Host);
        var strtype = rtype!.EasyName(options);
        var strnull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();
        var strarg = this.MemberType.EasyName(EasyTypeOptions.Full);

        WithGenerator.EmitDocumentation(cb, this.MemberName);
        cb.AppendLine($"{mods}{strtype}{strnull}");
        cb.AppendLine($"{this.MethodName}({strarg} value);");

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
            var membername = this.MemberName;
            var methodname = this.MethodName;
            var argtype = this.MemberType;

            var found = Finder.Find(
                [Host.AllBaseTypes, Host.AllInterfaces], out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (XNode.TryFindMethod(methodname, argtype, type, [], out var method))
                    {
                        // We need an explicit accessibility...
                        var dec = method.DeclaredAccessibility; if (dec == Accessibility.Private) break;
                        var str = dec.ToAccessibilityString(false); if (str == null) break;

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
                    while (XNode.TryFindMember<IPropertySymbol>(membername, type, [], out var member, out var at))
                    {
                        if (type.IsInterface) { value = $"public abstract "; return true; }

                        var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                        value = !mvirt
                            ? $"public abstract override "
                            : $"public abstract override ";

                        return true;
                    }

                    // Method requested...
                    while (type.HasInheritsWithAttribute(out var at))
                    {
                        if (type.IsInterface) { value = $"public abstract "; return true; }
                        if (type.IsAbstract) { value = $"public abstract override "; return true; }
                        else
                        {
                            // If appears in a base method, defer to it...
                            if (XNode.TryFindMethod(methodname, argtype, null, [type.AllBaseTypes], out var _) ||
                                XNode.TryFindMember<IPropertySymbol>(membername, null, [type.AllBaseTypes], out var _, out var _))
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
    /// Invoked when the host type is an interface.
    /// </summary>
    bool EmitHostRegular(ref TreeContext context, CodeBuilder cb)
    {
        var ctor = Host.FindCopyConstructor(strict: false);
        if (ctor == null) { TreeError.NoCopyConstructor.Report(Host, context.Context); return false; }

        if (!Attribute.HasReturnType(out var rtype, out var rnull)) { rtype = Host; rnull = false; }
        var options = rtype!.GetReturnOptions(Host);
        var strtype = rtype!.EasyName(options);
        var strnull = rnull ? "?" : string.Empty;
        var mods = GetModifiers();
        var strarg = this.MemberType.EasyName(EasyTypeOptions.Full);

        WithGenerator.EmitDocumentation(cb, this.MemberName);
        cb.AppendLine($"{mods}{strtype}{strnull}");
        cb.AppendLine($"{this.MethodName}({strarg} value)");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var host = Host.EasyName();

            cb.AppendLine($"var host = new {host}(this)");
            cb.AppendLine("{");
            cb.IndentLevel++;
            {
                cb.AppendLine($"{this.MemberName} = value");
            }
            cb.IndentLevel--;
            cb.AppendLine("};");
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
            var membername = this.MemberName;
            var methodname = this.MethodName;
            var argtype = this.MemberType;

            var found = Finder.Find(
                [Host.AllBaseTypes, Host.AllInterfaces], out string? value,
                (type, out value) =>
                {
                    // Existing method...
                    while (XNode.TryFindMethod(methodname, argtype, type, [], out var method))
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

                    // Existing member...
                    while (XNode.TryFindMember<IPropertySymbol>(membername, type, [], out var member, out var at))
                    {
                        if (type.IsInterface)
                        {
                            value = hsealed || !hvirt ? $"public " : $"public virtual ";
                            return true;
                        }
                        else
                        {
                            var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
                            value = mvirt
                                ? (!hvirt ? $"public new " : $"public override ")
                                : (!hvirt ? $"public new " : $"public new virtual ");

                            return true;
                        }
                    }

                    // Method requested...
                    while (type.HasInheritsWithAttribute(out var at))
                    {
                        if (type.IsInterface)
                        {
                            value = hsealed || !hvirt ? $"public " : $"public virtual ";
                            return true;
                        }
                        else
                        {
                            // If appears in a base method, defer to it...
                            if (XNode.TryFindMethod(methodname, argtype, null, [type.AllBaseTypes], out _)) break;

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
        var mname = this.MethodName;

        var items = GetExplicitInterfaces();
        foreach (var item in items)
        {
            var iface = item.IFace.EasyName(options);
            var rtype = item.RType.EasyName(EasyTypeOptions.Full);
            if (item.RNullable && !rtype.EndsWith('?')) rtype += '?';
            var atype = item.ArgType.EasyName(EasyTypeOptions.Full);

            cb.AppendLine();
            cb.AppendLine($"{rtype}");
            cb.AppendLine($"{iface}.{mname}({atype} value)");
            cb.AppendLine($"=> ({rtype}){mname}(value);");
        }
    }

    /// <summary>
    /// Keeps track of an interface that needs explicit implementation.
    /// </summary>
    /// <param name="IFace">The interface itself.</param>
    /// <param name="RType">The method return type.</param>
    /// <param name="RNullable">Wheter the return type is nullable or not.</param>
    /// <param name="ArgType">The argument type.</param>
    record Explicit(
        INamedTypeSymbol IFace,
        INamedTypeSymbol RType, bool RNullable, ITypeSymbol ArgType);


    /// <summary>
    /// Obtains the list of interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<Explicit> GetExplicitInterfaces()
    {
        var membername = this.MemberName;
        var methodname = this.MethodName;
        var argtype = this.MemberType;

        var comparer = SymbolEqualityComparer.Default;
        List<Explicit> list = [];

        foreach (var iface in Host.Interfaces) TryCapture(iface);
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

            // Method already exists...
            if (XNode.TryFindMethod(methodname, argtype, iface, [], out var method))
            {
                var rtype = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out var rnull);
                var atype = method.Parameters[0].Type;
                var item = new Explicit(iface, rtype, rnull, atype);
                list.Add(item);
                return;
            }

            // Decorated member exists...
            if (XNode.TryFindMember<IPropertySymbol>(membername, iface, [], out var member, out var at))
            {
                if (!at.HasReturnType(out var rtype, out var rnull)) rtype = iface;
                var atype = member.Type;
                var item = new Explicit(iface, rtype, rnull, atype);
                list.Add(item);
                return;
            }

            // Method is requested...
            if (iface.HasInheritsWithAttribute(out at))
            {
                if (!at.HasReturnType(out var rtype, out var rnull)) rtype = iface;
                var atype = argtype;
                var item = new Explicit(iface, rtype, rnull, atype);
                list.Add(item);
                return;
            }
        }
    }
}