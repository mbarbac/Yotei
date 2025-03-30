namespace Yotei.Tools;

// ========================================================
/// <summary>
/// 
/// </summary>
public class LambdaParser
{
    // <summary>
    /// Parses the given dynamic lambda expression and returns a new instance that maintains the
    /// dynamic arguments used to invoke it, and the last node in the chain of dynamic operations
    /// found while executing that expression for parsing purposes.
    /// <br/> The optional collection of concrete arguments can be used if not-dynamic arguments
    /// are needed to execute the expression.
    /// <br/> Action-alike expressions, with void or no return type, are not supported.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="concretes"></param>
    /// <returns></returns>
    public static LambdaParser Parse(Delegate expression, params object?[] concretes)
    {
        expression = expression.ThrowWhenNull();

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

        if (concretes.Length > index) // Too many concrete arguments...
        {
            throw new InvalidOperationException(
                "Too many concrete arguments provided.").WithData(concretes);
        }

        // Executing the delegate and returning...
        lock (SyncRoot)
        {
            Clear(Instance);
            foreach (var arg in args) Instance._DynamicArguments.Add(arg);

            var obj = expression.DynamicInvoke([.. items]);
            Instance.Result = Instance.LastNode ?? Instance.ToLambdaNode(obj);

            var parser = NewParserFrom(Instance);
            Clear(Instance);
            return parser;
        }
    }

    /// <summary>
    /// Invoked to clear the given instance.
    /// </summary>
    static void Clear(LambdaParser parser)
    {
        parser._DynamicArguments = [];
        parser.Result = null!;
        parser.LastNode = null;
    }

    /// <summary>
    /// Returns a new instance with the values of its properties copied from the given source.
    /// This method is invoked to produce the instance to return, so there is no real need to
    /// copy all properties.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    static LambdaParser NewParserFrom(LambdaParser source)
    {
        var target = new LambdaParser();

        foreach (var arg in source._DynamicArguments) target._DynamicArguments.Add(arg);
        target.Result = source.Result;
        target.LastNode = source.LastNode;

        return target;
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

    /// <summary>
    /// The internal instance used to actually parse dynamic lambda expressions.
    /// </summary>
    internal static LambdaParser Instance { get; } = new();

    /// <summary>
    /// For the purposes of the dynamic lambda expression parser, the DLR engine acts as a unique
    /// shared resource whose execution and state must be isolated each execution of the parser,
    /// under the protection of this lock.
    /// </summary>
    internal static object SyncRoot { get; } = new();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string ToString()
    {
        var args = DynamicArguments.Select(x => x.Sketch()).Sketch();
        return $"({args}) => {Result}";
    }

    /// <summary>
    /// The collection of dynamic arguments used to invoke the dynamic lambda expression this
    /// instance is associated with.
    /// </summary>
    public ImmutableArray<LambdaNodeArgument> DynamicArguments => _DynamicArguments.ToImmutableArray();
    List<LambdaNodeArgument> _DynamicArguments = [];

    /// <summary>
    /// Represents the chain of dynamic operations obtained from parsing the given dynamic lambda
    /// expression. This property contains the last binded operation, from whose parents the full
    /// chain can be obtained.
    /// </summary>
    public LambdaNode Result { get; private set; } = default!;

    /// <summary>
    /// Maintains the last node binded. <see cref="LambdaNode"/> and <see cref="LambdaMetaNode"/>
    /// bind operations set the value of this property.
    /// </summary>
    internal LambdaNode? LastNode { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a dynamic lambda node associated with the given value, which can either be an
    /// existing surrogate or an ad-hoc instance created for that value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal LambdaNode ToLambdaNode(object? value)
    {
        return value switch
        {
            LambdaNode item => item,
            LambdaMetaNode item => item.ValueNode,
            DynamicMetaObject item => ToLambdaNode(item.Value),

            _ => new LambdaNodeValue(value)
        };
    }

    /// <summary>
    /// Returns an array of dynamic lambda nodes, each associated with one of the given values,
    /// which can either be an existing surrogate or an ad-hoc instance created for that values.
    /// If the given array is null, then an empty one is returned.
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
}