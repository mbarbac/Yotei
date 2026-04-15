namespace Yotei.Tools.Generators;

// ========================================================
partial class TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// This method is INFRASTRUCTURE only, and it is only intended to be invoked by the compiler.
    /// Application code shall not invoke it.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Registering post-initialization actions...
        context.RegisterPostInitializationOutput(DispatchInitialize);

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(FilterNode, CaptureNode)
            .Where(static x => x is not null)
            .Collect();

        // Registering source code emit actions...
        context.RegisterSourceOutput(items, EmitNodes);
    }

    /* NOTE:
     * As per my understanding of the documentation, if we capture the 'Compilation' object we
     * will then loose the incremental nature of the generator (so that it says that the slightest
     * change or user typing will drive a full source generation over and over again).
     * What I cannot understand is why then it has been made possible:
     *      var combined = context.CompilationProvider.Combine(items);
     *      context.RegisterSourceOutput(combined, EmitNodes);
     * (provided an appropriate signature for the EmitNodes method).
     */

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to dispatch post-initialization actions.
    /// </summary>
    /// <param name="context"></param>
    void DispatchInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddEmbeddedAttributeDefinition();

        if (EmitNullabilityHelpers) DoEmitNullabilityHelpers(context);
        OnInitialize(context);
    }

    /// <summary>
    /// Invoked to emit the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// nullabilily helper types types in the namespace of the current generator.
    /// </summary>
    void DoEmitNullabilityHelpers(IncrementalGeneratorPostInitializationContext context)
    {
        var nspace = GetType().Namespace;
        var source = $$"""
            using System;
            namespace {{nspace}};

            /// <summary>
            /// Used to decorate types for which nullability information shall be persisted, typically
            /// used with reference or generic types.
            /// <para>
            /// Nullable annotations on reference types are just syntactic sugar, used by the compiler
            /// but not persisted in metadata or custom attributes. In addition, the compiler prevents
            /// using them in some constructions. By contrast, the compiler translates annotated value
            /// types into instances of <see cref="Nullable{T}"/>.
            /// </para>
            /// <para>
            /// The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be used
            /// as workarounds when there is the need to persist this nullability information on an
            /// arbitrary type, provided that the drawbacks of using them are acceptable.
            /// </para>
            /// <para>
            /// You are responsible to use it in allowed contexts. For instance, the 'EasyName' family
            /// of functions will not intercept usages not allowed by the compiler, as for instance
            /// some usages with reference types.
            /// </para>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            [Microsoft.CodeAnalysis.Embedded]
            public class IsNullable<T> { }
                
            /// <summary>
            /// <inheritdoc cref="IsNullable{T}"/>
            /// </summary>
            [Microsoft.CodeAnalysis.Embedded]
            [AttributeUsage(AttributeTargets.All)]
            public class IsNullableAttribute : Attribute { }
            """;

        var rname = "IsNullable[T]";
        var fname = GetFileName(nspace, rname, EmitFilesInFolders);
        context.AddSource(fname, source);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the contents of a source file embedded in the generator's assembly resources, as
    /// for instance marker attributes. The resource name must match the one used in the project
    /// file (typically in an '[EmbeddedResource Include="name.cs" /]' entry of an ItemGroup
    /// section, with angle brackets).
    /// </summary>
    /// <param name="rname"></param>
    /// <returns></returns>
    public string ReadResourceContents(string rname)
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

    /// <summary>
    /// Reads the contents of a source file embedded as a resource in the generator's assembly
    /// (for instance, a marker attribute file), and then emits these contents in the generated
    /// source code using a file whose name is built from the given namespace and item name
    /// (that typically is the name of the type the generated file is about).
    /// <br/> The <paramref name="useFolder"/> value is used to determine if the namespace part
    /// shall be used as the folder name. If not, then the name produced will be a flat one. The
    /// value of this parameter is typically the <see cref="EmitFilesInFolders"/> one.
    /// <br/> If <paramref name="removeExtension"/> is not null, it is removed from the original
    /// short file name. If <paramref name="addExtension"/> is not null, it is added at the end of
    /// the produced file name.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="resourceName"></param>
    /// <param name="nspace"></param>
    /// <param name="itemName"></param>
    /// <param name="useFolder"></param>
    /// <param name="removeExtension"></param>
    /// <param name="addExtension"></param>
    /// <param name="comparison"></param>
    public void AddResourceContents(
        IncrementalGeneratorPostInitializationContext context,
        string resourceName,
        string? nspace, string itemName, bool useFolder,
        string? removeExtension = "cs", string? addExtension = "g.cs",
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        var source = ReadResourceContents(resourceName);
        var fname = GetFileName(nspace, itemName, useFolder, removeExtension, addExtension, comparison);
        context.AddSource(fname, source);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains an actual file name where to emit source code, based upon the given namespace and
    /// an item name. The namespace can be null if it is not used. The item name is typically the
    /// the name of type the file is about, maybe with a '.cs' extension.
    /// <br/> If <paramref name="removeExtension"/> is not null, it is removed from the original
    /// short file name. If <paramref name="addExtension"/> is not null, it is added at the end of
    /// the produced file name.
    /// <br/> The <paramref name="useFolder"/> value is used to determine if the namespace part
    /// shall be used as the folder name. If not, then the name produced will be a flat one. The
    /// value of this parameter is typically the <see cref="EmitFilesInFolders"/> one.
    /// </summary>
    /// <param name="nspace"></param>
    /// <param name="itemName"></param>
    /// <param name="useFolder"></param>
    /// <param name="removeExtension"></param>
    /// <param name="addExtension"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public string GetFileName(
        string? nspace, string itemName,
        bool useFolder,
        string? removeExtension = "cs", string? addExtension = "g.cs",
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        nspace = nspace.NullWhenEmpty(trim: true) ?? "";
        var nparts = nspace.Length > 0 ? FirstLevelDotParts(nspace, true) : [];

        itemName = itemName.NotNullNotEmpty(trim: true);
        removeExtension = removeExtension.NullWhenEmpty(trim: true);
        if (removeExtension != null)
        {
            if (!removeExtension.StartsWith('.')) removeExtension = $".{removeExtension}";
            if (itemName.EndsWith(removeExtension, comparison))
                itemName = itemName.RemoveLast(removeExtension, comparison).ToString();
        }
        var fparts = FirstLevelDotParts(itemName, true);

        string name = ""; if (!useFolder)
        {
            fparts.AddRange(nparts);
            name = string.Join(".", fparts);
        }
        else
        {
            if (nparts.Count > 0) name = string.Join(".", nparts) + '/';
            name += string.Join(".", fparts);
        }

        addExtension = addExtension.NullWhenEmpty(trim: true);
        if (addExtension != null)
        {
            if (!addExtension.StartsWith('.')) name += '.';
            name += addExtension;
        }

        return name;
    }

    /// <summary>
    /// Returns the list of first-level file name dot separated parts. Dots inside square brackets
    /// protected sections are not taken into consideration. Unless explicitly not requested, the
    /// returned list is reversed.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static List<string> FirstLevelDotParts(string str, bool reverse = true)
    {
        List<int> dots = [];
        int depth = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '[') { depth++; continue; }
            if (str[i] == ']') { if (depth > 0) depth--; continue; }
            if (str[i] == '.' && depth == 0) dots.Add(i);
        }

        List<string> parts = [];
        int last = 0;
        foreach (var dot in dots)
        {
            parts.Add(str[last..dot]);
            last = dot + 1;
        }
        parts.Add(str[last..]);

        if (reverse) parts.Reverse();
        return parts;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the collection of attributes that decorate the given syntax. Motivation: for
    /// whatever reasons <see cref="ISymbol.GetAttributes"/> does not return all the symbol's
    /// attributes when it is defined in different places (ie: partial types). So we need a
    /// way of capturing all of them from the syntax locations.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <returns></returns>
    static IEnumerable<AttributeData> FindSyntaxAttributes(
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
        var items = attributes.Where(x =>
            x.AttributeClass != null &&
            x.AttributeClass.MatchAny(types)).ToList();

        foreach (var name in names)
        {
            var temps = attributes.Where(x => x.AttributeClass?.Name == name);
            foreach (var temp in temps) if (!items.Contains(temp)) items.Add(temp);
        }

        return items;
    }
}