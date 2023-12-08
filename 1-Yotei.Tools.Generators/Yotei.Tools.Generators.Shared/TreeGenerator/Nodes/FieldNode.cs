namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a field-alike source generation node.
/// </summary>
internal class FieldNode : Node
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="candidate"></param>
    public FieldNode(FieldCandidate candidate) : base(candidate) { }

    /// <summary>
    /// Initializes a new instance not associated with a given candidate.
    /// </summary>
    /// <param name="symbol"></param>
    public FieldNode(IFieldSymbol symbol) : base(symbol) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Field: {Symbol.ToStringEx(useSymbolType: true)}";

    /// <summary>
    /// <inheritdoc cref="Node.Candidate"/>
    /// </summary>
    public new FieldCandidate? Candidate => (FieldCandidate?)base.Candidate;

    /// <summary>
    /// <inheritdoc cref="Node.Symbol"/>
    /// </summary>
    public new IFieldSymbol Symbol => _Symbol ??= GetSymbol();
    IFieldSymbol? _Symbol;

    IFieldSymbol GetSymbol()
    {
        var psymbol = (IFieldSymbol)base.Symbol;

        if (_HostType is not null)
        {
            var type = _HostType; while (type != null)
            {
                var member = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x =>
                {
                    return x.Name == psymbol.Name;
                });
                if (member != null) return member;
                type = type.BaseType;
            }
        }
        return psymbol;
    }

    /// <summary>
    /// The host type for which the source code of this instance will be emitted. If it is not
    /// explicitly set, then its value is taken from the actual containing type of this instance.
    /// </summary>
    public INamedTypeSymbol HostType
    {
        get => _HostType ?? Symbol.ContainingType;
        set
        {
            _HostType = value;
            _Symbol = null;
        }
    }
    INamedTypeSymbol _HostType = default!;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        // Chain of types must be partial ones...
        var type = HostType;
        while (type != null)
        {
            if (!type.TypeIsPartial(context)) return false;
            type = type.ContainingType;
        }

        // Type must be of a supported kind...
        if (!HostType.TypeIsSupported(context)) return false;

        // Validated...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    public override void Print(SourceProductionContext context, FileBuilder file)
    {
        file.AppendLine($"// {ToString()}");
    }
}