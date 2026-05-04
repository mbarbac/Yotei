namespace Yotei.Tools.Generators;

// ========================================================
partial class TreeGenerator
{
    /// <summary>
    /// INFRASTRUCTURE ONLY. Not intended for inheritors' usage.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Registering post-initialization actions...
        context.RegisterPostInitializationOutput(OnInitialize);

        // Registering syntax provider steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(TreePredicate, CaptureNode)
            .Where(static x => x is not null)
            .Collect();

        // Reading config options from consuming project...
        var options = context.AdditionalTextsProvider
            .Where(x => ConfigurationFileName != null && x.Path.EndsWith(ConfigurationFileName))
            .Select((x, token) => new TreeOptions(x.GetText(token)?.ToString()))
            .Collect();

        // Registering source code emit actions...
        var combined = items.Combine(options);
        context.RegisterSourceOutput(combined, EmitNodes);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the collection of attributes decorating the given syntax, and transform them into
    /// those decorating the associated symbol. Motivation: for whatever reasons I found that the
    /// <see cref="ISymbol.GetAttributes"/> method not always return all the symbol's attributes,
    /// for instance when the symbol is defined in differente places (ie: partial types).
    /// </summary>
    static List<AttributeData> FindSyntaxAttributes(
        ISymbol symbol,
        MemberDeclarationSyntax syntax)
    {
        var list = new List<AttributeData>();

        var atsyntaxes = syntax.AttributeLists.SelectMany(static x => x.Attributes);
        foreach (var atsyntax in atsyntaxes)
        {
            var atd = symbol.GetAttributes().FirstOrDefault(
                x => x.ApplicationSyntaxReference?.GetSyntax() == atsyntax);

            if (atd != null && !list.Contains(atd)) list.Add(atd);
        }
        return list;
    }

    /// <summary>
    /// Filters the given collection of attributes by matching them with either the given types
    /// or with the given fully qualified type names.
    /// </summary>
    static List<AttributeData> FilterAttributes(
        IEnumerable<AttributeData> attributes,
        List<Type> types,
        List<string> names)
    {
        // Matching against the given regular types...
        var list = attributes.Where(
            x => x.AttributeClass?.MatchAny([.. types]) ?? false)
            .ToList();

        // Matching against the given fully qualified type names...
        foreach (var name in names)
        {
            var items = attributes.Where(x => x.AttributeClass?.Name == name);
            foreach (var item in items)
                if (!list.Contains(item)) list.Add(item);
        }

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to report the errors that may have been captured before, and to emit the source
    /// code of the other captured nodes.
    /// </summary>
    void EmitNodes(
        SourceProductionContext gencontext,
        (ImmutableArray<INode>, ImmutableArray<TreeOptions>) source)
    {
    }
}