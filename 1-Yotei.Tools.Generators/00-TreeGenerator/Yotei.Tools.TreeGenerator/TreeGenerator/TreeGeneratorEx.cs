namespace Yotei.Tools.Generators;

// ========================================================
partial class TreeGenerator
{
    /// <summary>
    /// Gets the file name of the consuming project configuration file that will be used by this
    /// generator. If <see langword="null"/>, then no options are read.
    /// </summary>
    protected virtual string ConfigurationFileName { get; } = "TreeGeneratorOptions.ini";

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

    /* NOTE: I've found that marker attributes MUST be emitted at post-initialization time. If
     * not, their attribute's class is a 'ErrorType' one, probably because they were not properly
     * resolved - actually, their namespace become the 'global' one. This makes almost impossible
     * to properly match them against any given regular type or fully qualified name.
     * The flip side is that we have not yet read any configuration file. So, we cannot use any
     * setting. Therefore, by convention, we will at least use the generator's namespace as the
     * prefix for the generated files, to avoid name conflicts and the like.
     */

    /// <summary>
    /// Adds into the compilation the contents read from a file in the generator's project, at
    /// post-initialization time. This method is typically used to emit marker attributes defined
    /// by the generator project.
    /// <br/> The resource file must be specified in the generator csproj as an embedded resource,
    /// within an 'ItemGroup' section (ie: '[EmbeddedResource Include="path.ext" /]', using angle
    /// brackets instead of the squared ones).
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

    /// <summary>
    /// Normalizes the given file name by, firstly, replacing its angle bracket characters with
    /// their corresponding squared ones, and then reversing (if requested) its first-level dot
    /// separated parts (those whose dots are not protected by squared brackets), and then (if
    /// requested) using all parts but the last one as the folder name. In addition, if it has
    /// an extension, it is replaced by a '.g.ext' chain.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="useFolder"></param>
    /// <param name="reverseName"></param>
    /// <returns></returns>
    public static string NormalizeFileName(
        string name,
        bool useFolder, bool reverseName)
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
        if (useFolder && parts.Count > 1)
        {
            var part = parts[^1]; parts.RemoveAt(parts.Count - 1);
            if (reverseName) parts.Reverse();
            name = string.Join(".", parts);
            name += $"/{part}";
            if (ext != null) name += ext;
            return name;
        }

        // Default is flat names...
        if (reverseName) parts.Reverse();
        name = string.Join(".", parts);
        if (ext != null) name += ext;
        return name;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the list of the first-level dot-separated parts, meaning those whose dots are
    /// not protected by squared brackets.
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to both generate the source code of the captured nodes, and to report the error
    /// conditions the error nodes may carry with them.
    /// </summary>
    /// NOTE: This method is INFRASTRUCTURE not intended for application usage. Reason is that
    /// for technical reasons it gets an array of TreeOptions, when only the first one (or none)
    /// is acceptable.
    void EmitNodes(
        SourceProductionContext gencontext,
        (ImmutableArray<INode>, ImmutableArray<TreeOptions>) source)
    {
        var nodes = source.Item1;
        var options = source.Item2.Length > 0 ? source.Item2[0] : new TreeOptions();
        var usefolders = options.GenerateFilesInFolders;
        var reversenames = options.ReverseGeneratedFileNames;

        // Nullability helpers...
        if (options.EmitNullabilityHelpers)
        {
            var name = NormalizeFileName(
                "Yotei.Tools.NullabilityHelpers.cs", usefolders, reversenames);

            var code = $$"""
                namespace {{GetType().Namespace!}}
                {
                    /// <summary>
                    /// Used to wrap types for which nullability information shall be persisted.
                    /// <para>
                    /// Nullable annotations on value types are always translated by the compiler into instances
                    /// of the <see cref="Nullable{T}"/> struct. By contrast, nullable annotations on reference
                    /// types are just syntactic sugar used by the compiler but, in general, either they are not
                    /// persisted in metadata or in custom attributes, or they are not allowed in certain contexts
                    /// (e.g., generic type arguments).
                    /// <br/> The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be
                    /// used as workarounds when there is the need to persist nullability information for their
                    /// associated types, or when there is the need to specify it in those not-allowed contexts.
                    /// </para>
                    /// </summary>
                    [Microsoft.CodeAnalysis.Embedded]
                    public class IsNullable<T> { }
                    
                    /// <summary>
                    /// <inheritdoc cref="IsNullable{T}"/>
                    /// </summary>
                    [Microsoft.CodeAnalysis.Embedded]
                    [AttributeUsage(AttributeTargets.All)]
                    public class IsNullableAttribute : Attribute { }
                }
                """;

            gencontext.AddSource(name, code);
        }

        // Processing nodes...
        var context = new TreeContext(gencontext, options);
        var files = new List<TypeNode>();

        foreach (var node in nodes)
        {
            gencontext.CancellationToken.ThrowIfCancellationRequested();

            if (node is ErrorNode error) error.Report(gencontext);
            else CaptureHierarchy(files, node, ref context);
        }

        // Generating source code...
        foreach (var file in files)
        {
            gencontext.CancellationToken.ThrowIfCancellationRequested();

            var cb = new CodeBuilder();
            var ok = file.Emit(ref context, cb);
            if (ok)
            {
                var fname = file.FileName + ".cs";
                var name = NormalizeFileName(fname, usefolders, reversenames);
                var code = cb.ToString();
                gencontext.AddSource(name, code);
            }
        }
    }
}