using Microsoft.CodeAnalysis.Operations;
using System.Reflection.Metadata;

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
    public XPropertyNode(IPropertySymbol symbol) : base(symbol)
    {
        MethodName = $"With{Symbol.Name}";
        ArgumentName = $"v_{Symbol.Name}";
    }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }

    readonly string MethodName = default!;
    readonly string ArgumentName = default!;

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

        if (!IsInherited) // Member captured per-se...
        {
            if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
            if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }
        }
        else // Captured because its host inherits members...
        {
            var ats = host.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]).ToList();
            if (Attributes.Count == 0) { host.ReportError(TreeError.NoAttributes, context); r = false; }
            if (ats.Count > 1) { host.ReportError(TreeError.TooManyAttributes, context); r = false; }
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
        var host = ParentNode!.Symbol;

        // Intercepting explicitly implemented...
        if (XNode.FindMethod(MethodName, Symbol.Type, out _, host)) return;

        // Dispatching...
        if (host.IsInterface) EmitHostInterface(context, cb);
        else if (host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    [SuppressMessage("", "IDE0060")]
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var host = ParentNode!.Symbol;
        var type = XNode.FindReturnType<IPropertySymbol>(
            Symbol.Name,
            out var xtype, out var xnullable, out _, host, host.AllInterfaces) ? xtype : host;

        var roptions = XNode.ReturnOptions(host, type);
        var rtype = type.EasyName(roptions);
        var rnull = xnullable ? "?" : string.Empty;
        var modifiers = GetInterfaceModifiers();
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({argtype} {ArgumentName});");
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// <br/> If not null, ends with a space separator.
    /// </summary>
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var host = ParentNode!.Symbol;
        var type = XNode.FindReturnType<IPropertySymbol>(
            Symbol.Name,
            out var xtype, out var xnullable, out _, host, host.AllInterfaces) ? xtype : host;

        var roptions = XNode.ReturnOptions(host, type);
        var rtype = type.EasyName(roptions);
        var rnull = xnullable ? "?" : string.Empty;
        var modifiers = GetAbstractModifiers();
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({argtype} {ArgumentName});");

        EmitExplicitInterfaces(context, cb);
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// <br/> If not null, ends with a space separator.
    /// </summary>
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

            // Method requested...
            if (XNode.FindDecoratedMember<IPropertySymbol>(Symbol.Name, out var member, out var at, type))
            {
                if (type.IsInterface) { value = $"public abstract "; return true; }

                var mvirt = at.HasUseVirtual(out var temp) ? temp : true;
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var host = ParentNode!.Symbol;
        var ctor = host.FindCopyConstructor(strict: false);
        if (ctor == null) { host.ReportError(TreeError.NoCopyConstructor, context); return; }

        var type = XNode.FindReturnType<IPropertySymbol>(
            Symbol.Name,
            out var xtype, out var xnullable, out _, host, host.AllInterfaces) ? xtype : host;

        var roptions = XNode.ReturnOptions(host, type);
        var rtype = type.EasyName(roptions);
        var rnull = xnullable ? "?" : string.Empty;
        var modifiers = GetRegularModifiers();
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);

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
    /// <br/> If not null, ends with a space separator.
    /// </summary>
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

            // Method requested...
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

    // ----------------------------------------------------

    record ExplicitInfo(
        INamedTypeSymbol RType, bool RNullable,
        INamedTypeSymbol IFace,
        INamedTypeSymbol MType);

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation, if any.
    /// </summary>
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
}