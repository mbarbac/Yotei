using System.Net.Mail;

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
    [SuppressMessage("", "IDE0290")]
    public XPropertyNode(IPropertySymbol symbol) : base(symbol) { }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (Host.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); return false; }
        if (Symbol.IsIndexer) { Symbol.ReportError(TreeError.IndexerNotSupported, context); return false; }
        if (!Symbol.HasGetter) { Symbol.ReportError(TreeError.NoGetter, context); return false; }
        if (!Symbol.HasSetter && !Host.IsInterface) { Symbol.ReportError(TreeError.NoSetter, context); return false; }

        return base.Validate(context);
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
        if (FindMethod(out _, Host)) return;

        // Capturing working data...
        if (!CaptureWorkingData(context)) return;

        // Dispatching...
        if (Host.IsInterface) EmitHostInterface(context, cb);
        else if (Host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    /// <summary>
    /// Captures working data at emit time. Returns true if generation can continue, or false
    /// otherwise.
    /// </summary>
    bool CaptureWorkingData(SourceProductionContext context)
    {
        MethodName = $"With{Symbol.Name}";
        ArgumentName = $"v_{Symbol.Name}";
        UseVirtual = true;
        ReturnType = Host;
        ReturnNullable = false;
        ReturnOptions = EasyTypeSymbol.Default;

        // Finding the source attribute...
        AttributeData at;

        if (!IsInherited) // Captured per-se...
        {
            if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); return false; }
            if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); return false; }
            at = Attributes[0];
        }
        else // Captured by the host inheriting members...
        {
            var ats = Host.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]).ToList();
            if (ats.Count == 0) { Host.ReportError(TreeError.NoAttributes, context); return false; }
            if (ats.Count > 1) { Host.ReportError(TreeError.TooManyAttributes, context); return false; }
            at = ats[0];
        }

        // Return type...
        if (XNode.FindReturnType(at, out var type, out var nullable))
        {
            ReturnType = type;
            ReturnNullable = nullable;

            var same = SymbolEqualityComparer.Default.Equals(Host, type);
            if (!same) ReturnOptions = EasyTypeSymbol.Full with
            { NullableStyle = IsNullableStyle.None };
        }

        // Use virtual...
        if (XNode.FindUseVirtual(at, out var virt)) UseVirtual = virt;

        // Finishing...
        return true;
    }

    string MethodName = default!;
    string ArgumentName = default!;
    bool UseVirtual = default;
    INamedTypeSymbol ReturnType = default!;
    bool ReturnNullable = default;
    EasyTypeSymbol ReturnOptions = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);
        var modifiers = GetInterfaceModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({argtype} {ArgumentName});");
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// </summary>
    string? GetInterfaceModifiers()
    {
        var found = Finder.Find((type, out value) =>
        {
            if (FindMethod(out _, type) || XNode.FindWithAttribute(out _, out _, Symbol.Name, type))
            {
                value = "new ";
                return true;
            }
            value = default;
            return false;
        },
        out string? value, null, Host.AllInterfaces);
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
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);
        var modifiers = GetAbstractModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({argtype} {ArgumentName});");

        EmitExplicitInterfaces(context, cb);
    }

    /// <summary>
    /// Invoked to obtain the appropriate method modifiers.
    /// </summary>
    /// Base        Modifier
    /// ---------------------------------------------------
    /// interface   abstract
    /// abstract    abstract override
    /// regular     abstract new
    /// virt        abstract override
    string? GetAbstractModifiers() => null; // HIGH: GetAbstractModifiers

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Host.FindCopyConstructor(strict: false);
        if (ctor == null)
        {
            Symbol.ReportError(TreeError.NoCopyConstructor, context);
            Host.ReportError(TreeError.NoCopyConstructor, context);
            return;
        }

        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var argtype = Symbol.Type.EasyName(EasyTypeSymbol.Full);
        var modifiers = GetRegularModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({argtype} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var hostname = Host.EasyName();
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
    /// </summary>
    /// Base        Derived     Sealed  Modifier
    /// ---------------------------------------------------
    /// regular     regular     no      new
    /// regular     virt        no      new virtual
    /// regular     regular     yes     new
    /// regular     virt        yes     new
    /// ---------------------------------------------------
    /// virt        regular     no      new
    /// virt        virt        no      override
    /// virt        regular     yes     override
    /// virt        virt        yes     override
    string? GetRegularModifiers() => null; // HIGH: GetRegularModifiers

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation, if any.
    /// </summary>
    void EmitExplicitInterfaces(SourceProductionContext context, CodeBuilder cb)
    {
        var ifaces = GetExplicitInterfaces();
        foreach (var item in ifaces)
        {
            var iface = item.IFace;
            var mtype = item.MemberType;

            var ifacename = iface.EasyName(EasyTypeSymbol.Full with { NullableStyle = IsNullableStyle.None });
            var nullable = ReturnNullable ? "?" : string.Empty;
            var mtypename = mtype.EasyName(EasyTypeSymbol.Full);

            cb.AppendLine();
            cb.AppendLine($"{ifacename}{nullable}");
            cb.AppendLine($"{ifacename}.{MethodName}({mtypename} value)");
            cb.AppendLine($"=> {MethodName}(value);");
            cb.AppendLine($"");
        }
    }

    /// <summary>
    /// Represents a explicit interface along with the type of the member at its level.
    /// </summary>
    record Explicit(INamedTypeSymbol IFace, INamedTypeSymbol MemberType);

    /// <summary>
    /// Obtains a list with the interfaces that need explicit implementation, along with the
    /// actual type of the member.
    /// </summary>
    List<Explicit> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        List<Explicit> list = [];
        foreach (var iface in Host.Interfaces) TryCaptureAt(iface);
        return list;

        bool TryCaptureAt(INamedTypeSymbol iface)
        {
            var found = false;
            INamedTypeSymbol? membertype = null;

            foreach (var child in iface.Interfaces)
                if (TryCaptureAt(child)) { membertype = child; found = true; }

            if (!found)
                if (FindDecoratedMember(out var temp, iface))
                { membertype = (INamedTypeSymbol)temp.Type; found = true; }

            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x.IFace, iface));
                if (temp == null) list.Add(new(iface, membertype!));
            }

            return found;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a compatible member at the given type, if not null, and at the types of the
    /// given arrays.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    bool FindMember(
        [NotNullWhen(true)] out IPropertySymbol? value,
        INamedTypeSymbol? type, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            value = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == MethodName);

            return value != null;
        },
        out value, type, chains);
    }

    /// <summary>
    /// Tries to find a compatible decorated member at the given type, if not null, and at the types
    /// of the given arrays.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    bool FindDecoratedMember(
        [NotNullWhen(true)] out IPropertySymbol? value,
        INamedTypeSymbol? type, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            value = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == MethodName && (
                x.HasAttributes(typeof(WithAttribute)) || x.HasAttributes(typeof(WithAttribute<>))));

            return value != null;
        },
        out value, type, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a suitable method at the given type, if not null, and at the types of the
    /// given arrays.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    bool FindMethod(
        [NotNullWhen(true)] out IMethodSymbol? value,
        INamedTypeSymbol? type, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == MethodName &&
                x.Parameters.Length == 1 &&
                Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

            return value != null;
        },
        out value, type, chains);
    }
}