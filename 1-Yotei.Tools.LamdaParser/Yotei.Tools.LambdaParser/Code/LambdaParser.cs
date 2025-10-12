namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions returning the last node of the
/// chain of dynamic operations binded against the dynamic operations of that expression, which
/// are also captured and returned.
/// </summary>
public class LambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns an instance that maintains both
    /// the last node of the tree of dynamic operations binded against the dynamic arguments of
    /// that expression, and those dynamic arguments as well. The optional collection of concrete
    /// arguments is used to provide the values of the not-dynamic arguments, if any.
    /// <br/> Action-alike delegates, or delegates that return void, are not supported.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="concretes"></param>
    /// <returns></returns>
    public static LambdaParser Parse(Delegate expression, params object?[]? concretes)
    {
        expression.ThrowWhenNull();

        // Obtaining the delegate's method...
        var method = expression.GetMethodInfo() ??
           throw new ArgumentException(
               "Cannot obtain the method of the given lambda expression.");

        if (method.ReturnType == typeof(void))
            throw new NotSupportedException(
                "Action-alike delegates, with no return type, are not supported.");

        // Parsing the expression arguments...
        concretes ??= [null!];

        var items = new List<object?>();
        var args = new List<LambdaNodeArgument>();
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

                var dyn = new LambdaNodeArgument(name);
                args.Add(dyn);
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

        concretes ??= [null];
        if (concretes.Length > index) // Too many concrete arguments...
        {
            throw new InvalidOperationException(
                "Too many concrete arguments provided.").WithData(concretes);
        }

        // Executing under a lock...
        lock (SyncRoot)
        {
            Clear(Instance);
            foreach (var arg in args) Instance._DynamicArguments.Add(arg);

            var obj = expression.DynamicInvoke([.. items]);

            // Hacks for special return types...
            if (obj != null)
            {
                var type = obj.GetType();

                if (type.IsAnonymous) // Anonymous types...
                {
                    Instance.Result = new LambdaNodeValue(obj);
                    goto FINISH;
                }
                else if (type.IsArray) // Arrays...
                {
                    var array = (Array)obj;
                    var nodes = new LambdaNode[array.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        nodes[i] = Instance.ToLambdaNode(array.GetValue(i));
                    }

                    Instance.Result = new LambdaNodeValue(nodes);
                    goto FINISH;
                }
                else if (obj is ICollection list) // Lists and alike...
                {
                    var ret = new List<LambdaNode>();
                    foreach (var item in list) ret.Add(Instance.ToLambdaNode(item));

                    Instance.Result = new LambdaNodeValue(ret);
                    goto FINISH;
                }
            }

            // Standard case...
            Instance.Result = Instance.LastNode ?? Instance.ToLambdaNode(obj);

            // Finishing...
            FINISH:
            var parser = Clone(Instance);
            Clear(Instance);
            return parser;
        }
    }

    /// <summary>
    /// Returns a new instance with the values of its properties copied from the given source.
    /// Note that there is not the need to copy them all, only the relevant ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    static LambdaParser Clone(LambdaParser source)
    {
        var target = new LambdaParser();

        foreach (var arg in source._DynamicArguments) target._DynamicArguments.Add(arg);
        target.Result = source.Result;
        target.LastNode = source.LastNode;

        return target;
    }

    /// <summary>
    /// Invoked to clear the given parser instance.
    /// </summary>
    static void Clear(LambdaParser parser)
    {
        parser._DynamicArguments = [];
        parser.Result = null!;
        parser.LastNode = null!;
        parser.Surrogates = [];
    }

    /// <summary>
    /// Determines if the given parameter is a dynamic one or not.
    /// <para>
    /// By default it could be determined by checking if it is decorarated with 'DynamicAttribute'
    /// but, at least from NET 4.6 and some other versions, it didn't work. So we use a hack that,
    /// despite it is not 100% robust (*) I've never seen a situation where it doesn't work.
    /// <br/> (*) Calling code can always decorate an argument with 'ClassInterfaceAttribute'.
    /// </para>
    /// </summary>
    static bool IsDynamic(ParameterInfo par)
    {
        var ats = par.GetCustomAttributes(typeof(DynamicAttribute), false);
        if (ats.Length > 0) return true;

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var args = DynamicArguments.Select(x => x.Sketch()).Sketch();
        return $"({args}) => {Result}";
    }

    /// <summary>
    /// The collection of dynamic arguments used to invoke the dynamic lambda expression this
    /// instance is associated with.
    /// </summary>
    public ImmutableArray<LambdaNodeArgument> DynamicArguments => [.. _DynamicArguments];
    List<LambdaNodeArgument> _DynamicArguments = [];

    /// <summary>
    /// Represents the chain of dynamic operations obtained from parsing the given dynamic lambda
    /// expression. This property contains the last binded operation, from whose parents the full
    /// chain can be obtained.
    /// </summary>
    public LambdaNode Result { get; private set; } = default!;

    /// <summary>
    /// Maintains the last binded node.
    /// </summary>
    internal LambdaNode? LastNode { get; set; }

    /// <summary>
    /// The internal instance used to actually parse dynamic lambda expressions.
    /// </summary>
    internal static LambdaParser Instance { get; } = new();

    /// <summary>
    /// For the purposes of the dynamic lambda expression parser, the DLR engine acts as a unique
    /// shared resource whose execution and state must be isolated each execution of the parser,
    /// which we achieve using the protection of this lock.
    /// </summary>
    internal static object SyncRoot { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Maintains the surrogates associated with the object in the collection.
    /// </summary>
    internal Dictionary<object, LambdaNode> Surrogates = [];

    /// <summary>
    /// Returns a dynamic lambda node surrogate associated with the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal LambdaNode ToLambdaNode(object? value)
    {
        if (value == null) return new LambdaNodeValue(null);

        if (Surrogates.TryGetValue(value, out var node)) return node;

        node = value switch
        {
            LambdaNode item => item,
            LambdaMetaNode item => item.ValueNode,
            DynamicMetaObject item => ToLambdaNode(item.Value),

            _ => Surrogates[value] = new LambdaNodeValue(value)
        };

        return node;
    }

    /// <summary>
    /// Returns an array of dynamic lambda node surrogates associated with each element of the
    /// given one of values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    internal LambdaNode[] ToLambdaNodes(object?[]? values)
    {
        if (values == null) return [];

        var items = new LambdaNode[values.Length];
        for (int i = 0; i < values.Length; i++) items[i] = ToLambdaNode(values[i]);
        return items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, followed by a line
    /// terminator, using the given foreground color, and replicates it to the console output.
    /// </summary>
    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(
        ConsoleColor color, string message) => DebugEx.WriteLine(true, color, message);
}