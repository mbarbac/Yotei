namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        var r = true;

        if (Symbol.IsRecord)
        {
            TreeDiagnostics.RecordsNotSupported(Symbol).Report(context);
            r = false;
        }

        if (!base.Validate(context)) r = false;

        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var head = base.GetHeader(context);
        var add = GetAddICloneableValue(Symbol, out var temp, chain: true, ifaces: true) && temp;

        if (add) head += " : ICloneable";
        return head;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented explicitly...
        if (FindCloneMethod(Symbol) != null) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitAsInterface(context, cb);
        else if (Symbol.IsAbstract) EmitAsAbstract(context, cb);
        else EmitAsConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitAsInterface(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if no modifiers.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitAsAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if no modifiers.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete or regular one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitAsConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if no modifiers.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the explicit interface implementations associated to this type, if any.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="cb"></param>
    /// <exception cref="System.NullReferenceException"></exception>
    protected void EmitExplicitInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        throw null;
    }

    /// <summary>
    /// Returns a list with the interface types that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        throw null;
    }

    // ----------------------------------------------------

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(CloneGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated code.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{AttributeDoc}}
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a '<c>Clone()</c>' method in the given type, including also its base types
    /// and interfaces if requested. Returns null if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public IMethodSymbol? FindCloneMethod(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                item = FindCloneMethod(temp);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                item = FindCloneMethod(temp);
                if (item != null) break;
            }
        }

        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a '<c>CloneableAttribute</c>' attribute in the given type, including also
    /// its base types and interfaces if requested. Returns null if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public static AttributeData? FindCloneableAttribute(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false)
    {
        var at = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();

        if (at == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                at = FindCloneableAttribute(temp);
                if (at != null) break;
            }
        }

        if (at == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                at = FindCloneableAttribute(temp);
                if (at != null) break;
            }
        }

        return at;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the '<see cref="CloneableAttribute.PreventVirtual"/>'
    /// named argument from the given attribute data. Returns <c>true</c> if the value is found,
    /// and the value itself in the <paramref name="value"/> parameter, or false otherwise.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetPreventVirtualValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.PreventVirtual), out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the '<see cref="CloneableAttribute.PreventVirtual"/>'
    /// named argument from that attribute applied to the given type, if any. Returns <c>true</c>
    /// if the value is found, and the value itself in the <paramref name="value"/> parameter, or
    /// false otherwise.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public static bool GetPreventVirtualValue(
        ITypeSymbol type,
        out bool value,
        bool chain = false, bool ifaces = false)
    {
        var at = FindCloneableAttribute(type, chain, ifaces);
        if (at != null)
            if (GetPreventVirtualValue(at, out value)) return true;

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetPreventVirtualValue(temp, out value)) return true;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetPreventVirtualValue(temp, out value)) return true;
        }

        value = default;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the '<see cref="CloneableAttribute.AddICloneable"/>'
    /// named argument from the given attribute data. Returns <c>true</c> if the value is found,
    /// and the value itself in the <paramref name="value"/> parameter, or false otherwise.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetAddICloneableValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.AddICloneable), out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the '<see cref="CloneableAttribute.AddICloneable"/>'
    /// named argument from that attribute applied to the given type, if any. Returns <c>true</c>
    /// if the value is found, and the value itself in the <paramref name="value"/> parameter, or
    /// false otherwise.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public static bool GetAddICloneableValue(
        ITypeSymbol type,
        out bool value,
        bool chain = false, bool ifaces = false)
    {
        var at = FindCloneableAttribute(type, chain, ifaces);
        if (at != null)
            if (GetAddICloneableValue(at, out value)) return true;

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetAddICloneableValue(temp, out value)) return true;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetAddICloneableValue(temp, out value)) return true;
        }

        value = default;
        return false;
    }
}