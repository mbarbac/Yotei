#pragma warning disable IDE0059

using System.Data;

namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol ReturnType = default!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Other validations...
        if (Symbol.IsRecord)
        {
            TreeDiagnostics.RecordsNotSupported(Symbol).Report(context);
            return false;
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var head = base.GetHeader(context);
        var add = FindAddICloneableValue(Symbol, out var found) && found;

        if (add) head += " : ICloneable";
        return head;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Capturing...
        if (!CaptureReturnType(out ReturnType, context)) return;

        // Declared or implemented explicitly...
        if (FindCloneMethod(Symbol, out _) != null) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the return type to use with the generated methods.
    /// </summary>
    bool CaptureReturnType(out INamedTypeSymbol type, SourceProductionContext context)
    {
        // Interfaces are valid per-se...
        if (Symbol.IsInterface()) { type = Symbol; return true; }

        // Return decorated...
        var dec = FindReturnsDecoratedValue(Symbol, out var found, Symbol.AllBaseTypes(), Symbol.AllInterfaces);
        if (found && dec) { type = Symbol; return true; }

        // Return interface...
        type = Symbol.Recursive((INamedTypeSymbol iface, out bool found) =>
        {
            found = false;
            if (!iface.IsInterface()) return null;
            if (iface.Name == "ICloneable") return null;

            if (FindCloneMethod(iface, out found) is not null) return iface;
            if (FindCloneableAttribute(iface, out found) is not null) return iface;
            return null;
        },
        out found, Symbol.Interfaces)!;
        if (found && type is not null) return true;

        // Cannot find an interface...
        ReportNoInterfaceToReturn(context);
        type = Symbol;
        return true;
    }

    void ReportNoInterfaceToReturn(SourceProductionContext context)
    {
        var id = "Cloneable01";
        var head = "No Interface to return.";
        var desc = $"Cannot find an interface to be the return type for '{Symbol.Name}'.";
        var severity = DiagnosticSeverity.Warning;
        var location =
            Symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation() ??
            Symbol.Locations.FirstOrDefault();

        var item = Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);

        context.ReportDiagnostic(item);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var done = Symbol.Recursive((INamedTypeSymbol iface, out bool found) =>
            {
                if (Symbol.Name == "IFace04") { } // DEBUG...

                if (FindCloneableAttribute(iface, out found) is not null && found) return true;
                if (FindCloneMethod(iface, out found) is not null && found) return true;
                found = false;
                return false;
            },
            out bool found,
            Symbol.AllInterfaces);

            return found && done ? "new " : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var options = ReturnType.IsInterface() ? RoslynNameOptions.Full with { UseTypeNullable = false } : RoslynNameOptions.Default;
        var retname = ReturnType.EasyName(options);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{retname} Clone();");

        EmitExplicitInterfaces(cb);

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // If there is a base method...
                var method = FindCloneMethod(host, out var found, host.AllBaseTypes());
                if (method != null)
                {
                    // Reusing base accesibility...
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;
                    var str = access.ToCSharpString(addspace: true);

                    // We may need to re-abstract...
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;
                    return isvirtual ? $"{str}abstract override " : $"{str}abstract ";
                }

                // If method requested, we need to re-abstract...
                var at = FindCloneableAttribute(host, out found, host.AllBaseTypes());
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
    /// Invoked when the host type is a concrete type.
    /// </summary>
    void EmitHostConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        // // We need a copy constructor...
        var ctor = Symbol.GetCopyConstructor(strict: false);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
            return;
        }

        // Emitting...
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName();
        var options = ReturnType.IsInterface() ? RoslynNameOptions.Full with { UseTypeNullable = false } : RoslynNameOptions.Default;
        var retname = ReturnType.EasyName(options);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{retname} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_temp = new {typename}(this);");
            cb.AppendLine("return v_temp;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitExplicitInterfaces(cb);

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var issealed = Symbol.IsSealed;
            var prevent = FindPreventVirtualValue(Symbol, out var found, Symbol.AllBaseTypes(), Symbol.AllInterfaces);

            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // If there is a base method...
                var method = FindCloneMethod(host, out found, host.AllBaseTypes());
                if (method != null)
                {
                    // Reusing base accesibility...
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;
                    var str = access.ToCSharpString(addspace: true);

                    // Returning...
                    if (prevent) return $"{str}override sealed ";
                    else
                    {
                        var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;
                        return isvirtual ? $"{str}override " : $"{str}new ";
                    }
                }

                // If method requested, we need to re-abstract...
                var at = FindCloneableAttribute(host, out found, host.AllBaseTypes());
                if (at != null)
                {
                    return host.IsAbstract || !prevent ? "public override " : "public new ";
                }
            }

            // Default for concrete types...
            return prevent || issealed ? "public " : "public virtual ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the implementation of the needed explicit interfaces.
    /// </summary>
    void EmitExplicitInterfaces(CodeBuilder cb)
    {
        var comparer = SymbolComparer.Default;
        var ifaces = GetExplicitInterfaces();

        foreach (var iface in ifaces)
        {
            if (comparer.Equals(iface, ReturnType)) continue;

            var typename = iface.EasyName(RoslynNameOptions.Full with { UseTypeNullable = false });
            var valuename = iface.Name == "ICloneable" ? "object" : typename;

            cb.AppendLine();
            cb.AppendLine(valuename);
            cb.AppendLine($"{typename}.Clone() => ({typename})Clone();");
        }
    }

    /// <summary>
    /// Gets a list with the interfaces that need explicit implementation.
    /// </summary>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        TryAddICloneable();
        return list;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        void TryCapture(INamedTypeSymbol iface)
        {
            if (FindCloneMethod(iface, out _) is not null ||
                FindCloneableAttribute(iface, out _) is not null &&
                list.Find(x => comparer.Equals(x, iface)) == null)
                list.Add(iface);

            /*
            // First the childs...
            var found = false;
            foreach (var child in iface.Interfaces)
            {
                var temp = TryCapture(child);
                if (temp) found = true;
            }

            // Then, the interface itself...
            found = found ||
                iface.Name == "ICloneable" ||
                FindCloneMethod(iface, out _) != null ||
                FindCloneableAttribute(iface, out _) != null;

            // Adding to the list if needed, and finishing...
            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }
            return found;*/
        }

        /// <summary>
        /// Tries to add the <see cref="ICloneable"/> interface to the given list, if such has
        /// been requested.
        /// </summary>
        void TryAddICloneable()
        {
            var any = Symbol.AllInterfaces.Any(x => x.Name == "ICloneable");
            var add = any || FindAddICloneableValue(Symbol, out var found) && found;

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

    /// <summary>
    /// Tries to find a 'Clone' method in the given type or chains.
    /// </summary>
    static IMethodSymbol? FindCloneMethod(
        INamedTypeSymbol type, out bool found, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out bool found) =>
        {
            var method = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0 &&
                x.ReturnsVoid == false);

            found = method is not null;
            return method;
        },
        out found, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a <see cref="CloneableAttribute"/> attribute in the given type or chains.
    /// </summary>
    static AttributeData? FindCloneableAttribute(
        INamedTypeSymbol type, out bool found, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out bool found) =>
        {
            var at = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();

            found = at is not null;
            return at;
        },
        out found, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the  <see cref="CloneableAttribute.ReturnsDecorated"/> element
    /// in the given attribute data.
    /// </summary>
    static bool FindReturnsDecoratedValue(AttributeData at, out bool found)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.ReturnsDecorated), out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                found = true;
                return temp;
            }
        }
        found = false;
        return false;
    }

    /// <summary>
    /// Tries to find the value of the  <see cref="CloneableAttribute.ReturnsDecorated"/> element
    /// in the given type or chains.
    /// </summary>
    static bool FindReturnsDecoratedValue(
        INamedTypeSymbol type, out bool found, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out bool found) =>
        {
            var at = FindCloneableAttribute(type, out found, chains);
            if (found && at is not null)
            {
                var value = FindReturnsDecoratedValue(at, out found);
                if (found) return value;
            }
            return false;
        },
        out found, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the  <see cref="CloneableAttribute.PreventVirtual"/> element
    /// in the given attribute data.
    /// </summary>
    static bool FindPreventVirtualValue(AttributeData at, out bool found)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.PreventVirtual), out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                found = true;
                return temp;
            }
        }
        found = false;
        return false;
    }

    /// <summary>
    /// Tries to find the value of the  <see cref="CloneableAttribute.PreventVirtual"/> element
    /// in the given type or chains.
    /// </summary>
    static bool FindPreventVirtualValue(
        INamedTypeSymbol type, out bool found, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out bool found) =>
        {
            var at = FindCloneableAttribute(type, out found, chains);
            if (found && at is not null)
            {
                var value = FindPreventVirtualValue(at, out found);
                if (found) return value;
            }
            return false;
        },
        out found, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the  <see cref="CloneableAttribute.AddICloneable"/> element
    /// in the given attribute data.
    /// </summary>
    static bool FindAddICloneableValue(AttributeData at, out bool found)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.AddICloneable), out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                found = true;
                return temp;
            }
        }
        found = false;
        return false;
    }

    /// <summary>
    /// Tries to find the value of the  <see cref="CloneableAttribute.AddICloneable"/> element
    /// in the given type or chains.
    /// </summary>
    static bool FindAddICloneableValue(
        INamedTypeSymbol type, out bool found, params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out bool found) =>
        {
            var at = FindCloneableAttribute(type, out found, chains);
            if (found && at is not null)
            {
                var value = FindAddICloneableValue(at, out found);
                if (found) return value;
            }
            return false;
        },
        out found, chains);
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
}