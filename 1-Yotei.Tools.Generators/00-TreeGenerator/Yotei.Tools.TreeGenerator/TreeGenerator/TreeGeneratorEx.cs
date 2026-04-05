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

        // TODO - Initialize
        return;
    }

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
        var nspace = GetType().Namespace!;
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

        AddSourceContents(context, !EmitFilesInFolders ? null : nspace, "IsNullable[T]", source);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the contents of a source file embedded in the generator's assembly resources (for
    /// instance: marker attributes).
    /// <br/> The resource name must match the one used in the project file (typically in an
    /// '[EmbeddedResource Include="name.cs" /]' entry of an ItemGroup section).
    /// </summary>
    /// <param name="rname"></param>
    /// <returns></returns>
    protected string ReadSourceContents(string rname)
    {
        // For whatever reasons 'folder\name' must be specified as 'folder.name'...
        rname = rname.NotNullNotEmpty(trim: true);
        rname = rname.Replace('\\', '.');

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
    /// Adds the given source contents to the compilation in a file whose actual name is built
    /// from the given folder and name, in reversed dot order.
    /// <br/> If the folder is not null, then is used to place the file in that folder, which is
    /// not dot-reversed.
    /// <br/> The name must not contain invalid file name characters (ie: angle brackets must be
    /// replaced by their squared brackets counterparts). Then, it is dot-order reversed using the
    /// first-level dots, provided they are not protected by squared brackets.
    /// <br/> If not null, the <paramref name="removeExtension"/> extension is removed from the
    /// original name. Similarly, if the <paramref name="addExtension"/> extension is not null,
    /// is then added to the final name.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="folder"></param>
    /// <param name="fname"></param>
    /// <param name="source"></param>
    /// <param name="removeExtension"></param>
    /// <param name="addExtension"></param>
    /// <param name="comparison"></param>
    protected void AddSourceContents(
        IncrementalGeneratorPostInitializationContext context,
        string? folder, string fname,
        string source,
        string? removeExtension = "cs", string? addExtension = "g.cs",
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        folder = folder.NullWhenEmpty(trim: true);
        fname = fname.NotNullNotEmpty(trim: true);
        source = source.ThrowWhenNull();

        removeExtension = removeExtension.NullWhenEmpty(trim: true);
        if (removeExtension != null && !removeExtension.StartsWith('.'))
            removeExtension = $".{removeExtension}";

        addExtension = addExtension.NullWhenEmpty(trim: true);
        if (addExtension != null && addExtension.StartsWith('.'))
            addExtension = addExtension[1..].NullWhenEmpty(trim: true);

        fname = GetFileName();
        context.AddSource(fname, source);

        /// <summary>
        /// Gets the actual file name to use.
        /// </summary>
        string GetFileName()
        {
            if (removeExtension != null && fname.EndsWith(removeExtension, comparison))
                fname = fname.RemoveLast(removeExtension, comparison).ToString();

            var parts = GetDotParts(fname);
            parts.Reverse();
            if (addExtension != null) parts.Add(addExtension);

            fname = string.Join(".", parts);
            if (folder != null) fname = $"{folder}/{fname}";
            return fname;
        }

        /// <summary>
        /// Gets the list of first-level dot parts, as protected by squared brackets.
        /// </summary>
        static List<string> GetDotParts(string str)
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

            return parts;
        }
    }
}