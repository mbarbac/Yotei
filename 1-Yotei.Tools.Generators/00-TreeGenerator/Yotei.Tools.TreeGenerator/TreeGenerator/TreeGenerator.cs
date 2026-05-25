namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents represents a tree-oriented incremental source code generator that organizes its
/// captured nodes into a hierarchical tree where each top-most node corresponds to a single type
/// that will be emitted in its own file, along with its child elements, if any.
/// <para>
/// Derived types need to be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. In is also expected that <see cref="LanguageNames.CSharp"/> is used
/// as its argument.
/// </para>
/// </summary>
/// DEBUG NOTE:
/// - Install the .NET compiler SDK (in addition to Roslyn components).
/// - Make sure the derived generator' project is a Roslyn component:
///         <IsRoslynComponent>true</IsRoslynComponent>
/// - In the derived generator project's properties, add a Roslyn debug profile whose target shall
///   be the project that, when compiled, will be debugged (ie: a test project). This debugs the
///   generator project as well as a kind-of side-effect.
/// - Mark the derived generator project as the startup one (not the test one!).
/// - In the play button, select the debug profile.
/// - Click F5 (run) to compile (F6 does nothing).
public partial class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// The name of the consuming project configuration file. If its value is <see langword="null"/>
    /// then no options are read.
    /// </summary>
    protected virtual string ConfigurationFileName { get; } = "TreeGeneratorOptions.ini";

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register post-initialization actions, such as reading external source files, or
    /// generating code for marker attributes, among others.
    /// <br/> Inheritors must invoke their base method first.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
    }

    /// <summary>
    /// Adds the contents of the given embedded resource file, in the generator project, to the
    /// compilation, in the given path.
    /// <para>
    /// The resource file must be identified as an embedded resource in the generator's project
    /// file by using a "[EmbeddedResource Include="folder\name.ext" /]" line in a 'ItemGroup'
    /// section (square brackets must be substituted by their angle counterparts). The output
    /// path follows the same 'folder\name.ext' convention.
    /// </para>
    /// <para>Best practice: add the resource files one by one, ex-novo, not copying them from
    /// any source or template. Then, once created, identify them as an embedded resource in the
    /// project file.</para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="rname"></param>
    /// <param name="path"></param>
    protected void AddLocalResource(
        IncrementalGeneratorPostInitializationContext context,
        string rname,
        string path)
    {
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types used to identify decorated type-alike elements.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated property-alike elements.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated field-alike elements.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated method-alike elements.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated type
    /// -alike elements.
    /// </summary>
    protected virtual List<string> TypeAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated property
    /// -alike elements.
    /// </summary>
    protected virtual List<string> PropertyAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated field
    /// -alike elements.
    /// </summary>
    protected virtual List<string> FieldAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated method
    /// -alike elements.
    /// </summary>
    protected virtual List<string> MethodAttributeNames { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// source code generation candidate, or not.
    /// <para>
    /// This method, by default, validates that the node kind is among the recognized ones for
    /// which either a list of attribute types or a list of attributes' full qualified names, or
    /// both, is provided. Inheritors may override this behavior to add other node kinds, or add
    /// custom validation rules.
    /// </para>
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool TreePredicate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node switch
        {
            BaseTypeDeclarationSyntax => TypeAttributes.Count > 0 || TypeAttributeNames.Count > 0,
            BasePropertyDeclarationSyntax => PropertyAttributes.Count > 0 || PropertyAttributeNames.Count > 0,
            BaseFieldDeclarationSyntax => FieldAttributes.Count > 0 || FieldAttributeNames.Count > 0,
            BaseMethodDeclarationSyntax => MethodAttributes.Count > 0 || MethodAttributeNames.Count > 0,
            _ => false
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new node of the appropriate type for this instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected virtual TypeNode CreateNode(INamedTypeSymbol symbol) => new(symbol);

    /// <summary>
    /// Invoked to create a new node of the appropriate type for this instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected virtual PropertyNode CreateNode(IPropertySymbol symbol) => new(symbol);

    /// <summary>
    /// Invoked to create a new node of the appropriate type for this instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected virtual FieldNode CreateNode(IFieldSymbol symbol) => new(symbol);

    /// <summary>
    /// Invoked to create a new node of the appropriate type for this instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected virtual MethodNode CreateNode(IMethodSymbol symbol) => new(symbol);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to capture the potential syntax node candidate by transforming it into a suitable
    /// source code generation node. This method may return <see langword="null"/> to ignore that
    /// node, or <see cref="ErrorNode"/> instances to hold errors to be reported at source code
    /// generation time.
    /// <para>
    /// This method, by default, validates that the candidate syntax node has any attributes that
    /// match the defined ones for its kind and, if so, creates an appropriate instance. Inheritors
    /// may override this method as needed.
    /// </para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual INode CaptureNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Type-alike nodes.
        while (node is BaseTypeDeclarationSyntax syntax && (
            syntax is TypeDeclarationSyntax ||
            syntax is EnumDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol == null) return null!;

            var atx = FindSyntaxAttributes(syntax, symbol);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) return null!;

            var temp = CreateNode(symbol).With(syntax, ats, model);
            return temp;
        }

        // Property-alike nodes.
        while (node is BasePropertyDeclarationSyntax syntax && (
            syntax is PropertyDeclarationSyntax ||
            syntax is IndexerDeclarationSyntax ||
            syntax is EventDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token) as IPropertySymbol;
            if (symbol == null) return null!;

            var atx = FindSyntaxAttributes(syntax, symbol);
            var ats = FilterAttributes(atx, PropertyAttributes, PropertyAttributeNames);
            if (ats.Count == 0) return null!;

            var temp = CreateNode(symbol).With(syntax, ats, model);
            return temp;
        }

        // Property-alike nodes.
        while (node is BaseFieldDeclarationSyntax syntax && (
            syntax is FieldDeclarationSyntax ||
            syntax is EventFieldDeclarationSyntax))
        {
            var items = syntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol == null) continue;

                var atx = FindSyntaxAttributes(syntax, symbol);
                var ats = FilterAttributes(atx, FieldAttributes, FieldAttributeNames);
                if (ats.Count == 0) continue;

                var temp = CreateNode(symbol).With(syntax, ats, model);
                return temp;
            }
            return null!;
        }

        // Method-alike nodes.
        while (node is BaseMethodDeclarationSyntax syntax && (
            syntax is MethodDeclarationSyntax ||
            syntax is ConstructorDeclarationSyntax ||
            syntax is DestructorDeclarationSyntax ||
            syntax is OperatorDeclarationSyntax ||
            syntax is ConversionOperatorDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol == null) return null!;

            var atx = FindSyntaxAttributes(syntax, symbol);
            var ats = FilterAttributes(atx, PropertyAttributes, PropertyAttributeNames);
            if (ats.Count == 0) return null!;

            var temp = CreateNode(symbol).With(syntax, ats, model);
            return temp;
        }

        // Finishing...
        return null!;
    }

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
    /// of the given types, or ay of the given fully qualified names.
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
    /// Invoked to capture the given node, or its information, into the hierarchy repressented by
    /// the given collection of top-most files (where a new one can be added if needed). Returns
    /// the actual node captured, the one existing but augmented, or null if any.
    /// <br/> Inheritors may want to override this method if they deal with new or derived nodes,
    /// eventually calling their base method first as needed.
    /// </summary>
    /// <param name="files"></param>
    /// <param name="node"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual INode? CaptureHierarchy(
        List<TypeNode> files, INode node, in TreeContext context)
    {
        var comparer = SymbolEqualityComparer.Default;

        // Type nodes...
        if (node is TypeNode tnode)
        {
            var type = files.Find(x => comparer.Equals(x.Symbol, tnode.Symbol));

            if (type is null) files.Add(tnode); // Adding the new type node...
            else
            {
                if (type.ChildsOnly) // Substituting the childs-only existing one...
                {
                    tnode.Augment(type);
                    files.Remove(type);
                    files.Add(tnode);
                    return tnode;
                }
                else // Or augmenting the existing one...
                {
                    type.Augment(tnode);
                    return type;
                }
            }
        }

        // Property nodes...
        if (node is PropertyNode pnode)
        {
            var host = pnode.Symbol.ContainingType;
            var type = files.Find(x => comparer.Equals(x.Symbol, host));

            if (type is null) // Creating a childs-only instance...
            {
                type = new TypeNode(host) { ChildsOnly = true };
                files.Add(type);
            }

            var item = type.ChildProperties.Find(x => comparer.Equals(x.Symbol, pnode.Symbol));

            if (item is null) // Adding a new child...
            {
                type.ChildProperties.Add(pnode);
                pnode.Parent = type;
                return pnode;
            }
            else // Or augmenting the existing one...
            {
                item.Augment(pnode);
                return item;
            }
        }

        // Field nodes...
        if (node is FieldNode fnode)
        {
            var host = fnode.Symbol.ContainingType;
            var type = files.Find(x => comparer.Equals(x.Symbol, host));

            if (type is null) // Creating a childs-only instance...
            {
                type = new TypeNode(host) { ChildsOnly = true };
                files.Add(type);
            }

            var item = type.ChildFields.Find(x => comparer.Equals(x.Symbol, fnode.Symbol));

            if (item is null) // Adding a new child...
            {
                type.ChildFields.Add(fnode);
                fnode.Parent = type;
                return fnode;
            }
            else // Or augmenting the existing one...
            {
                item.Augment(fnode);
                return item;
            }
        }

        // Method nodes...
        if (node is MethodNode mnode)
        {
            var host = mnode.Symbol.ContainingType;
            var type = files.Find(x => comparer.Equals(x.Symbol, host));

            if (type is null) // Creating a childs-only instance...
            {
                type = new TypeNode(host) { ChildsOnly = true };
                files.Add(type);
            }

            var item = type.ChildMethods.Find(x => comparer.Equals(x.Symbol, mnode.Symbol));

            if (item is null) // Adding a new child...
            {
                type.ChildMethods.Add(mnode);
                mnode.Parent = type;
                return mnode;
            }
            else // Or augmenting the existing one...
            {
                item.Augment(mnode);
                return item;
            }
        }

        // Ignoring...
        return null;
    }
}