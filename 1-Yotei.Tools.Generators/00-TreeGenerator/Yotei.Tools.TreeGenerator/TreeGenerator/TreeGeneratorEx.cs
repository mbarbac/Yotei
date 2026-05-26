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

        // Reading config options from consuming project...
        var options = context.AdditionalTextsProvider
            .Where(x => ConfigurationFileName != null && x.Path.EndsWith(ConfigurationFileName))
            .Select((x, token) => new TreeOptions(x.GetText(token)?.ToString(), out _))
            .Collect();

        // Registering syntax provider steps..
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(TreePredicate, CaptureNode)
            .Where(static x => x is not null)
            .Collect();

        // Comnining and emitting...
        var combined = items.Combine(options);
        context.RegisterSourceOutput(combined, EmitNodes);
    }

    /// <summary>
    /// Invoked to process the captures nodes by either reporting the ones representing captured
    /// errors, or by emitting the source code of the other ones.
    /// </summary>
    void EmitNodes(
        SourceProductionContext context,
        (ImmutableArray<INode>, ImmutableArray<TreeOptions>) source)
    {
        var nodes = source.Item1;
        var options = source.Item2.Length > 0 ? source.Item2[0] : new TreeOptions();

        // Generating hierarchy and reporting captured errors...
        var treecontext = new TreeContext(context, options);
        var files = new List<TypeNode>();

        foreach (var node in nodes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (node is ErrorNode error) error.Report(context);
            else CaptureHierarchy(files, node, in treecontext);
        }

        // Emitting source code...
        var reverseparts = options.ReverseGeneratedFileNames;
        var usefolder = options.GenerateFilesInFolders;

        foreach (var file in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var cb = new CodeBuilder();
            var ok = file.Emit(in treecontext, cb);
            if (ok)
            {
                var fname = file.EasyFileName + ".cs";
                var name = NormalizeFileName(fname, reverseparts, usefolder);
                var code = cb.ToString();
                context.AddSource(name, code);
            }
        }
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
}