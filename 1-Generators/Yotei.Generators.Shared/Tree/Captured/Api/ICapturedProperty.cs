namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a property-alike element captured by a source code generator.
/// </summary>
internal interface ICapturedProperty : ICaptured
{
    /// <summary>
    /// The parent type of this instance.
    /// </summary>
    ICapturedType Parent { get; }

    /// <summary>
    /// <inheritdoc cref="ICaptured.Syntax"/>
    /// </summary>
    new PropertyDeclarationSyntax Syntax { get; }

    /// <summary>
    /// <inheritdoc cref="ICaptured.Symbol"/>
    /// </summary>
    new IPropertySymbol Symbol { get; }
}