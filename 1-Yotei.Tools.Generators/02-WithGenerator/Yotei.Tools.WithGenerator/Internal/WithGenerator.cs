namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class WithGenerator : TreeGenerator
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
        AddInitializationResource(context, "Public/WithAttribute.cs", "Markers");
        AddInitializationResource(context, "Public/InheritsWithAttribute.cs", "Markers");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [typeof(InheritsWithAttribute)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> PropertyAttributes { get; } = [typeof(WithAttribute)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> FieldAttributes { get; } = [typeof(WithAttribute)];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override PropertyNode CreateNode(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new XPropertyNode(symbol); // for this generator!
        item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override FieldNode CreateNode(
        IFieldSymbol symbol,
        BaseFieldDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new XFieldNode(symbol); // for this generator!
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
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{DocVersion}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated methods.
    /// </summary>
    /// <param name="cb"></param>
    public static void EmitDocumentation(CodeBuilder cb, string name) => cb.AppendLine($$"""
            /// <summary>
            /// Emulates the '<see langword="with"/>' keyword for the '{{name}}' member.
            /// </summary>
            {{DocAttribute}}
            """);
}