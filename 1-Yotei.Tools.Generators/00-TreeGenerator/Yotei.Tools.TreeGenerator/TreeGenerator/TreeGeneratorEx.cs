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
        string nspace = GetType().Namespace!;
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

        AddSourceContents(context, nspace, false, "IsNullable[T]", source);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the contents of a source file embedded in the generator's assembly resources, as for
    /// instance marker attributes ones. The file name must match the resource name, including its
    /// path if applicable. The namespace typically is the namespace of the current generator.
    /// <br/> Use a '[EmbeddedResource Include="name.cs" /]' specification whithin an ItemGroup to
    /// embed the given source file in the generator's project.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="nspace"></param>
    /// <param name="fname"></param>
    /// <returns></returns>
    protected string ReadSourceContents(string nspace, string fname)
    {
        fname = fname.NotNullNotEmpty(trim: true);
        nspace = nspace.ThrowWhenNull().Trim();

        var path = nspace.Length > 0 ? $"{nspace}.{fname}" : fname;
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(path)
            ?? throw new NotFoundException($"Embedded file not found: {fname}");

        using var reader = new StreamReader(stream);
        var source = reader.ReadToEnd();
        return source;
    }

    /// <summary>
    /// Adds the given source code to the compilation.
    /// <br/> The name of the actual file to add if built from the given namespace and original
    /// file, in reversed dot order, after having removed given extension, and adding the final
    /// one (provided they are not null ones).
    /// <br/> The given '<paramref name="fname"/>' must not contain invalid file characters (ie:
    /// substitute angle brackets by their corresponding squared ones). Reversed dot order is then
    /// obtained using the first-level dots not contained within squared brakets.
    /// <br/> If '<paramref name="usefolder"/>' is requested, then the namespace part is used as
    /// the folder where to place the emitted file.
    /// </summary>
    protected void AddSourceContents(
        IncrementalGeneratorPostInitializationContext context,
        string nspace, bool usefolder,
        string fname,
        string source,
        string? removelast = "cs",
        string? addlast = "g.cs",
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        nspace = nspace.ThrowWhenNull().Trim();
        fname = fname.NotNullNotEmpty(trim: true);
        source = source.ThrowWhenNull();
        removelast = removelast.NullWhenEmpty(trim: true);
        addlast = addlast.NullWhenEmpty(trim: true);

        if (removelast != null && !removelast.StartsWith('.'))
            removelast = $".{removelast}";

        if (removelast != null && fname.EndsWith(removelast, comparison))
            fname = fname.RemoveLast(removelast, comparison).ToString();

        var name = GetFileName();
        context.AddSource(name, source);

        /// <summary>
        /// Gets the actual file name where to emit the given source.
        /// </summary>
        string GetFileName()
        {
            if (!usefolder || nspace.Length == 0) // Flat file names...
            {
                var name = nspace.Length > 0 ? $"{nspace}.{fname}" : fname;
                var parts = GetDotParts(name);
                parts.Reverse();
                if (addlast != null) parts.Add(addlast);

                name = string.Join(".", parts);
                return name;
            }
            else // Use the namespace part as the folder...
            {
                var parts = GetDotParts(fname);
                parts.Reverse();
                if (addlast != null) parts.Add(addlast);

                var name = string.Join(".", parts);
                name = $"{nspace}/{name}";
                return name;
            }
        }

        /// <summary>
        /// Gets the list of first-level dot separated parts, protected by squared brackets.
        /// </summary>
        List<string> GetDotParts(string str)
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