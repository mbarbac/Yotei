namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents the base class of tree-oriented incremental source generators. Derived ones must
/// be decorated with the <see cref="GeneratorAttribute"/> attribute to be used by the compiler,
/// using '(<see cref="LanguageNames.CSharp"/>)' as its argument.
/// </summary>
internal abstract class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        context.RegisterPostInitializationOutput(OnInitialized);
    }

    /// <summary>
    /// Determines if this instance will try to launch a debug session when involked by the
    /// compiler.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked to register post-initialization actions, such as generating code for custom
    /// attributes or reading external files.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to determine if the given node shall be considered a valid one for source code
    /// generation purposes, if the given name of any of its attributes match with any of the
    /// names defined for its node type.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    bool Validate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            if (TypeAttributes.Length == 0) return false;

            var atts = typeSyntax.AttributeLists.GetAttributes();
            return atts.Any(x => Compare(x.Name, TypeAttributes));
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            if (PropertyAttributes.Length == 0) return false;

            var atts = propertySyntax.AttributeLists.GetAttributes();
            return atts.Any(x => Compare(x.Name, PropertyAttributes));
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            if (FieldAttributes.Length == 0) return false;

            var atts = fieldSyntax.AttributeLists.GetAttributes();
            return atts.Any(x => Compare(x.Name, FieldAttributes));
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            if (MethodAttributes.Length == 0) return false;

            var atts = methodSyntax.AttributeLists.GetAttributes();
            return atts.Any(x => Compare(x.Name, MethodAttributes));
        }

        // Not supported node type...
        return false;
    }

    /// <summary>
    /// Determines if any of the attributes applied to the given syntax node match any of the
    /// target attribute names.
    /// </summary>
    static bool Compare(NameSyntax syntax, string[] targets)
    {
        var name = syntax.ShortName();

        for (int i = 0; i < targets.Length; i++) if (name == targets[i]) return true;
        return false;
    }

    /// <summary>
    /// The collection of attribute names that, if decorate a given type, identifies it as a
    /// potential candidate for source code generation.
    /// </summary>
    public virtual string[] TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorate a given property, identifies it as a
    /// potential candidate for source code generation.
    /// </summary>
    public virtual string[] PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorate a given field, identifies it as a
    /// potential candidate for source code generation.
    /// </summary>
    public virtual string[] FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorate a given method, identifies it as a
    /// potential candidate for source code generation.
    /// </summary>
    public virtual string[] MethodAttributes { get; } = [];
}