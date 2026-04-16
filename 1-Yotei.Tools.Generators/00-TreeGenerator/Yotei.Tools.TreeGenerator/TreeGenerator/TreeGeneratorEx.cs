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
        context.RegisterPostInitializationOutput(PrepareOnInitialize);

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(FastPredicate, CaptureNode)
            .Where(static x => x is not null)
            .Collect();

        // Registering source code emit actions...
        context.RegisterSourceOutput(items, EmitNodes);
    }

    /// <summary>
    /// Invoked to prepare post-initialization actions and elements.
    /// </summary>
    /// <param name="context"></param>
    void PrepareOnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        // TODO...
        OnInitialize(context);
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
    /// Reads the contents of a source file embedded in the generator's assembly resources, as
    /// for instance the source code of marker attributes. The resource name must be the one
    /// in the generator project file (typically an '[EmbeddedResource Include="name.cs" /]' entry
    /// of an ItemGroup section, with angle brackets)
    /// </summary>
    /// <param name="rname"></param>
    /// <returns></returns>
    public string ReadResource(string rname)
    {
        // For whatever reasons 'folder\name' must be specified as 'folder.name'...
        rname = rname.NotNullNotEmpty(trim: true);
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
}