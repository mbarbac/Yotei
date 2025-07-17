using System.Xml;

namespace Yotei.ORM.Generators;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    const string IListNamespace = "Yotei.ORM.Tools";
    const string ListNamespace = "Yotei.ORM.Tools.Code";

    const string ListName = "InvariantList";
    const string IListName = "IInvariantList";

    AttributeData AttributeData = null!;
    INamedTypeSymbol AttributeClass = null!;
    int Arity = 0;
    INamedTypeSymbol KType = null!; bool IsKTypeNullable = false; // Arity == 2
    INamedTypeSymbol TType = null!; bool IsTTypeNullable = false; // Arity == 1 or 2

    INamedTypeSymbol ReturnType = null!;
    Type Template = null!;
    string KTypeName = null!;
    string TTypeName = null!;
    string AttributeName = null!;
    string BracketName = null!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Capturing the decoration information...
        if (!CaptureAttribute(context)) return false;

        // Capturing arity and types
        if (!CaptureArityAndTypes(AttributeClass.Arity, context)) return false;

        // We need to validate copy constructor's presence if host is not an interface...
        if (!ValidateCopyConstructor(context)) return false;

        // Capturing the return type to use...
        if ((ReturnType = GetReturnType(context)) == null) return false;

        // Finishing...
        Template = Arity == 1 ? typeof(IChainTemplate<>) : typeof(IChainTemplate<,>);
        KTypeName = KType?.EasyName(RoslynNameOptions.Full)!; if (IsKTypeNullable) KTypeName += "?";
        TTypeName = TType?.EasyName(RoslynNameOptions.Full)!; if (IsTTypeNullable) TTypeName += "?";
        AttributeName = AttributeClass.Name.RemoveEnd("Attribute");
        BracketName = Arity == 1 ? $"<{TTypeName}>" : $"<{KTypeName}, {TTypeName}>";

        return true;
    }

    /// <summary>
    /// Captures the attribute-related information that decorates this host. Reports an error
    /// if cannot be captured.
    /// </summary>
    /// <returns></returns>
    bool CaptureAttribute(SourceProductionContext context)
    {
        if (Candidate != null)
        {
            var len = Candidate.Attributes.Length;
            if (len == 0) { InvariantListDiagnostics.NoAttributes(Symbol).Report(context); return false; }
            if (len > 1) { InvariantListDiagnostics.TooManyAttributes(Symbol).Report(context); return false; }
            
            AttributeData = Candidate.Attributes[0];
        }
        else
        {
            var ats = Symbol.GetAttributes().Where(x =>
                x.AttributeClass != null && (
                x.AttributeClass.Name.StartsWith(ListName) ||
                x.AttributeClass.Name.StartsWith(IListName)))
                .ToArray();

            if (ats.Length == 0) { InvariantListDiagnostics.NoAttributes(Symbol).Report(context); return false; }
            if (ats.Length > 1) { InvariantListDiagnostics.TooManyAttributes(Symbol).Report(context); return false; }
            
            AttributeData = ats[0];
        }

        if ((AttributeClass = AttributeData.AttributeClass!) == null)
        {
            InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Captures the given arity and, based upon its value, captures the appropriate attribute
    /// types. Reports an error if cannot be captured.
    /// </summary>
    bool CaptureArityAndTypes(int arity, SourceProductionContext context)
    {
        if (arity == 0) // No generic parameters...
        {
            var args = AttributeData.ConstructorArguments;

            if (args.Length == 1)
            {
                TType = GetUnderlyingType(args[0], out IsTTypeNullable, context);
                if (TType != null) { Arity = 1; return true; }
            }
            else if (args.Length == 2)
            {
                KType = GetUnderlyingType(args[0], out IsKTypeNullable, context);
                if (KType != null)
                {
                    TType = GetUnderlyingType(args[1], out IsTTypeNullable, context);
                    if (TType != null) { Arity = 2; return true; }
                }
            }
        }
        else if (arity == 1) // One generic <T> parameter...
        {
            TType = GetUnderlyingType((INamedTypeSymbol)AttributeClass.TypeArguments[0], out IsTTypeNullable, context);
            if (TType != null) { Arity = 1; return true; }
        }
        else if (arity == 2) // Two generic <K, T> parameters...
        {
            KType = GetUnderlyingType((INamedTypeSymbol)AttributeClass.TypeArguments[0], out IsKTypeNullable, context);
            if (KType != null)
            {
                TType = GetUnderlyingType((INamedTypeSymbol)AttributeClass.TypeArguments[1], out IsTTypeNullable, context);
                if (TType != null) { Arity = 2; return true; }
            }
        }
        
        // Invalid attribute...
        {
            InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
            return false;
        }
    }

    /// <summary>
    /// Invoked to get the actual underlying type, and if it is a nullable one or not.
    /// <br/> Return null and reports an appropriate error if it cannot be found.
    /// </summary>
    INamedTypeSymbol GetUnderlyingType(
        TypedConstant source, out bool nullable, SourceProductionContext context)
    {
        if (source.IsNull || source.Kind != TypedConstantKind.Type)
        {
            InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
            nullable = false;
            return null!;
        }

        var type = (INamedTypeSymbol)source.Value!;
        return GetUnderlyingType(type, out nullable, context);
    }

    /// <summary>
    /// Invoked to get the actual underlying type, and if it is a nullable one or not.
    /// <br/> Return null and reports an appropriate error if it cannot be found.
    /// </summary>
    INamedTypeSymbol GetUnderlyingType(
        INamedTypeSymbol source, out bool nullable, SourceProductionContext context)
    {
        if (source.Name is "Nullable" or "AsNullable")
        {
            if (source.TypeArguments.Length == 1)
            {
                source = (INamedTypeSymbol)source.TypeArguments[0];
                nullable = true;
                return source;
            }

            InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
            nullable = false;
            return null!;
        }
        else
        {
            nullable = false;
            return source;
        }
    }

    /// <summary>
    /// Validates, if the host is not an interface, that a copy constructor is present.
    /// </summary>
    bool ValidateCopyConstructor(SourceProductionContext context)
    {
        if (!Symbol.IsInterface())
        {
            var cons = Symbol.GetCopyConstructor();
            if (cons == null)
            {
                TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns the interface type to be used to redeclare or to reimplement the methods on the
    /// invariant template. That type can either be the host symbol, if it is an interface, or
    /// its top-most one that ultimately inherits from an invariant one.
    /// <br/> Returns null and reports an error if that return interface type cannot be found.
    /// </summary>
    INamedTypeSymbol GetReturnType(SourceProductionContext context)
    {
        // Interfaces being implemented have as return type themselves...
        if (Symbol.IsInterface()) return Symbol;

        // Findind the top-most one...
        foreach (var iface in Symbol.Interfaces) if (IsReturnType(iface)) return iface;

        InvariantListDiagnostics.NoInvariantInterface(Symbol).Report(context);
        return null!;

        // Determines if the given type ultimately inherits from an invariant one...
        bool IsReturnType(INamedTypeSymbol iface)
        {
            if (iface.Name.StartsWith(IListName)) return true;

            var ats = iface.GetAttributes();
            foreach (var at in ats)
                if (at.AttributeClass != null &&
                    at.AttributeClass.Name.StartsWith(IListName)) return true;

            foreach(var child in iface.Interfaces) if (IsReturnType(child)) return true;

            return false;
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var nsname = Symbol.IsInterface() ? IListNamespace : ListNamespace;
        var name = $"{nsname}.{AttributeName}{BracketName}";
        var head = base.GetHeader(context) + $" : {name}";

        return head;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Emitting 'Clone()' method is needed...
        TryEmitCloneMethod(cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to emit a 'Clone()' method, if needed.
    /// </summary>
    void TryEmitCloneMethod(CodeBuilder cb)
    {
        // If already declared or implemented we are done...
        if (HasCloneMethod(Symbol)) return;

        // Documenting...
        cb.AppendLine("/// <inheritdoc cref=\"ICloneable.Clone\"/>");
        cb.AppendLine(AttributeDoc);

        // Host is an interface...
        if (Symbol.IsInterface())
        {
            var name = Symbol.EasyName();
            cb.AppendLine($"new {name} Clone();");
        }

        // Otherwise we are inheriting from an abstract class...
        else
        {
            var name = Symbol.EasyName();
            var retname = ReturnType.EasyName();
            cb.AppendLine($"public override {retname} Clone() => new {name}(this);");

            foreach (var iface in FindCloneInterfaces(Symbol))
            {
                name = iface.EasyName();

                cb.AppendLine();
                cb.AppendLine($"{name}");
                cb.AppendLine($"{name}.Clone() => Clone();");
            }
        }
    }

    /// <summary>
    /// Returns the collection of interfaces whose 'Clone()' methods need reimplementation.
    /// </summary>
    IEnumerable<INamedTypeSymbol> FindCloneInterfaces(INamedTypeSymbol type)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given type has a clone-alike method declared, implemented or requested,
    /// including also its base types and interfaces if requested.
    /// </summary>
    bool HasCloneMethod(INamedTypeSymbol type, bool chain = false, bool ifaces = true)
    {
        if (type.Name == "ICloneable") return true;

        var method = FindCloneMethod(type);
        if (method != null) return true;

        var ats = type.GetAttributes();
        foreach (var at in ats)
            if (at.AttributeClass != null &&
                at.AttributeClass.Name.Contains("Cloneable")) return true;

        if (chain)
            foreach (var child in type.AllBaseTypes())
                if (HasCloneMethod((INamedTypeSymbol)child)) return true;

        if (ifaces)
            foreach (var child in type.AllInterfaces)
                if (HasCloneMethod(child)) return true;

        return false;
    }

    /// <summary>
    /// Finds the actual 'Clone()' method declared or implemented in the given type, or null if
    /// any.
    /// </summary>
    IMethodSymbol? FindCloneMethod(INamedTypeSymbol type)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        return item;
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantListGenerator)}}", "{{VersionDoc}}")]
        """;
}