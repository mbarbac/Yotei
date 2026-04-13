namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured type-alike source code generation node.
/// </summary>
public record TypeNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="syntax"></param>
    public TypeNode(BaseTypeDeclarationSyntax syntax)
    {
        ArgumentNullException.ThrowIfNull(syntax);
        Container = ContainerInfo.Create(syntax, childsOnly: false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ContainerInfo"/>
    /// </summary>
    public ContainerInfo Container { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<Diagnostic> Diagnostics { get; } = [];
}