namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class CloneGenerator : TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        // Base method first...
        base.OnInitialize(context);

        // Marker attributes...
        AddInitializationResource(context, "Public.CloneableAttribute.cs", "Markers");
        AddInitializationResource(context, "Public.CloneableAttribute[T].cs", "Markers");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>),];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected override TypeNode CreateNode(
        INamedTypeSymbol symbol,
        BaseTypeDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new XTypeNode(symbol); // for this generator!
        item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the version of this generator for documentation purposes.
    /// </summary>
    public static string DocVersion => Assembly.GetExecutingAssembly().GetName().Version.To3String();

    /// <summary>
    /// Gets the string that emits the attribute decoration, for documentation purposes.
    /// </summary>
    public static string DocAttribute => $$"""
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(CloneGenerator)}}", "{{DocVersion}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated methods.
    /// </summary>
    /// <param name="cb"></param>
    public static void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
            /// <summary>
            /// <inheritdoc cref="ICloneable.Clone"/>
            /// </summary>
            /// {{DocAttribute}}
            """);
}