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

        if (node is TypeNode tnode) // DEBUG-ONLY
        {
            string str;
            EasyTypeOptions options;
            var source = tnode.Symbol;

            options = EasyTypeOptions.Empty; str = source.EasyName(options); Debug.WriteLine(str);
            options = EasyTypeOptions.Default; str = source.EasyName(options); Debug.WriteLine(str);
            options = EasyTypeOptions.DefaultEx; str = source.EasyName(options); Debug.WriteLine(str);
            options = EasyTypeOptions.Full; str = source.EasyName(options); Debug.WriteLine(str);
        }

        if (node is MethodNode mnode) // DEBUG-ONLY
        {
            string str;
            EasyMethodOptions options;
            var source = mnode.Symbol;

            options = EasyMethodOptions.Empty; str = source.EasyName(options); Debug.WriteLine(str);
            options = EasyMethodOptions.Default; str = source.EasyName(options); Debug.WriteLine(str);
            options = EasyMethodOptions.DefaultEx; str = source.EasyName(options); Debug.WriteLine(str);
            options = EasyMethodOptions.Full; str = source.EasyName(options); Debug.WriteLine(str);
        }

        return node;
    }
}