namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a tree-oriented incremental source generator. Classes that implement this type
/// must also implement the <see cref="IIncrementalGenerator"/> interface, and must be decorated
/// with the <see cref="GeneratorAttribute"/> attribute.
/// </summary>
public interface IGenerator
{
    /// <summary>
    /// Invoked to initialize the generator and register generation steps via callback on the
    /// context.
    /// </summary>
    /// <param name="context"></param>
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
    TreeLevel TreeLevel { get; }

    /// <summary>
    /// Invoked to create an appropriate object for this instance.
    /// </summary>
    /// <param name="semanticModel"></param>
    /// <param name="generator"></param>
    /// <param name="typeSyntax"></param>
    /// <param name="typeSymbol"></param>
    /// <returns></returns>
    ICapturedType CreateCapturedType(
        SemanticModel semanticModel, IGenerator generator,
        TypeDeclarationSyntax typeSyntax, INamedTypeSymbol typeSymbol);

    /// <summary>
    /// Invoked to create an appropriate object for this instance.
    /// </summary>
    /// <param name="capturedType"></param>
    /// <param name="propSyntax"></param>
    /// <param name="propSymbol"></param>
    /// <returns></returns>
    ICapturedProperty CreateCapturedProperty(
        ICapturedType capturedType,
        PropertyDeclarationSyntax propSyntax, IPropertySymbol propSymbol);

    /// <summary>
    /// Invoked to create an appropriate object for this instance.
    /// </summary>
    /// <param name="capturedType"></param>
    /// <param name="fieldSyntax"></param>
    /// <param name="fieldSymbol"></param>
    /// <returns></returns>
    ICapturedField CreateCapturedField(
        ICapturedType capturedType,
        FieldDeclarationSyntax fieldSyntax, IFieldSymbol fieldSymbol);

    /// <summary>
    /// Invoked to obtain an unique file name for this generator for the given combination of
    /// namespace and type declaration syntaxes.
    /// </summary>
    /// <param name="nsSyntaxChain"></param>
    /// <param name="tpSyntaxChain"></param>
    /// <returns></returns>
    string FileName(
        ImmutableArray<BaseNamespaceDeclarationSyntax> nsSyntaxChain,
        ImmutableArray<TypeDeclarationSyntax> tpSyntaxChain);
}