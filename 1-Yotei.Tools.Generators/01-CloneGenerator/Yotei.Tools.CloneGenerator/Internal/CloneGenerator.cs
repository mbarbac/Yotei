namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class CloneGenerator : TreeGenerator
{
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        base.OnInitialize(context);
        AddLocalResource(context, "Public/CloneableAttribute.cs", "Markers");
    }

    protected override List<Type> TypeAttributes { get; } = [typeof(CloneableAttribute)];
    protected override List<Type> PropertyAttributes { get; } = [typeof(CloneableAttribute)];
    protected override List<Type> FieldAttributes { get; } = [typeof(CloneableAttribute)];
    protected override List<Type> MethodAttributes { get; } = [typeof(CloneableAttribute)];

    protected override INode CaptureNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        var node = base.CaptureNode(context, token);

        if (node is PropertyNode xnode &&
            xnode.Symbol.Name == "Name") // DEBUG-ONLY
        {
            var source = xnode.Symbol.Type;
            var options = EasyTypeOptions.DefaultEx;
            var str = source.EasyName(options);

            options = EasyTypeOptions.Full;
            str = source.EasyName(options);
        }
        return node;
    }
}