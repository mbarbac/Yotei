namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a field-alike element captured by a source code generator.
/// </summary>
internal interface ICapturedField : ICaptured
{
    /// <summary>
    /// The parent type of this instance.
    /// </summary>
    ICapturedType Parent { get; }

    /// <summary>
    /// <inheritdoc cref="ICaptured.Syntax"/>
    /// </summary>
    new FieldDeclarationSyntax Syntax { get; }

    /// <summary>
    /// <inheritdoc cref="ICaptured.Symbol"/>
    /// </summary>
    new IFieldSymbol Symbol { get; }
}