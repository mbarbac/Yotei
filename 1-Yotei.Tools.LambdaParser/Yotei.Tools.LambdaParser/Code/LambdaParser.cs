#pragma warning disable IDE0305

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lambda expressions
/// having dynamic arguments, returning the chain of dynamic operations bounded to them.
/// </summary>
public partial class LambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns a new instance that maintains the
    /// dynamic arguments used to invoke it, and the chain of dynamic operations found while it
    /// was executed to resolve them.
    /// <br/> The optional collection of concrete arguments is used to replace the not-dynamic
    /// arguments in the given expression with the given values.
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

        if (method.ReturnType == typeof(void))
            throw new NotSupportedException(
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
    /// Determines if the given parameter correspond to a dynamic argument in the given dynamic
    /// lambda expression, or not.
    /// </summary>
    /// <param name="par"></param>
    /// <returns></returns>
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

    // ----------------------------------------------------

    /// <summary>
    /// Private constructor.
    /// </summary>
    private LambdaParser() { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var args = DynamicArguments.Select(x => x.Sketch()).Sketch();
        return $"({args}) => {Result}";
    }

    /// <summary>
    /// For the purposes of <see cref="LambdaParser"/> the DLR acts as a unique shared resource
    /// whose state must be isolated each run of the parser. The <c>Parse(...)</c> method waits
    /// until and exclusive lock can be captured on this object.
    /// </summary>
    public static object SyncRoot { get; } = new();

    /// <summary>
    /// The collection of dynamic arguments used to invoke the given dynamic lambda expression.
    /// </summary>
    public ImmutableArray<LambdaNodeArgument> DynamicArguments => _DynamicArguments.ToImmutableArray();
    readonly List<LambdaNodeArgument> _DynamicArguments = [];

    /// <summary>
    /// Represents the chain of dynamic operations binded to the arguments of the given dynamic
    /// lambda expression. This property contains the last binded operation, from whose parents
    /// the full chain can be obtained.
    /// </summary>
    public LambdaNode Result { get; private set; } = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Maitains the last dynamic node binded while parsing the given dynamic lambda expression.
    /// The binders are in charge of setting the value of this property as in some circumstances
    /// the DLR does it not.
    /// </summary>
    internal LambdaNode? LastNode { get; set; }

    /// <summary>
    /// Keeps track of the dynamic lambda nodes that shall be used as surrogates for the tracked
    /// objects.
    /// </summary>
    internal Dictionary<object, LambdaNode> Surrogates { get; } = [];

    /// <summary>
    /// Returns a dynamic lambda node associated with the given object. The returned one can
    /// either be an existing surrogate, or an ad-hoc node created for that object.
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
    /// Returns an array of lambda nodes, each associated with the corresponding object in the
    /// given array of objects. Each elements can either be an existing surrogate or a node
    /// created ad-hoc for that object. If the given array is null, and empty one is returned.
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

    internal const ConsoleColor NewNodeColor = ConsoleColor.White;
    internal const ConsoleColor NewMetaColor = ConsoleColor.Gray;
    internal const ConsoleColor NodeBindedColor = ConsoleColor.Blue;
    internal const ConsoleColor MetaBindedColor = ConsoleColor.Yellow;
    internal const ConsoleColor ValidateLambdaColor = ConsoleColor.Cyan;
    internal const ConsoleColor UpdateLambdaColor = ConsoleColor.Magenta;

    /// <summary>
    /// Invoked to print the given debug message.
    /// </summary>
    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(string message) => DebugEx.WriteLine(message);

    /// <summary>
    /// Invoked to print the given debug message.
    /// </summary>
    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(
        ConsoleColor color, string message) => DebugEx.WriteLine(color, message);

}