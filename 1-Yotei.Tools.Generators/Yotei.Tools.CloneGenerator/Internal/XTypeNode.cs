namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        bool r = true;

        // Base validations...
        if (!base.Validate(context)) r = false;

        // Only one valid attribute at once...
        var num = Candidate is not null
            ? Candidate.Attributes.Length
            : Symbol.GetAttributes().Count(x =>
                x.AttributeClass != null &&
                x.AttributeClass.Name.StartsWith(nameof(CloneableAttribute)));

        if (num > 1)
        {
            TreeDiagnostics.TooManyAttributes(Symbol).Report(context);
            r = false;
        }

        // Records not supported...
        if (Symbol.IsRecord)
        {
            TreeDiagnostics.RecordsNotSupported(Symbol).Report(context);
            r = false;
        }

        // Finishing...
        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented explicitly...
        if (FindCloneMethod(Symbol, out _)) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        if (!FindReturnTypeValue(Symbol, out var type)) type = Symbol;
        var typename = type.EasyName(RoslynNameOptions.Full);
        var modifiers = GetModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an abstract one.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is a concrete one.
    /// </summary>
    void EmitHostConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation.
    /// </summary>
    void EmitExplicitInterfaces(CodeBuilder cb)
    {
        throw null;
    }

    /// <summary>
    /// Gets the collection of interfaces that need explicit implementation.
    /// </summary>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a 'Clone()' method on the given type, or on the first valid one in the
    /// given chains.
    /// </summary>
    static bool FindCloneMethod(
        INamedTypeSymbol type,
        out IMethodSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IMethodSymbol value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0 &&
                x.ReturnsVoid == false);

            return value is not null;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a <see cref="CloneableAttribute"/> or a <see cref="CloneableAttribute{T}"/>
    /// attribute decorating the given type, or the first valid one in the given chains.
    /// </summary>
    static bool FindCloneableAttribute(
        INamedTypeSymbol type,
        out AttributeData value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out AttributeData value) =>
        {
            value = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();
            if (value is not null) return true;

            value = type.GetAttributes(typeof(CloneableAttribute<>)).FirstOrDefault();
            if (value is not null) return true;

            return false;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find the return type from the <see cref="CloneableAttribute.ReturnType"/>
    /// property, or from the <see cref="CloneableAttribute{T}"/> one.
    /// </summary>
    static bool FindReturnTypeValue(AttributeData data, out INamedTypeSymbol value)
    {
        var cls = data.AttributeClass;
        if (cls is not null)
        {
            // Not-generic attribute...
            if (cls.Arity == 0)
            {
                if (data.GetNamedArgument(nameof(CloneableAttribute.ReturnType), out var arg))
                {
                    if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
                    {
                        value = temp;
                        return true;
                    }
                }
            }

            // Generic attribute...
            else if (cls.Arity == 1)
            {
                value = (INamedTypeSymbol)cls.TypeArguments[0];
                return true;
            }
        }

        value = null!;
        return false;
    }

    /// <summary>
    /// Invoked to find the return type from the <see cref="CloneableAttribute.ReturnType"/>
    /// property, or from the <see cref="CloneableAttribute{T}"/> one, applied to the given type,
    /// or to the first valid one in the given chains.
    /// </summary>
    static bool FindReturnTypeValue(
        INamedTypeSymbol type,
        out INamedTypeSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        value = null!;
        
        var found = FindCloneableAttribute(type, out var data, chains);
        if (found) found = FindReturnTypeValue(data, out value);
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find the return type from the <see cref="CloneableAttribute.VirtualMethod"/>
    /// property, or from the <see cref="CloneableAttribute{T}"/> one.
    /// </summary>
    static bool FindVirtualMethodValue(AttributeData data, out bool value)
    {
        if (data.GetNamedArgument(nameof(CloneableAttribute.VirtualMethod), out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = false;
        return false;
    }

    /// <summary>
    /// Invoked to find the return type from the <see cref="CloneableAttribute.VirtualMethod"/>
    /// property, or from the <see cref="CloneableAttribute{T}"/> one, applied to the given type,
    /// or to the first valid one in the given chains.
    /// </summary>
    static bool FindVirtualMethodValue(
        INamedTypeSymbol type,
        out bool value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        value = false;

        var found = FindCloneableAttribute(type, out var data, chains);
        if (found) found = FindVirtualMethodValue(data, out value);
        return found;
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