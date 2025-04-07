using Microsoft.CodeAnalysis.Diagnostics;

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
    protected void EmitAsInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if no modifiers.
        /// </summary>
        string? GetModifiers()
        {
            var found = false;
            var host = Symbol.BaseType;

            if (!found) found = Symbol.AllInterfaces.Any(x => x.Name == "ICloneable");
            if (!found) found = GetAddICloneableValue(Symbol, out var value, ifaces: true) && value;
            if (!found && host != null) found = FindCloneMethod(host, ifaces: true) != null;
            if (!found && host != null) found = FindCloneableAttribute(host, ifaces: true) != null;

            return found ? "new " : null;
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
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        EmitExplicitInterfaces(context, cb);

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if no modifiers.
        /// </summary>
        string? GetModifiers()
        {
            // When symbol is a derived one...
            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // There might already be a base method...
                var method = FindCloneMethod(host, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;

                    return isvirtual ? $"{access}abstract override " : $"{access}abstract ";
                }

                // Or that method is being implemented...
                var at = FindCloneableAttribute(host, chain: true, ifaces: true);
                if (at != null)
                {
                    return "public abstract override ";
                }
            }

            // Default for abstract types...
            return "public abstract ";
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

        EmitExplicitInterfaces(context, cb);

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if no modifiers.
        /// </summary>
        string? GetModifiers()
        {
            var issealed = Symbol.IsSealed;
            var prevent = GetPreventVirtualValue(Symbol, out var temp, chain: true, ifaces: true) && temp;

            // When symbol is a derived one...
            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // There might already be a base method...
                var method = FindCloneMethod(host, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;

                    var str = access.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;

                    return isvirtual switch
                    {
                        true => $"{str}override ",
                        false => $"{str}new ",
                    };
                }

                // Or that method is being implemented...
                var at = FindCloneableAttribute(host, chain: true, ifaces: true);
                if (at != null)
                {
                    return prevent ? "public new " : "public override ";
                }
            }

            // Default for concrete types...
            return prevent || issealed ? "public " : "public virtual ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the explicit interface implementations associated to this type, if any.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="cb"></param>
    protected void EmitExplicitInterfaces(SourceProductionContext _, CodeBuilder cb)
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
        var comparer = SymbolEqualityComparer.Default;
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) Capture(iface);
        TryICloneable();
        return list;

        /// <summary>
        /// Tries to capture the given interface type.
        /// </summary>
        bool Capture(ITypeSymbol iface)
        {
            var found = false;

            // First, its child interfaces...
            foreach (var child in iface.Interfaces)
            {
                var temp = Capture(child);
                if (temp) found = true;
            }

            // An them the given interface itself....
            found = found ||
                iface.Name == "ICloneable" ||
                FindCloneMethod(iface) != null ||
                FindCloneableAttribute(iface) != null;

            // Adding if needed, and finishing...
            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }

            return found;
        }

        /// <summary>
        /// Treat <see cref="CloneableAttribute.AddICloneable"/> as a special case.
        /// </summary>
        void TryICloneable()
        {
            var add = GetAddICloneableValue(Symbol, out var value, chain: true, ifaces: true) && value;
            if (add)
            {
                var comp = GetBranchCompilation();
                var item = comp.GetTypeByMetadataName("System.ICloneable");
                if (item != null)
                {
                    var temp = list.Find(x => comparer.Equals(x, item));
                    if (temp == null) list.Add(item);
                }
            }
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
    /// Tries to find a '<c>Clone()</c>' method in the given type, including also its base types
    /// and interfaces if requested. Returns null if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
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
    /// Tries to find a '<see cref="CloneableAttribute"/>' attribute in the given type,
    /// including also its base types and interfaces if requested. Returns null if not found.
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