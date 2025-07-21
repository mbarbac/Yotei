#pragma warning disable IDE0075

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        bool r = true;

        // Base validations...
        if (!base.Validate(context)) r = false;

        // Other validations...
        if (Host.IsRecord) { TreeDiagnostics.RecordsNotSupported(Host).Report(context); r = false; }
        if (Symbol.IsIndexer) { TreeDiagnostics.IndexerNotSupported(Symbol).Report(context); r = false; }
        if (!Symbol.HasGetter()) { TreeDiagnostics.NoGetter(Symbol).Report(context); r = false; }
        if (!Host.IsInterface() && !Symbol.HasSetterOrInit()) { TreeDiagnostics.NoSetter(Symbol).Report(context); r = false; }

        // Finishing...
        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented explicitly...
        //if (FindCloneMethod(Symbol) != null) return;

        // Dispatching...
        if (Host.IsInterface()) EmitAsInterface(context, cb);
        else if (Host.IsAbstract) EmitAsAbstract(context, cb);
        else EmitAsConcrete(context, cb);

        base.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    void EmitAsInterface(SourceProductionContext source, CodeBuilder cb) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract class.
    /// </summary>
    void EmitAsAbstract(SourceProductionContext source, CodeBuilder cb) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete class.
    /// </summary>
    void EmitAsConcrete(SourceProductionContext source, CodeBuilder cb) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member with the same name as this symbol in the given type, whose type
    /// is either the same as this symbol (strict mode), or just compatible with it by default.
    /// he base types and interfaces of the host type are also searched if explicitly requested.
    /// Returns <c>null</c> if not found.
    /// </summary>
    IPropertySymbol? FindMember(
        INamedTypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false, bool decorated = false)
    {
        var comparer = SymbolEqualityComparer.Default;

        var item = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.Parameters.Length == 0 &&
            (decorated
            ? x.HasAttributes(typeof(WithAttribute))
            : true) &&
            (strict
            ? comparer.Equals(Symbol.Type, x.Parameters[0].Type)
            : Symbol.Type.IsAssignableTo(x.Parameters[0].Type)));

        if (item == null && chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                item = FindMember(child, strict, false, false, decorated);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                item = FindMember(child, strict, false, false, decorated);
                if (item != null) break;
            }
        }

        return item;
    }

    /// <summary>
    /// Tries to find a member with the same name as this symbol in the given type, whose type
    /// is either the same as this symbol (strict mode), or just compatible with it by default.
    /// he base types and interfaces of the host type are also searched if explicitly requested.
    /// Returns <c>null</c> if not found.
    /// </summary>
    IPropertySymbol? FindDecoratedMember(
        INamedTypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false) => FindMember(type, strict, chain, ifaces, true);

    /// <summary>
    /// Returns the 'With()' method declared or implemented by the given type, where the method's
    /// unique argument must either be the same as this symbol (strict mode), or just compatible
    /// with it by default. The base types and interfaces of the host type are also searched if
    /// explicitly requested. Returns <c>null</c> if not found.
    /// </summary>
    IMethodSymbol? FindWithMethod(
        INamedTypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false)
    {
        var comparer = SymbolEqualityComparer.Default;

        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            (strict
            ? comparer.Equals(Symbol.Type, x.Parameters[0].Type)
            : Symbol.Type.IsAssignableTo(x.Parameters[0].Type)));

        if (item == null && chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                item = FindWithMethod(child, strict);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                item = FindWithMethod(child, strict);
                if (item != null) break;
            }
        }

        return item;
    }

    // ----------------------------------------------------

    /*static AttributeData? FindCloneableAttribute(
        INamedTypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();
        if (item != null) return item;

        if (chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                item = FindCloneableAttribute(child);
                if (item != null) return item;
            }
        }

        if (ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                item = FindCloneableAttribute(child);
                if (item != null) return item;
            }
        }

        return null;
    }*/

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated code.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the 'with' keyword for the '{{Symbol.Name}}' decorated member.
        /// </summary>
        {{AttributeDoc}}
        """);
}