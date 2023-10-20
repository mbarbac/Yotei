namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lambda expressions
/// whose sole argument is a dynamic one, returning an instance that contains that argument and
/// the chain of dynamic operations binded to it.
/// </summary>
public class LambdaParser
{
    /// <summary>
    /// Private constructor.
    /// </summary>
    private LambdaParser() { }

    /// <summary>
    /// Parses the given dynamic lambda expression and returns a new instance that contains its
    /// dynamic argument and the chain of dynamic operations binded to it.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static LambdaParser Parse(Func<dynamic, object> expression)
    {
        expression = expression.ThrowWhenNull();

        var method = expression.GetMethodInfo() ??
           throw new ArgumentException(
               "Cannot obtain the method of the given dynamic lambda expression.");

        var pars = method.GetParameters();
        if (pars.Length != 1) throw new ArgumentException(
            "The given dynamic expression must have just one argument.");

        var name = pars[0].Name ??
            throw new ArgumentException(
                "The dynamic argument of the dynamic lambda expression has no name.");

        var parser = new LambdaParser() { Argument = new LambdaNodeArgument(name) };
        parser.Argument.LambdaParser = parser;

        lock (CaveatsRoot)
        {
            var obj = expression(parser.Argument);
            parser.Result = parser.LastNode ?? ToLambdaNode(obj, parser);
        }
        return parser;
    }

    /// <summary>
    /// Used to protect the parsing so that only one is performed at the same time. When tests
    /// are executed in parallel under xUnit, it seems there is some sort of interaction between
    /// the DLR and xUnit that mix things together.
    /// See notes in 'Test_Caveats.Setter_Concatenated_On_Same_Dynamic()'.
    /// </summary>
    static object CaveatsRoot = new();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({Argument}) => {Result}";

    /// <summary>
    /// The argument of the dynamic lambda expression that has been parsed.
    /// </summary>
    public LambdaNodeArgument Argument { get; private set; } = default!;

    /// <summary>
    /// The chain of dynamic operations binded to the argument of the dynamic lambda expression
    /// that has been parsed. This property contains the last binded operation on that argument,
    /// from which the full chain can be obtained.
    /// </summary>
    public LambdaNode Result { get; private set; } = default!;

    // ----------------------------------------------------

    /// <summary>
    /// The enforced last binded node. The default value of this property is <c>null</c>, and
    /// in general all the binders should set it to <c>null</c> - except those for which the DLR
    /// loose track of the binded value.
    /// </summary>
    internal LambdaNode? LastNode { get; set; } = null;

    /// <summary>
    /// Used to keep track of the dynamic node surrogates for the given objects.
    /// </summary>
    internal Dictionary<object, LambdaNode> Surrogates { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a dynamic node for the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parser"></param>
    /// <returns></returns>
    internal static LambdaNode ToLambdaNode(object? value, LambdaParser? parser)
    {
        if (parser != null &&
            value != null &&
            parser.Surrogates.TryGetValue(value, out var node)) return node;

        return value switch
        {
            LambdaNode item => item,
            LambdaMetaNode item => item.LambdaNode,
            DynamicMetaObject item => ToLambdaNode(item.Value, parser),

            _ => new LambdaNodeConstant(value),
        };
    }

    /// <summary>
    /// Returns an array with the dynamic nodes for the given values.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="parser"></param>
    /// <returns></returns>
    internal static LambdaNode[] ToLambdaNodes(object?[]? values, LambdaParser? parser)
    {
        if (values == null) return Array.Empty<LambdaNode>();

        var items = new LambdaNode[values.Length];
        for (int i = 0; i < values.Length; i++)
            items[i] = ToLambdaNode(values[i], parser);

        return items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to get a type-compatible object for conversion purposes. If the parser is not
    /// null, the this method adds an entry into its <see cref="Surrogates"/>
    /// dictionary.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="node"></param>
    /// <param name="parser"></param>
    /// <param name="surrogate"></param>
    /// <returns></returns>
    internal static object GetCompatible(
        Type type,
        LambdaNode node,
        LambdaParser? parser,
        LambdaNodeConvert surrogate)
    {
        var r = CreateCompatible(type, node);

        if (parser != null) parser.Surrogates[r] = surrogate;
        return r;
    }

    /// <summary>
    /// Invoked to get an object compatible with the given type. These results will be used as
    /// the keys of the <see cref="Surrogates"/> dictionary, so must not be null. The documents
    /// mention that <see cref="RuntimeHelpers.GetUninitializedObject(Type)"/> does not create
    /// uninitialized string (because empty instances of immutable types serve no purpose...),
    /// so we intercept and create an unique one to prevent reusing instances.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    static object CreateCompatible(Type type, LambdaNode node)
    {
        if (type.IsAssignableTo(typeof(LambdaNode))) return node;

        if (type == typeof(string)) return Guid.NewGuid().ToString();

        try
        {
            var r = RuntimeHelpers.GetUninitializedObject(type);
            if (r is not null) return r;
        }
        catch { }
        return new object();
    }
}