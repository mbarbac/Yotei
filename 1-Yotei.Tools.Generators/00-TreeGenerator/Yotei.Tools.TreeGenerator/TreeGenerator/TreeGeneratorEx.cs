namespace Yotei.Tools.Generators;

/* NOTE:
 * As per my understanding of the documentation, if we capture the 'Compilation' object then
 * we will loose the 'incremental' nature of the generator - so that the slightest change would
 * drive a full source code generation over and over again. What I cannot understand is why, then,
 * the following is possible (provided an appropriate signature for the EmitNodes method):
 *      var combined = context.CompilationProvider.Combine(items);
 *      context.RegisterSourceOutput(combined, EmitNodes);
 */

// ========================================================
partial class TreeGenerator
{
    /// <summary>
    /// <inheritdoc/> This method is INFRASTRUCTURE only, and it is only intended to be invoked
    /// by the compiler. Application code shall not invoke it.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Registering post-initialization actions...
        context.RegisterPostInitializationOutput(OnInitialize);

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(FastPredicate, CaptureNode)
            .Where(static x => x is not null)
            .Collect();

        var options = context.AnalyzerConfigOptionsProvider.Select((opts, _) =>
        {
            var item = CreateTreeOptions();
            item.ReadElements(opts.GlobalOptions);
            return item;
        });

        // Registering source code emit actions...
        var combined = items.Combine(options);
        context.RegisterSourceOutput(combined, EmitNodes);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Normalizes the given file name by replacing angle brackets for their square counterparts,
    /// reversing (if requested) their first-level dot parts (not protected by brackets), and then
    /// (if requested) using all parts but the last one as the folder name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="usefolder"></param>
    /// <param name="reverse"></param>
    /// <param name="removeExtension"></param>
    /// <param name="addExtension"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string NormalizeFileName(
        string name,
        bool usefolder, bool reverse,
        string? removeExtension = "cs", string? addExtension = "g.cs",
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        name = name.NotNullNotEmpty(trim: true);
        name = name.Replace('<', '[').Replace('>', ']');

        if ((removeExtension = removeExtension?.NullWhenEmpty(trim: true)) != null)
        {
            if (!removeExtension.StartsWith('.')) removeExtension = $".{removeExtension}";
            if (name.EndsWith(removeExtension, comparison))
                name = name.RemoveLast(removeExtension, comparison).ToString();
        }

        var parts = FirstDotLevelParts(name);
        var part = usefolder ? parts[^1] : null;
        if (usefolder) parts.RemoveAt(parts.Count - 1);
        if (reverse) parts.Reverse();

        if (parts.Count == 1) name = parts[0];
        else if (usefolder)
        {
            name = string.Join(".", parts);
            name += '/';
            name += part;
        }
        else name = string.Join(".", parts);

        if ((addExtension = addExtension?.NullWhenEmpty(trim: true)) != null)
        {
            if (!addExtension.StartsWith('.')) name += '.';
            name += addExtension;
        }

