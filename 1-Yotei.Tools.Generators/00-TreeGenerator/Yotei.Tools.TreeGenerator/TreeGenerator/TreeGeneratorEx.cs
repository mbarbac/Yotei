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
            .Select((x, token) => new TreeOptions(x.GetText(token)?.ToString(), out _))
            .Collect();

        // Registering source code emit actions...
        var combined = items.Combine(options);
        context.RegisterSourceOutput(combined, EmitNodes);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adds to the compilation the contents read from the given resource file.
    /// <br/> This method is typically used to add marker attributes whose source code is written
    /// in a resource file in the derived generator's project. Note that this addition must happen
    /// at the post initialization phase or, otherwise, their atttribute classes are not properly
    /// resolved.
    /// <para>
    /// The resource file MUST be specified in the derived generator's PROJECT FILE as an embedded
    /// resource (ie: '[EmbeddedResource Include="path.ext" /]', using angle brackets instead of
    /// squared ones used here for display purposes) whithin an 'ItemGroup' section. In addition,
    /// if the '<paramref name="outfolder"/>' value is not <see langword="null"/>, then the file
    /// will be emitted at that folder.
    /// </para>
    /// </summary>
    /// NOTES:
    /// I've found that marker attributes MUST be emitted at post-initialization time, and not at
    /// code generation one. Otherwise, their attribute class is 'ErrorType', I guess because they
    /// have not been properly resolved yet (actually, their namespace is 'global'). This makes
    /// almost impossible to match them against any type or fully qualified name that may have
    /// been specified.
    /// On the flip side, at this post-initialization moment we have no access to any resource
    /// in the consuming project, so options have not read yet. This prevents obtaining folder
    /// conventions or any other settings. So, by convention, to prevent name conflicts, we'll
    /// use the namespace of the derived generator as the prefix for the emitted files.
    /// <param name="context"></param>
    /// <param name="rname"></param>
    /// <param name="outfolder"></param>
    public void AddLocalResource(
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
    /// Normalizes the given file name by substituting the angle brackets by their corresponding
    /// squared ones, and interrogation signs by underscores.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string NormalizeFileName(string name) =>
        name.NotNullNotEmpty(trim: true)
        .Replace('<', '[')
        .Replace('>', ']')
        .Replace('?', '_');

    /// <summary>
    /// Normalizes the given file name so that it can be used to emit files in an standardized
    /// way. This method substitutes the angle brackets by their corresponding squared ones, and
    /// interrogation signs by underscores. Then, if requested, its first-level dot-separated
    /// parts (not protected by squared brackets) are reversed. Then, if requested, all parts
    /// but the last ones are used as the folder name. Finally, if the name had an extension,
    /// it is replaced by a '.g.ext' one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reverseparts"></param>
    /// <param name="usefolder"></param>
    /// <returns></returns>
    public static string NormalizeFileName(
        string name,
        bool reverseparts, bool usefolder)
    {
        name = NormalizeFileName(name);

        var parts = FirstLevelDotParts(name, normalize: false);
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

    /// <summary>
    /// Obtains a list with the first-level dot separated parts contained in the given normalized
    /// file name. Normalized file names have substituted their angle brackets by squared ones.
    /// First-level parts are those dot-separated ones where the dots are not protected by square
    /// brackets.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<string> FirstLevelDotParts(string name, bool normalize = true)
    {
        name = name.NotNullNotEmpty(trim: true);

        if (normalize) name = NormalizeFileName(name);

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
    /// Obtains the collection of attributes decorating the given syntax node, and transform them
    /// into the ones decorating the given symbol.
    /// <br/> Motivation: the <see cref="ISymbol.GetAttributes"/> method not always return all the
    /// symbol's attributes, for instance when the symbol is defined in differente places (as for
    /// instance in partial types).
    /// </summary>
    static List<AttributeData> FindSyntaxAttributes(
        MemberDeclarationSyntax syntax,
        ISymbol symbol)
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
    /// Filters the given collection of attributes by those whose attribute class match either any
    /// of the given types, or ay of the given fully qualified names
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
    /// Invoked to add (at code generation time) the source code of the <see cref="IsNullable{T}"/>
    /// and the <see cref="IsNullableAttribute"/> types, under the derived generator's namespace.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="options"></param>
    void IncludeNullabilityHelpers(SourceProductionContext context, TreeOptions options)
    {
        var name = NormalizeFileName("Yotei.Tools.NullabilityHelpers.cs",
            options.ReverseGeneratedFileNames,
            options.GenerateFilesInFolders);

        var nspace = GetType().Namespace ?? "Yotei.Tools";
        var code = $$"""
            namespace {{nspace}}
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

        context.AddSource(name, code);
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

        // Emits the nullability helpers...
        if (AddNullabilityHelpers) IncludeNullabilityHelpers(context, options);

        // Generating hierarchy and reporting captured errors...

        // Emitting source code...
    }
}