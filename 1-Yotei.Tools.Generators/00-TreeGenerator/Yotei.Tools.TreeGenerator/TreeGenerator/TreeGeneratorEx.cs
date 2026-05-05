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

    /* NOTE:
     * I've found that marker attributes MUST be emitted at post-initialization time. Otherwise,
     * their attribute' class is an 'ErrorType' one (probably because its has not been properly
     * resolved yet - actually, their namespace is the 'global' one). This makes almost impossible
     * to properly match them against any regular type or fully qualified type name that may have
     * been identified as an attribute in the generator.
     * On the flip side, at that moment cannot access to any resource in the consuming generator
     * project, as for instance its options. Therefore, by convention, to prevent name conflicts,
     * we'll use the generator's namespace as the prefix for the generated files.
     */

    /// <summary>
    /// Adds into the compilation the contents read from the given resource file. This method is
    /// typically used to add into the compilation marker attributes whose source code is in a
    /// given resource file in the derived generator's project. This addition must happen at the
    /// post initialization phase so that their classes are properly resolved before even the
    /// filtering phase.
    /// <para>
    /// The resource file MUST be specified in the derived generator's csproj project file as an
    /// embedded resource (ie: '[EmbeddedResource Include="path.ext" /]') within an 'ItemGroup'
    /// section (using angle brackets instead of the squared ones in the example). In addition,
    /// if the given '<paramref name="outfolder"/>' value is not <see langword="null"/>, then the
    /// out file will be emitted at that folder.
    /// </para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="rname"></param>
    /// <param name="outfolder"></param>
    protected void AddLocalResource(
        IncrementalGeneratorPostInitializationContext context,
        string rname,
        string? outfolder = null)
    {
        if (outfolder is not null && outfolder.EndsWith('/')) outfolder = outfolder[..^1];
        outfolder = outfolder.NullWhenEmpty(trim: true);

        rname = rname.NotNullNotEmpty(trim: true);
        rname = rname.Replace('\\', '.').Replace('/', '.');

        // Reading the resource file...
        var type = GetType();
        var asm = type.Assembly;
        var xname = $"{type.Namespace}.{rname}";
        using var stream = asm.GetManifestResourceStream(xname);
        using var reader = new StreamReader(stream);
        var code = reader.ReadToEnd();

        // Preparing output...
        rname = rname.Replace('<', '[').Replace('>', ']');
        var parts = FirstLevelDotParts(rname);

        // We may have an extension...
        var ext = parts.Count > 1 ? parts[^1] : null;
        if (ext != null)
        {
            ext = $".g.{ext}";
            parts.RemoveAt(parts.Count - 1);
        }

        // By convention, we just need the last part with the namespace prefix...
        rname = $"{type.Namespace}.{parts[^1]}";
        if (outfolder != null) rname = $"{outfolder}/{rname}";
        if (ext != null) rname += ext;

        // Adding...
        context.AddSource(rname, code);
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
    /// Obtains a list with the first-level dot parts contained in the given name. These parts
    /// are the dot-separated ones, provided the dots are not protected by square brakets.
    /// <br/> This condition implies the name must have been curated so that any angle brackets
    /// have been replaced by their squared ones counterparts.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected static List<string> FirstLevelDotParts(string name)
    {
        var dots = FirstLevelDots(name);
        var list = new List<string>();
        var last = 0;

        foreach (var dot in dots)
        {
            list.Add(name[last..dot]);
            last = dot + 1;
        }
        list.Add(name[last..]);
        return list;

        // Obtains the list of first-level dots.
        static List<int> FirstLevelDots(string name)
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
    }

    /// <summary>
    /// Invoked to normalize the given file name. Firstly, this method replaces any angle brackets
    /// by their squared ones counterparts. Then its reverses the first-level dot-separated parts,
    /// if such is requested. Finally, if requested, uses all parts but the last one as the folder
    /// name. In addition, if the file name had an extension, it is replaced by a '.g.ext' one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="usefolder"></param>
    /// <param name="reverseparts"></param>
    /// <returns></returns>
    public static string NormalizeFileName(
        string name,
        bool usefolder, bool reverseparts)
    {
        name = name.NotNullNotEmpty(trim: true);
        name = name.Replace('<', '[').Replace('>', ']');
        name = name.Replace('?', '_');

        var parts = FirstLevelDotParts(name);

        var ext = parts.Count > 1 ? parts[^1] : null;
        if (ext != null)
        {
            ext = $".g.{ext}";
            parts.RemoveAt(parts.Count - 1);
        }

        // Use folders...
        if (usefolder && parts.Count > 1)
        {
            var part = parts[^1]; parts.RemoveAt(parts.Count - 1);
            if (reverseparts) parts.Reverse();
            name = string.Join(".", parts);
            name += $"/{part}";
            if (ext != null) name += ext;
            return name;
        }

        // Default is flat names...
        if (reverseparts) parts.Reverse();
        name = string.Join(".", parts);
        if (ext != null) name += ext;
        return name;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to report the errors that may have been captured before, and to emit the source
    /// code of the other captured nodes.
    /// </summary>
    void EmitNodes(
        SourceProductionContext context,
        (ImmutableArray<INode>, ImmutableArray<TreeOptions>) source)
    {
        var nodes = source.Item1;
        var options = source.Item2.Length > 0 ? source.Item2[0] : new TreeOptions();

        // Unless overriden, this emits the nullability helpers...
        AddNullabilityHelpers(context, options);

        // Generating hierarchy and reporting captured errors...

        // Emitting source code...
    }
}