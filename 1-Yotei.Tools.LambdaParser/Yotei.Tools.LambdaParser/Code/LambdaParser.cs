#if DEBUG && DEBUG_LAMBDAPARSER
#define DEBUGPRINT
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lambda expressions
/// whose sole argument is a dynamic one.
/// </summary>
public partial class LambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns a new instance that contains the
    /// dynamic argument used to invoke it, and the chain of dynamic operations found while it
    /// was executed.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
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

        lock (SyncRoot)
        {
            var obj = expression(parser.Argument);
            parser.Result = parser.LastNode ?? ToLambdaNode(obj, parser);
        }
        return parser;
    }

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
    /// Private constructor.
    /// </summary>
    private LambdaParser() { }

    /// <summary>
    /// The DLR is a unique shared resource that, for the purposes of <see cref="LambdaParser"/>,
    /// cannot be used in parallel, because state its state must be managed in isolation by each
    /// run of the parser.
    /// </summary>
    public static object SyncRoot { get; } = new();

    /// <summary>
    /// The last binded node. The binders are in charge to set the appropriate value of this
    /// property, as in some circumstances the DLR does not.
    /// </summary>
    internal LambdaNode? LastNode { get; set; }

    /// <summary>
    /// Used to keep track of the dynamic lambda nodes that shall be used as the surrogates for
    /// the given objects.
    /// </summary>
    internal Dictionary<object, LambdaNode> Surrogates { get; } = [];
    
    // ----------------------------------------------------

    /// <summary>
    /// Returns a dynamic lambda node associated with the given object. It can be an existing
    /// surrogate, or an ad-hoc one created for that object.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parser"></param>
    /// <returns></returns>
    internal static LambdaNode ToLambdaNode(object? value, LambdaParser? parser)
    {
        if (value != null &&
            parser != null &&
            parser.Surrogates.TryGetValue(value, out var node)) return node;

        return value switch
        {
            LambdaNode item => item,
            LambdaMetaNode meta => meta.LambdaNode,
            DynamicMetaObject meta => ToLambdaNode(meta.Value, parser),

            _ => new LambdaNodeConstant(value)
        };
    }

    /// <summary>
    /// Returns an array of lamba nodes each associated with the corresponding object in the
    /// given array. Each element can be an existing surrogate, or an ad-hoc node created for
    /// that object.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="parser"></param>
    /// <returns></returns>
    internal static LambdaNode[] ToLambdaNodes(object?[]? values, LambdaParser? parser)
    {
        if (values == null) return [];

        var items = new LambdaNode[values.Length];
        for (int i = 0; i < values.Length; i++) items[i] = ToLambdaNode(values[i], parser);
        return items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The event handler that is invoked for debug purposes.
    /// </summary>
    [SuppressMessage("", "CA2211")]
    public static EventHandler<string> OnDebug = null!;

    /// <summary>
    /// Invoked to print the given debug message.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    internal static void Print(object? sender, string message)
    {
#if DEBUGPRINT
        var handler = OnDebug;
        handler?.Invoke(sender, message);
#endif
    }
}