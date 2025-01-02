namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents the base class of tree-oriented incremental source code generators. Derived
/// classes must be decorated with the <see cref="GeneratorAttribute"/> attribute, with a
/// <see cref="LanguageNames.CSharp"/> argument, to be invoked by the compiler.
/// </summary>
internal class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if this instance tries to launch a debug session when it is invoked by the
    /// compiler, or not.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked after initialization to register constant post-initialization actions, such as
    /// generating aditional code, or reading extenal files.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialized(IncrementalGeneratorPostInitializationContext context) { }

    // ----------------------------------------------------

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
    /// Invoked automatically by the compiler to initialize the generator and to register source
    /// code generation steps via callbacks on the context. Although this method is public, it is
    /// just infrastructure and shall not be called from user code.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Register post-initialization constant actions....
        context.RegisterPostInitializationOutput(OnInitialized);

        // Capturing attribute names...
        TypeAttributeNames = CaptureNames(TypeAttributes);
        PropertyAttributeNames = CaptureNames(PropertyAttributes);
        FieldAttributeNames = CaptureNames(FieldAttributes);
        MethodAttributeNames = CaptureNames(MethodAttributes);

        // Registering actions...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Where(x => x != null)
            .Collect();

        // Registering source code emission...
        context.RegisterSourceOutput(items, Execute);

        /// <summary>
        /// Invoked to capture the type names of the attributes specified in this instance.
        /// We assume each type derives from the <see cref="Attribute"/> class.
        /// </summary>
        static string[] CaptureNames(Type[] types)
        {
            var attribute = "Attribute";
            var array = new string[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var name = type.Name;

                if (!name.Contains(attribute))
                {
                    var index = name.IndexOf('`');
                    if (index > 0)
                    {
                        var gens = name[index..];
                        name = name.Replace(gens, "");
                        name += attribute;
                        name += gens;
                    }
                    else
                    {
                        name += attribute;
                    }
                }

                array[i] = name;
            }

            return array;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// candidate for source code generation or not. By default, this method just compares if any
    /// of the attributes applied to the given node match with any of the specified ones, for the
    /// kind of that node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual bool Predicate(SyntaxNode node, CancellationToken token)
    {
        throw null;

        /// <summary>
        /// Determines if the given syntax node has at least one attribute that matches with any
        /// of the given ones, using a quick comparison among the names of those collection of
        /// attributes.
        /// </summary>
        static bool Match(MemberDeclarationSyntax syntax, string[] types)
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token) => throw null;

    // ----------------------------------------------------

    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates) => throw null;
}