        return name;
    }

    /// <summary>
    /// Obtains the list of first-level dot separarated parts.
    /// </summary>
    static List<string> FirstDotLevelParts(string name)
    {
        List<int> dots = [];
        int depth = 0;
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] == '[') { depth++; continue; }
            if (name[i] == ']') { if (depth > 0) depth--; continue; }
            if (name[i] == '.' && depth == 0) dots.Add(i);
        }

        List<string> parts = [];
        int last = 0;
        foreach (var dot in dots)
        {
            parts.Add(name[last..dot]);
            last = dot + 1;
        }
        parts.Add(name[last..]);
        return parts;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the contents of a source file (for instance: the source code of a market attribute)
    /// embedded as a resource in the generator's assembly resources.
    /// <br/>- The combination of the resource folder and name must be the one specified in the
    /// generator project file (typically in an '[EmbeddedResource Include="name.cs" /]' entry of
    /// an ItemGroup section, with angle brackets).
    /// <br/>- In the project file, the name format is like 'rfolder/rname'. When passed to this
    /// method, the resource folder shall be null or not ending with '/'.
    /// </summary>
    /// <param name="rname"></param>
    /// <returns></returns>
    protected string ReadResource(string? rfolder, string rname)
    {
        rfolder = rfolder.NullWhenEmpty(trim: true);
        rname = rname.NotNullNotEmpty(trim: true);

        // For whatever reasons 'folder\name' must be specified as 'folder.name'...
        if (rfolder != null) rname = $"{rfolder}.{rname}";
        rname = rname.Replace('\\', '.').Replace('/', '.');

        var nspace = GetType().Namespace;
        var path = $"{nspace}.{rname}";
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(path)
            ?? throw new NotFoundException($"Embedded file not found: {rname}");

        using var reader = new StreamReader(stream);
        var source = reader.ReadToEnd();
        return source;
    }

    /// <summary>
    /// Invoked to read a source file embedded as a resource in the generator's project file.
    /// <br/>- The combination of the resource folder and name must be the one specified in the
    /// generator project file (typically in an '[EmbeddedResource Include="name.cs" /]' entry of
    /// an ItemGroup section, with angle brackets).
    /// <br/>- In the project file, the name format is like 'rfolder/rname'. When passed to this
    /// method, the resource folder shall be null or not ending with '/'.
    /// <br/>- The namespace argument can be null but, if not, is added as a prefix to the given
    /// resource name. Then the values of the <paramref name="useFolders"/> and the
    /// <paramref name="reverseFileName"/> arguments are used to build the actual file name where
    /// the code will be emitted.
    /// </summary>
    /// <param name="rfolder"></param>
    /// <param name="rname"></param>
    /// <param name="nspace"></param>
    /// <param name="context"></param>
    protected void ReadAndEmitResource(
        string? rfolder, string rname, string? nspace,
        bool useFolders, bool reverseFileName,
        SourceProductionContext context)
    {
        var code = ReadResource(rfolder, rname);
        var name = nspace == null ? rname : $"{nspace}.{rname}";

        name = NormalizeFileName(name, useFolders, reverseFileName);
        context.AddSource(name, code);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the collection of attributes that decorate the given syntax. Motivation: for
    /// whatever reasons <see cref="ISymbol.GetAttributes"/> does not return all the symbol's
    /// attributes when it is defined in different places (ie: partial types). So we need a
    /// way of capturing all of them from the syntax locations.
    /// </summary>
    /*static IEnumerable<AttributeData> FindSyntaxAttributes(
        ISymbol symbol,
        MemberDeclarationSyntax syntax)
    {
        var atsyntaxes = syntax.AttributeLists.SelectMany(static x => x.Attributes);
        foreach (var atsyntax in atsyntaxes)
        {
            var atd = symbol.GetAttributes().FirstOrDefault(
                x => x.ApplicationSyntaxReference?.GetSyntax() == atsyntax);

            if (atd is not null) yield return atd;
        }
    }*/
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

            if (atd is not null && !list.Contains(atd)) list.Add(atd);
        }
        return list;
    }

    /// <summary>
    /// Filters the given collection of attributes to return a new one containing only those
    /// that match any of the given types, and those whose full class names match any of the
    /// given ones.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="types"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    static List<AttributeData> FilterAttributes(
        IEnumerable<AttributeData> attributes,
        IEnumerable<Type> types,
        IEnumerable<string> names)
    {
        List<AttributeData> items = [];

        foreach (var at in attributes)
        {
            if (at.AttributeClass is null) continue;
            
            var found = at.AttributeClass.MatchAny(types);
            if (!found) found = names.Any(x => x == at.AttributeClass.Name);

            if (found && !items.Contains(at)) items.Add(at);
        }

        return items;
    }
    /*{
        var items = attributes.Where(x =>
            x.AttributeClass != null &&
            x.AttributeClass.MatchAny(types)).ToList();

        foreach (var name in names)
        {
            var temps = attributes.Where(x => x.AttributeClass?.Name == name);
            foreach (var temp in temps) if (!items.Contains(temp)) items.Add(temp);
        }

        return items;
    }*/
}