#pragma warning disable IDE0075

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
            TreeDiagnostics.KindNotSupported(Symbol).Report(context);
            r = false;
        }

        if (!base.Validate(context)) r = false;

        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented exlicitly...
        if (FindMethod(Symbol) != null) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitCoreAsInterface(context, cb);
        else if (Symbol.IsAbstract) EmitCoreAsAbstract(context, cb);
        else EmitCoreAsConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitCoreAsInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        // Gets the method modifiers, or null if any.
        // If we find the method in any interface (which covers the 'IClonable' attribute), or the
        // CloneableAttribute (as its generation should have emitted a method), we use 'new' as we
        // re-declare the method to return the new host type.
        string? GetModifiers()
        {
            var found = Symbol.AllInterfaces.Any(x =>
                FindMethod(x) != null ||
                FindCloneableAttribute(x) != null);

            return found ? "new " : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitCoreAsAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        EmitCoreExplicitInterfaces(context, cb);

        // Gets the method modifiers, or null if any.
        // The implemented method will always be an 'abstract' one, and might add 'override' in
        // some scenarios.
        string? GetModifiers()
        {
            if (Symbol.BaseType != null) 
            {
                // There might be a base method...
                var method = FindMethod(Symbol.BaseType, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;

                    return isvirtual ? $"{access}abstract override " : $"{access}abstract ";
                }

                // Might already be implemented because the attribute applied to base items...
                var at = FindCloneableAttribute(Symbol.BaseType, chain: true);
                if (at != null) return "public abstract override ";
            }

            // The default for abstract types...
            return "public abstract ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitCoreAsConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        // We need a copy constructor...
        var ctor = Symbol.GetCopyConstructor(strict: true);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
            return;
        }

        // Emitting...
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_temp = new {typename}(this);");
            cb.AppendLine("return v_temp;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitCoreExplicitInterfaces(context, cb);

        // Gets the method modifiers, or null if any.
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the explicit method implementations for the inherited methods. Note that
    /// this method only makes sense for abstract and concrete types, but not for interface ones.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitCoreExplicitInterfaces(SourceProductionContext context, CodeBuilder cb)
    {
        var ifaces = GetExplicitInterfaces();

        foreach (var iface in ifaces)
        {
            var typename = iface.EasyName(RoslynNameOptions.Full with { UseTypeNullable = false });
            var valuename = iface.Name == "ICloneable" ? "object" : typename;

            cb.AppendLine();
            cb.AppendLine(valuename);
            cb.AppendLine($"{typename}.Clone() => ({typename})Clone();");
        }
    }

    /// <summary>
    /// Returns a list with the interface types that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        var comparer = SymbolComparer.Default;
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) Capture(iface);
        return list;

        // Tries to capture the given interface...
        bool Capture(ITypeSymbol iface)
        {
            var found = false;

            foreach (var child in iface.Interfaces)
            {
                var temp = Capture(child);
                if (temp) found = true;
            }

            found = found ||
                iface.Name == "ICloneable" ||
                FindMethod(iface) != null ||
                FindCloneableAttribute(iface) != null;

            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }

            return found;
        }
    }

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
    /// Tries to find the generator's method in the given type, including its base types and
    /// interfaces if requested.
    /// <br/> Returns <c>null</c> if not found.
    /// </summary>
    IMethodSymbol? FindMethod(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

        if (item == null && chain) // Inheritance chain requested...
        {
            foreach (var temp in type.AllBaseTypes())
                if ((item = FindMethod(temp)) != null) break;
        }

        if (item == null && ifaces) // Interfaces requested...
        {
            foreach (var temp in type.AllInterfaces)
                if ((item = FindMethod(temp)) != null) break;
        }

        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the <see cref="CloneableAttribute"/> in the given type, including its
    /// inheritance and interfaces chains if requested.
    /// <br/> Returns <c>null</c> if not found.
    /// </summary>
    AttributeData? FindCloneableAttribute(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var at = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();

        if (at == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if ((at = FindCloneableAttribute(temp)) != null) break;
        }

        if (at == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if ((at = FindCloneableAttribute(temp)) != null) break;
        }

        return at;
    }

    /// <summary>
    /// Tries to get the value of the 'AddVirtual' property in the given attribute. Returns
    /// <c>false</c> if the setting is not found, or otherwise <c>true</c> and the setting's
    /// value in the out argument.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetAddVirtualValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.AddVirtual), out var arg))
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
    /// Tries to get the value of the 'AddVirtual' property from the attributes applied to the
    /// given type, including its inheritance and interfaces chains if requested. Returns
    /// <c>false</c> if the setting is not found, or otherwise <c>true</c> and the setting's
    /// value in the out argument.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    bool GetAddVirtualValue(
        ITypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        var at = FindCloneableAttribute(type);
        if (at != null)
        {
            if (GetAddVirtualValue(at, out value)) return true;
        }

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetAddVirtualValue(temp, out value)) return true;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetAddVirtualValue(temp, out value)) return true;
        }

        value = default;
        return false;
    }
}
