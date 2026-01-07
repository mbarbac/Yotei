namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lamda expressions
/// where at least one of its arguments is a dynamic one. Instances of this class contain the last
/// node of the chain of dynamic operations binded against the given dynamic arguments, along with
/// that dynamic arguments arguments themselves.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class LambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns an instance that maintains the last
    /// node binded against the dynamic arguments of that expression, along with the arguments of
    /// the expression, dynamic and concrete ones.
    /// <br/> Action-alike delegates, or delegates returning 'void' are not supported.
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

    // ----------------------------------------------------

    /// <summary>
    /// Creates a copy of the given parser to that the copy can be returned as the result of an
    /// expression parsing. Note that we need not to copy the values of all properties, only for
    /// the relevant ones.
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
    /// Invoked to clear the given parser so that it can be used for other purposes, such as be
    /// returned as the result of an expression parsing.
    /// </summary>
    /// <param name="parser"></param>
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
    /// By default, it could be determined by validating that the parameter is decorated with the
    /// '<see cref="DynamicAttribute"/>' attribute. But in .NET 4.6 a bug was introduced and it
    /// didn't work. So, we use the following hack that, although is not 100% robust (*), I've
    /// never seen a situation where it doesn't work.
    /// <br/> (*) Calling code can freely use <see cref="ClassInterfaceAttribute"/> to decorate
    /// a not-dynamic argument, but I wonder why would an application want to use dynamic parser
    /// in a COM environment.
    /// </para>
    /// </summary>
    /// <param name="par"></param>
    /// <returns></returns>
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
        // HIGH: EasyName related...
        //var args = DynamicArguments.Select(static x => x.Sketch()).Sketch();
        //return $"({args}) => {Result}";
        throw null;
    }

    /// <summary>
    /// The collection of dynamic arguments used to invoke the dynamic lambda expression that is
    /// represented by this instance.
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
    /// Maintains the surrogates associated with the objects in the collection.
    /// </summary>
    internal Dictionary<object, LambdaNode> Surrogates = [];

    /// <summary>
    /// Returns the dynamic lambda node surrogate associated with the given value.
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
    /// Returns the array of dynamic lambda node surrogates associated with each value element
    /// of the given one.
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
    /// Validates that the given name can be used to name dynamic elements.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static string ValidateName(string name)
    {
        name = name.NotNullNotEmpty(true);

        if (!VALID_FIRST.Contains(name[0])) throw new ArgumentException(
            "Name first character is invalid.")
            .WithData(name);

        for (int i = 1; i < name.Length; i++)
            if (!VALID_OTHER.Contains(name[i])) throw new ArgumentException(
            "Name contains an invalid character.")
            .WithData(name);

        return name;
    }

    readonly static string VALID_FIRST =
        "_$@"
        + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        + "abcdefghijklmnopqrstuvwxyz";

    readonly static string VALID_OTHER = VALID_FIRST
        + "#"
        + "0123456789";

    /// <summary>
    /// Validates that the given collection of lambda nodes can be used as the arguments of a
    /// method or indexer. The collection can be empty or not, as requested, but if not, cannot
    /// carry null elements.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="canBeEmpty"></param>
    /// <returns></returns>
    internal static ImmutableArray<LambdaNode> ValidateArguments(
        IEnumerable<LambdaNode> args,
        bool canBeEmpty)
    {
        args.ThrowWhenNull();

        var list = args is ImmutableArray<LambdaNode> temp ? temp : [.. args];

        if (list.Length == 0 && !canBeEmpty) throw new ArgumentException(
            "Collection of arguments cannot be empty.");

        if (list.Length != 0 && list.Any(static x => x is null)) throw new ArgumentException(
            "Collection of arguments carries null elements.")
            .WithData(args);

        return list;
    }

    /// <summary>
    /// Validates that the given collection of types can be used as the type arguments of a
    /// method. The collection cannot be empty and cannot carry null elements.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    internal static ImmutableArray<Type> ValidateTypeArguments(IEnumerable<Type> args)
    {
        args.ThrowWhenNull();

        var list = args is ImmutableArray<Type> temp ? temp : [.. args];

        if (list.Length == 0) throw new EmptyException(
            "Collection of type arguments cannot be empty.");

        if (list.Any(static x => x is null)) throw new ArgumentException(
            "Collection of type arguments carries null elements.")
            .WithData(args);

        return list;
    }

    // ----------------------------------------------------

    internal const ConsoleColor NewNodeColor = ConsoleColor.White;
    internal const ConsoleColor NewMetaColor = ConsoleColor.Gray;
    internal const ConsoleColor NodeBindedColor = ConsoleColor.Yellow;
    internal const ConsoleColor MetaBindedColor = ConsoleColor.Blue;
    internal const ConsoleColor ValidateLambdaColor = ConsoleColor.Cyan;
    internal const ConsoleColor UpdateLambdaColor = ConsoleColor.Magenta;

    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(ConsoleColor color, string message)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        Debug.WriteLine(message);
        Console.ForegroundColor = old;
    }
}