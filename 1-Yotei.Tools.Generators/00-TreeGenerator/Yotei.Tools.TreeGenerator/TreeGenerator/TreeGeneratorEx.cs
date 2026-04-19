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

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(TreePredicate, CaptureNode)
            .Where(static x => x is not null)
            .Collect();

        // Registering source code emit actions...
        context.RegisterSourceOutput(items, EmitNodes);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adds into the compilation the contents read from an file in the generator's project, at
    /// post-initialization time.
    /// <br/> The resource file must be specified in the generator csproj as an embedded resource,
    /// within an 'ItemGroup' section (ie: '[EmbeddedResource Include="path.ext" /]', using angle
    /// brackets instead of the squared ones). If the file path includes a folder in the generator
    /// project, use a dot separator.
    /// <br/> If the <paramref name="outfolder"/> is not <see langword="null"/>, then the out file
    /// will be emitted at that folder (do not end with '/', and use embedded dots if needed).
    /// Otherwise, at the root analyzer output one. In addition, the resource file name extension
    /// will be replaced by a '.g.ext' one.
    /// </summary>
    /// <param name="outFolder"></param>
    /// <param name="rname"></param>
    protected void AddInitializationResource(
        IncrementalGeneratorPostInitializationContext context,
        string rname,
        string? outfolder = null)
    {
        // Preparing...
        outfolder = outfolder.NullWhenEmpty(trim: true);
        rname = rname.NotNullNotEmpty(trim: true);

        // Reading the resource file...
        var type = GetType();
        var asm = type.Assembly;
        var xname = $"{type.Namespace}.{rname}";
        using var stream = asm.GetManifestResourceStream(xname);
        using var reader = new StreamReader(stream);
        var code = reader.ReadToEnd();

        // Preparing output...
        rname = rname.Replace('<', '[').Replace('>', ']');
        var dots = FirstLevelDots(rname);

        // At least one dot, we have an extension...
        var ext = dots.Count > 0 ? rname[dots[^1]..] : null;
        if (ext != null)
        {
            ext = $".g.cs";
            rname = rname[..dots[^1]];
        }

        // We don't need anything before the resource name itself...
        dots = FirstLevelDots(rname);
        if (dots.Count > 0) rname = rname[(dots[^1] + 1)..];

        // Preparing out name...
        rname = $"{type.Namespace}.{rname}";
        if (outfolder != null) rname = $"{outfolder}/{rname}";
        if (ext != null) rname += ext;

        // Adding...
        context.AddSource(rname, code);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the list of first-level dot separators, meaning those not protected by squared
    /// brackets, or an empty list if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected static List<int> FirstLevelDots(string name)
    {
        var list = new List<int>();
        var depth = 0;

        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] == '[') { depth++; continue; }
            if (name[i] == ']') { if (depth > 0) depth--; continue; }
            if (name[i] == '.' && depth == 0) list.Add(i);
        }
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the collection of attributes decorating the given syntax, and transform them into
    /// those decorating the associated symbol. Motivation: for whatever reasons I found that the
    /// <see cref="ISymbol.GetAttributes"/> method not always return all the symbol's attributes,
    /// for instance when the symbol is defined in differente places (ie: partial types).
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <returns></returns>
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
    /// <param name="attributes"></param>
    /// <param name="types"></param>
    /// <param name="names"></param>
    /// <returns></returns>
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
}