namespace Yotei.Tools.Generators;

/* To DEBUG:
 * - Install the .NET Compiler SDK (in addition to Roslyn components).
 * - Make sure the derived generator project is a Roslyn component:
 *   <IsRoslynComponent>true</IsRoslynComponent>
 * - In the derived generator project's properties, add a Roslyn debug profile, whose target is the
 *   project that when compiled will be debugged (ie: a test project).
 * - Mark the derived generator project as the startup one.
 * - In the play button, select the debug profile.
 * - Click F5 (run) to compile (F6 does nothing).
 */

// ========================================================
/// <summary>
/// Represents a tree-oriented incremental source generator that, when capturing its relevant nodes
/// organizes them by the types they refer to (which either are the types by themselves, or the
/// declaring ones for its childs). Each node is then emitted in its own file.
/// <para>
/// Derived types need to be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. In is also expected that <see cref="LanguageNames.CSharp"/> is used
/// as its argument.
/// </para>
/// </summary>
internal partial class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if the files emitted by this source generator are all them placed in the same
    /// root folder, rather than in the folder hierarchy each specifies (which is usefull, for
    /// instance, for debug purposes).
    /// </summary>
    protected virtual bool EmitFilesInFolders => false;

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// types are emitted in the namespace of the derived generator.
    /// </summary>
    protected virtual bool EmitNullabilityHelpers => true;

    /// <summary>
    /// Invoked to register post-initialization actions such as reading external files, generating
    /// code for marker attributes, and so on. Marker attribute classes can be decorated with the
    /// '[Microsoft.CodeAnalysis.Embedded]' attribute, as it is automatically added to the emitted
    /// source code.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context) { }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types used to identify decorated type-alike elements.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated property-alike elements.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated field-alike elements.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated method-alike elements.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated type
    /// -alike elements.
    /// </summary>
    protected virtual List<string> TypeAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated property
    /// -alike elements.
    /// </summary>
    protected virtual List<string> PropertyAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated field
    /// -alike elements.
    /// </summary>
    protected virtual List<string> FieldAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated method
    /// -alike elements.
    /// </summary>
    protected virtual List<string> MethodAttributeNames { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// candidate for source code generation, or not. By default, this base method validates that
    /// the node is among the recognized ones, and that the generator has specified attributes
    /// for its kind.
    /// <br/> Inheritors may override this method to, for instance, support other syntax node
    /// kinds. The ones supported by default are: <see cref="BaseTypeDeclarationSyntax"/>, 
    /// <see cref="BasePropertyDeclarationSyntax"/>, <see cref="BaseFieldDeclarationSyntax"/>,
    /// and <see cref="BaseMethodDeclarationSyntax"/>. Event-alike ones are covered by either
    /// the property or field ones
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0078")]
    protected virtual bool FastPredicate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node switch
        {
            BaseTypeDeclarationSyntax => TypeAttributes.Count > 0 || TypeAttributeNames.Count > 0,
            BasePropertyDeclarationSyntax => PropertyAttributes.Count > 0 || PropertyAttributes.Count > 0,
            BaseFieldDeclarationSyntax => FieldAttributes.Count > 0 || FieldAttributes.Count > 0,
            BaseMethodDeclarationSyntax => MethodAttributes.Count > 0 || MethodAttributes.Count > 0,

            _ => false
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate type. Inheritors
    /// may override this method to cache the specific information that may later need.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected virtual TypeCandidate CreateCandidate(
        INamedTypeSymbol symbol,
        BaseTypeDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel mode)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate type. Inheritors
    /// may override this method to cache the specific information that may later need.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected virtual PropertyCandidate CreateCandidate(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel mode)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate type. Inheritors
    /// may override this method to cache the specific information that may later need.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected virtual FieldCandidate CreateCandidate(
        IFieldSymbol symbol,
        BaseFieldDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel mode)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate type. Inheritors
    /// may override this method to cache the specific information that may later need.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected virtual MethodCandidate CreateCandidate(
        IMethodSymbol symbol,
        BaseMethodDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel mode)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture and return the given syntax node as a souce code generation candidate,
    /// or to return null to ignore it completely. In addition, error instances can be returned to
    /// report a diagnostic at source code generation time.
    /// <br/> Inheritors may override the associated 'CrateCandidate' methods, or override this
    /// one to, for instance, support other syntax node kinds.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual ICandidate CaptureCandidate(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Type-alike syntax nodes...
        while (node is BaseTypeDeclarationSyntax syntax && (
            syntax is TypeDeclarationSyntax ||
            syntax is EnumDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol == null) break;

            var atx = FindSyntaxAttributes(symbol, syntax).ToDebugArray();
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, syntax, ats, model);
            return candidate;
        }

        // Property-alike syntax nodes...
        while (node is BasePropertyDeclarationSyntax syntax && (
            syntax is PropertyDeclarationSyntax ||
            syntax is IndexerDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token) as IPropertySymbol;
            if (symbol == null) break;

            var atx = FindSyntaxAttributes(symbol, syntax).ToDebugArray();
            var ats = FilterAttributes(atx, PropertyAttributes, PropertyAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, syntax, ats, model);
            return candidate;
        }

        // Field-alike syntax nodes...
        while (node is BaseFieldDeclarationSyntax syntax)
        {
            var items = syntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol == null) continue;

                var atx = FindSyntaxAttributes(symbol, syntax).ToDebugArray();
                var ats = FilterAttributes(atx, FieldAttributes, FieldAttributeNames);
                if (ats.Count == 0) continue;

                var candidate = CreateCandidate(symbol, syntax, ats, model);
                return candidate;
            }
            break;
        }

        // Method-alike syntax nodes...
        while (node is BaseMethodDeclarationSyntax syntax && (
            syntax is MethodDeclarationSyntax ||
            syntax is ConstructorDeclarationSyntax ||
            syntax is DestructorDeclarationSyntax ||
            syntax is OperatorDeclarationSyntax ||
            syntax is ConversionOperatorDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol == null) break;

            var atx = FindSyntaxAttributes(symbol, syntax).ToDebugArray();
            var ats = FilterAttributes(atx, MethodAttributes, MethodAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, syntax, ats, model);
            return candidate;
        }

        // Finishing by ignoring the syntax node...
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to process the captured source code generation candidates.
    /// <br/> This method, by default, organizes the candidates in a hierarchy based on the types
    /// each one will logically belong to, and where each type will be emitted in its own file.
    /// <br/> Inheritors may override it to take full control on how and what their associated
    /// source code is emitted.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    protected virtual void EmitCandidates(
        SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        throw null;
    }
}