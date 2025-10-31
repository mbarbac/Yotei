using System.Reflection.Metadata;

namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal partial class CloneGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override Type[] TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override TypeNode CreateNode(
        TypeCandidate candidate)
        => new XTypeNode(candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };
}

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        ReturnType = Symbol;
        ReturnNullable = false;
        ReturnOptions = EasyNameOptions.Default;
        UseVirtual = true;
    }
    INamedTypeSymbol ReturnType;
    bool ReturnNullable;
    EasyNameOptions ReturnOptions;
    bool UseVirtual;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);

        if (Symbol.IsRecord) { CoreDiagnostics.RecordsNotSupported(Symbol).Report(context); r = false; }
        if (Attributes.Length == 0) { CoreDiagnostics.NoAttributes(Symbol).Report(context); r = false; }
        if (Attributes.Length > 1) { CoreDiagnostics.TooManyAttributes(Symbol).Report(context); r = false; }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures relevant working data at 'Emit' time.
    /// </summary>
    void CaptureEmit()
    {
        // Validate(...) ensures we have 1 and only 1 attribute...
        var at = Attributes[0];

        if (at.GetReturnType(out var type, out var nullable))
        {
            ReturnType = type;
            ReturnNullable = nullable;

            var same = SymbolEqualityComparer.Default.Equals(Symbol, type);
            if (!same) ReturnOptions = EasyNameOptions.Full with
            { TypeUseNullable = false };
        }

        if (at.GetUseVirtual(out var temp)) UseVirtual = temp;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <param name="needNL"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb, bool needNL)
    {
        // Explicitly declared or implemented...
        if (Symbol.FindMethod(true, out _)) return;

        // Capturing working data...
        CaptureEmit();

        // Dispatching...
        if (needNL) cb.AppendLine();
        if (Symbol.IsInterface) EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var modifiers = GetInterfaceModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");
    }

    /// <summary>
    /// Gets the appropriate method modifiers when the host instance is an interface one, or
    /// '<c>null</c>' if any can be obtained or are not needed. If not null, a space separator
    /// is added to the returned string.
    /// </summary>
    /// <returns></returns>
    string? GetInterfaceModifiers()
    {
        var found = Symbol.Finder<string?>(false, (parent, out value) =>
        {
            if (parent.FindMethod(true, out _) ||
                parent.FindCloneableAttribute(true, out _))
            {
                value = "new ";
                return true;
            }

            value = null!;
            return false;
        },
        out var value, Symbol.AllInterfaces);
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
        var modifiers = GetAbstractModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");

        EmitExplicitInterfaces(context, cb);
    }

    /// <summary>
    /// Gets the appropriate method modifiers when the host instance is an abstract one, or
    /// '<c>null</c>' if any can be obtained or are not needed. If not null, a space separator
    /// is added to the returned string.
    /// </summary>
    /// <returns></returns>
    string? GetAbstractModifiers()
    {
        var found = Symbol.Finder<string?>(false, (parent, out value) =>
        {
            value = null;

            // Existing base method...
            if (parent.FindMethod(true, out var method))
            {
                var access = method.DeclaredAccessibility;
                if (access != Accessibility.Private) return false;

                var str = access.EasyName(addspace: true);
                var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;
                value = isvirtual ? $"{str}abstract override " : $"{str}abstract ";
                return true;
            }

            // Or method requested...
            if (parent.FindCloneableAttribute(true, out var at))
            {
                if (parent.IsInterface) { value = "public abstract "; return true; }
                if (!parent.IsAbstract) { value = "public abstract new "; return true; }

                var usevirtual = UseVirtual;
                if (at.GetUseVirtual(out var temp)) usevirtual = temp;
                value = usevirtual ? "public abstract override " : "public abstract ";
                return true;
            }

            // Not found, try next...
            return false;
        },
        out var value, Symbol.AllBaseTypes, Symbol.AllInterfaces);
        return found ? value : "public abstract ";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Symbol.FindCopyConstructor(strict: false);
        if (ctor == null) { CoreDiagnostics.NoCopyConstructor(Symbol).Report(context); return; }

        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var modifiers = GetRegularModifiers(context);
        var hostname = Symbol.EasyName();

        EmitDocumentation(cb);
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
    /// Gets the appropriate method modifiers when the host instance is a regular one, or
    /// '<c>null</c>' if any can be obtained or are not needed. If not null, a space separator
    /// is added to the returned string.
    /// </summary>
    /// <returns></returns>
    string? GetRegularModifiers(SourceProductionContext context)
    {
        var found = Symbol.Finder<string?>(false, (parent, out value) =>
        {
            value = null;

            // Existing base method...
            if (parent.FindMethod(true, out var method))
            {
                var access = method.DeclaredAccessibility;
                if (access != Accessibility.Private) return false;

                var str = access.EasyName(addspace: true);
                var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;

                if (!UseVirtual)
                {
                    value = isvirtual ? $"{str}override sealed " : $"{str}new ";
                    return true;
                }

                value = isvirtual ? $"{str}override " : $"{str}new ";
                return true;
            }

            // Or method requested...
            if (parent.FindCloneableAttribute(true, out var at))
            {
                if (!at.GetReturnType(out var type, out var nullable)) type = Symbol;
                if (!Symbol.IsAssignableTo(ReturnType))
                {
                    CoreDiagnostics.InvalidReturnType(Symbol, ReturnType).Report(context);
                    return false;
                }

                if (parent.IsInterface)
                {
                    value = Symbol.IsSealed || !UseVirtual ? "public " : "public virtual ";
                    return true;
                }
                if (parent.IsAbstract)
                {
                    value = "public override ";
                    return true;
                }

                var usevirtual = UseVirtual;
                if (at.GetUseVirtual(out var temp)) usevirtual = temp;

                if (UseVirtual && !usevirtual) value = "public virtual new ";
                else value = usevirtual ? "public override " : "public new ";
                return true;
            }

            // Not found, try next...
            return false;
        },
        out var value, Symbol.AllBaseTypes, Symbol.AllInterfaces);
        if (found) return value;

        return !UseVirtual || Symbol.IsSealed ? "public " : "public virtual ";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation.
    /// </summary>
    void EmitExplicitInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        var options = EasyNameOptions.Full with { TypeUseNullable = false };
        var ifaces = GetExplicitInterfaces();
        foreach (var iface in ifaces)
        {
            var typename = iface.EasyName(options);
            var nullable = ReturnNullable ? "?" : string.Empty;
            var valuename = iface.Name == "ICloneable" ? "object" : $"{typename}{nullable}";

            cb.AppendLine();
            cb.AppendLine(valuename);
            cb.AppendLine($"{typename}.Clone()");
            cb.AppendLine($"=> ({valuename})Clone();");
        }
    }

    /// <summary>
    /// Gets the collection of interfaces that need explicit implementation.
    /// </summary>
    List<INamedTypeSymbol> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        List<INamedTypeSymbol> list = [];
        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        return list;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            var found = false;

            // First its childs...
            foreach (var child in iface.Interfaces) if (TryCapture(child)) found = true;

            // If no child, then maybe this interface by itself...
            if (!found)
            {
                if (iface.FindCloneableAttribute(true, out _) ||
                    iface.FindMethod(true, out _))
                    found = true;
            }

            // If found, add to the list...
            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp is null) list.Add(iface);
            }

            // Finishing...
            return found;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the appropriate documentation.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{CloneGenerator.AttributeDoc}}
        """);
}