namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents the base class of a tree-oriented incremental source generator that emits code
/// organized by type hierarchy this instance has captured. To be recognized by the compiler,
/// derived classes must be decorated with the '<see cref="GeneratorAttribute"/>' attribute
/// with a '<see cref="LanguageNames.CSharp"/>' argument.
/// </summary>
internal class CoreGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked at initialization time to allow application code to register post-initialization
    /// actions, such as generating additional code, reading external files, and so on.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context) { }

    /// <summary>
    /// Invoked automatically by the compiler to initialize the generator and to register source
    /// code generation steps via callbacks on the context.
    /// <br/> This method is infrastructure only and shall not be used by application code.
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }

    // ====================================================

    /// <summary>
    /// Invoked by the compiler to quickly determine if the given syntax node shall be considered
    /// as a potential candidate for source code generation, or not.
    /// <br/> Unless overriden this method, by default, compares if any of the attributes applied
    /// to the syntax node match with any of the specified ones for that kind of syntax node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool Predicate(SyntaxNode node, CancellationToken token)
    {
        throw null;
    }

    // ====================================================

    /// <summary>
    /// Invoked by the compiler to transform the syntax node carried by the given context either
    /// into a valid candidate for source code generation purposes, or into an error one that
    /// describes why that transformation is not possible.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        throw null;
    }

    // ====================================================

    /// <summary>
    /// Invoked to emit the source code of the collection of captured candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        throw null;
    }
}