namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a incremental source generator that emits code for the relevant types in their own
/// files, each captured by itself or by its belonging elements. Derived classes must be decorated
/// with a '<see cref="GeneratorAttribute"/>' attribute to be recognized by the compiler. It is
/// recommended that the attribute includes a '<see cref="LanguageNames.CSharp"/>' argument.
/// <para>
/// By default, instances of this type filter the relevant nodes by comparing the attributes that
/// decorate them with the ones specified for that node kind, and produce candidate objects that
/// carry the information needed to build the respective hierarchies and emit code.</para>
/// </summary>
internal class CoreGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE = "Attribute";

    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked at initialization time to register register post-initialization actions, such as
    /// generating additional code, reading external files, and so on.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialized(IncrementalGeneratorPostInitializationContext context) { }

    /// <summary>
    /// The collection of attributes types used by this generator to identify type candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] TypeAttributes { get; } = [];
    string[] TypeAttributeNames = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify property candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] PropertyAttributes { get; } = [];
    string[] PropertyAttributeNames = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify field candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] FieldAttributes { get; } = [];
    string[] FieldAttributeNames = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify method candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] MethodAttributes { get; } = [];
    string[] MethodAttributeNames = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>.
    /// <para>
    /// This method is INFRASTRUCTURE ONLY, it shall not be used by application code.</para>
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a compile-time debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Register actions....
        context.RegisterPostInitializationOutput(OnInitialized);

        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Where(static x => x != null)
            .Collect();

        context.RegisterSourceOutput(items, Execute);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked by the compiler to quickly determine if the given syntax node shall be considered
    /// as a potential candidate for source code generation, or not.
    /// <para>
    /// Unless overriden, this method selects nodes by determining if any of their decorating
    /// attributes match any of those specified in this instance for that node kind.</para>
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool Predicate(SyntaxNode node, CancellationToken token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked by the compiler to transform the syntax node carried by the given context into a
    /// source code generation candidate. Returning values can also be error or null ones, which
    /// are ignored.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for the captured candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
    }
}