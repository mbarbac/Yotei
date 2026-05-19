namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class CloneGenerator : TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        base.OnInitialize(context);
        AddLocalResource(context, 
            "Public/CloneableAttribute.cs",
            "Markers/CloneableAttribute.cs");
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [typeof(CloneableAttribute)];
    
    // DEBUG-ONLY: remove property attributes
    protected override List<Type> PropertyAttributes { get; } = [typeof(CloneableAttribute)];

    // DEBUG-ONLY: remove field attributes
    protected override List<Type> FieldAttributes { get; } = [typeof(CloneableAttribute)];

    // DEBUG-ONLY: remove method attributes
    protected override List<Type> MethodAttributes { get; } = [typeof(CloneableAttribute)];

    // DEBUG-ONLY: remove CaptureNode method.
    protected override INode CaptureNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        var node = base.CaptureNode(context, token);

        if (node is TypeNode item)
        {
            EasyTypeOptions options;
            string str;
            var type = item.Symbol;

            //var source = type.BaseType!.TypeArguments[0];

            var items = type.GetMembers().OfType<IMethodSymbol>();
            var method = items.Single(x => x.MethodKind == MethodKind.Ordinary);
            var source = method.Parameters.First().Type;

            options = EasyTypeOptions.Empty; str = source.EasyName(options); Debug.WriteLine(str);
            options = EasyTypeOptions.Default; str = source.EasyName(options); Debug.WriteLine(str);
            options = EasyTypeOptions.Full; str = source.EasyName(options); Debug.WriteLine(str);
        }

        return node;
    }
}