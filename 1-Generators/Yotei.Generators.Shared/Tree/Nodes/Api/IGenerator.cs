namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a tree-oriented incremental source generator. Generators that implement this type
/// must also implement the <see cref="IIncrementalGenerator"/> interface, and must be decorated
/// with the <see cref="GeneratorAttribute"/> attribute.
/// </summary>
internal interface IGenerator
{
    /// <summary>
    /// <inheritdoc cref="IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext)"/>
    /// </summary>
    void Initialize(IncrementalGeneratorInitializationContext context);

    /// <summary>
    /// Determines if this instance tries to launch a compile-time debugger, or not.
    /// </summary>
    bool LaunchDebugger { get; }

    /// <summary>
    /// Invoked to perform post-initialization activities, such as emitting code for attributes
    /// understood by this generator.
    /// </summary>
    /// <param name="context"></param>
    void PostInitialization(IncrementalGeneratorPostInitializationContext context);

    /// <summary>
    /// The fully qualified name of the attributes that decorates the syntax nodes that are to
    /// be considered as candidate nodes for code generation purposes.
    /// </summary>
    string AttributeName { get; }

    /// <summary>
    /// The level at which syntax nodes are captured by this generator.
    /// </summary>
    CaptureLevel CaptureLevel { get; }

    /// <summary>
    /// Invoked to create a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    ITypeNode CreateType(
        INode parent,
        TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel model);

    /// <summary>
    /// Invoked to create a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    IPropertyNode CreateProperty(
        ITypeNode parent,
        PropertyDeclarationSyntax syntax, IPropertySymbol symbol, SemanticModel model);

    /// <summary>
    /// Invoked to create a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    IFieldNode CreateField(
        ITypeNode parent,
        FieldDeclarationSyntax syntax, IFieldSymbol symbol, SemanticModel model);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the appropriate file name where the source code for the given captured
    /// element will be emitted.
    /// </summary>
    /// <param name="captured"></param>
    /// <returns></returns>
    string GetFileName(ICaptured captured);
}