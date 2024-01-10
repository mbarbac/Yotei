#if DEBUG && DEBUG_LAMBDA_PARSER
#define DEBUGPRINT
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lambda expressions
/// having dynamic arguments.
/// </summary>
public partial class LambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns a new instance that contains the
    /// dynamic arguments used to invoke it, and the chain of dynamic operations found while it
    /// was executed.
    /// <br/> The values of the optional collection of concrete arguments are used to replace
    /// the ones in the lambda expression that are not dynamic ones.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="concretes"></param>
    /// <returns></returns>
    public static LambdaParser Parse(Delegate expression, params object?[] concretes)
    {
        expression = expression.ThrowWhenNull();
        concretes ??= [null!];

        var parser = new LambdaParser();
        var items = new List<object?>();

        // Obtaining the delegate's method...
        var method = expression.GetMethodInfo() ??
           throw new ArgumentException(
               "Cannot obtain the method of the given lambda expression.");

        if (method.ReturnType == typeof(void)) throw new NotSupportedException(
            "Action-alike delegates, with no return type, are not supported.");

        // Parsing the expression arguments...
        var pars = method.GetParameters();
        var index = 0;
        for (int i = 0; i < pars.Length; i++)
        {
            var par = pars[i];

            if (IsDynamic(par)) // Capturing dynamic arguments...
            {
                var name = par.Name ?? throw new ArgumentException(
                    "A dynamic argument in the lambda expression has no name.")
                    .WithData(par);

                var dyn = new LambdaNodeArgument(name) { LambdaParser = parser };
                parser._DynamicArguments.Add(dyn);
                items.Add(dyn);
            }
            else // Capturing regular arguments...
            {
                if (index >= concretes.Length) throw new NotFoundException(
                    "Not enough concrete arguments provided.")
                    .WithData(concretes);

                items.Add(concretes[index]);
                index++;
            }
        }

        if (concretes.Length > index) throw new InvalidOperationException(
            "Too many concrete arguments provided.")
            .WithData(concretes);

        // Executing the delegate...
        lock (SyncRoot)
        {
            var obj = expression.DynamicInvoke([.. items]);
            parser.Result = parser.LastNode ?? ToLambdaNode(obj, parser);
            return parser;
        }
    }

    /// <summary>
    /// Determines if the given parameter is a dynamic one, or not.
    /// </summary>
    static bool IsDynamic(ParameterInfo par)
    {
        var ats = par.GetCustomAttributes(typeof(DynamicAttribute), false);
        if (ats.Length > 0) return true;

        // This hack comes from NET 4.6, where finding 'DynamicAttribute' didn't work. It is not
        // 100% robust as an argument can be be decorated with 'ClassInterfaceAttribute' by the
        // calling code, although I've never seen it...

        var type = typeof(ClassInterfaceAttribute);
        var at = par.ParameterType.CustomAttributes.FirstOrDefault(x => x.AttributeType == type);
        if (at != null)
        {
            type = typeof(ClassInterfaceType);
            var arg = at.ConstructorArguments.FirstOrDefault(x => x.ArgumentType == type);
            if (arg != default)
            {
                var cit = (ClassInterfaceType)arg.Value!;
                if (cit is ClassInterfaceType.AutoDispatch or ClassInterfaceType.AutoDual)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var args = DynamicArguments.Select(x => x.Sketch()).Sketch();
        return $"({args}) => {Result}";
    }

    /// <summary>
    /// The collection of dynamic arguments used to invoke the dynamic lambda expression.
    /// </summary>
    public ImmutableArray<LambdaNodeArgument> DynamicArguments => _DynamicArguments.ToImmutableArray();
    readonly List<LambdaNodeArgument> _DynamicArguments = [];

    /// <summary>
    /// The chain of dynamic operations binded to the arguments of the dynamic lambda expression
    /// that has been parsed. This property contains the last binded operation on the arguments,
    /// from which the full chain can be obtained.
    /// </summary>
    public LambdaNode Result { get; private set; } = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Private constructor.
    /// </summary>
    private LambdaParser() { }

    /// <summary>
    /// For the purposes of <see cref="LambdaParser"/> the DLR acts as a unique shared resource
    /// whose state must be managed in isolation by each run of the parser.
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

            _ => new LambdaNodeValue(value)
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