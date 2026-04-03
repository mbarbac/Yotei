namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lambda-alike ones at
/// least one of their arguments is a <see langword="dynamic"/> one. Instances of this type contain
/// the result of that parsing along with the dynamic arguments used to invoke that expression.
/// <br/> Instances of this type are immutable ones.
/// </summary>
public partial class DLambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns an instance that contains the result
    /// of that parsing along with the dynamic arguments used to invoke that expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="concretes"></param>
    /// <returns></returns>
    public static DLambdaParser Parse(Delegate expression, params object?[]? concretes)
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
        var args = new List<DLambdaNodeArgument>();
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

                var dyn = new DLambdaNodeArgument(name);
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
                    Instance.Result = new DLambdaNodeValue(obj);
                    goto FINISH;
                }
                else if (type.IsArray) // Arrays...
                {
                    var array = (Array)obj;
                    var nodes = new DLambdaNode[array.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        nodes[i] = Instance.ToLambdaNode(array.GetValue(i));
                    }

                    Instance.Result = new DLambdaNodeValue(nodes);
                    goto FINISH;
                }
                else if (obj is ICollection list) // Lists and alike...
                {
                    var ret = new List<DLambdaNode>();
                    foreach (var item in list) ret.Add(Instance.ToLambdaNode(item));

                    Instance.Result = new DLambdaNodeValue(ret);
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
    /// Determines if the given parameter is a <see langword="dynamic"/> one, or not (a regular
    /// or concrete one).
    /// <br/> By default, this could be determined by validating that the parameter is decorated
    /// with the <see cref="DynamicAttribute"/> attribute. But since NET 4.6 a regression bug was
    /// introduced so that this was not true for several releases. So, we use the following hack
    /// to overcome this that, although it technically might not be 100% robust (*), I've never
    /// seen a situation where it doesn't work.
    /// <br/> (*): Technically, calling code can freely use <see cref="ClassInterfaceAttribute"/>
    /// to decorate a not-dynamic arguments, but I wonder why would an application want to use
    /// this dynamic lambda parser using COM-related arguments.
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

    /// <summary>
    /// Creates a copy of the given parser that will be the one returned from the 'Parse' method.
    /// Note that we won't need to copy the values of all properties, but only the relevant ones
    /// for the client code to see.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    static DLambdaParser Clone(DLambdaParser source)
    {
        var target = new DLambdaParser();

        foreach (var arg in source._DynamicArguments) target._DynamicArguments.Add(arg);
        target.Result = source.Result;
        target.LastNode = source.LastNode;

        return target;
    }

    /// <summary>
    /// Invoked to clear the state of the given parser so that it can be used for other purposes.
    /// This method is used to clear for the shared instance as well as the to-be returned one.
    /// Again, we only need to actuate on the relevant properties.
    /// </summary>
    /// <param name="parser"></param>
    static void Clear(DLambdaParser parser)
    {
        parser._DynamicArguments.Clear();
        parser.Result = null!;
        parser.LastNode = null!;
        parser.Surrogates = [];
    }

    // ----------------------------------------------------

    // Private constructor to prevent direct creation.
    private DLambdaParser() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var args = DynamicArguments.Select(static x => x.Sketch()).Sketch();
        return $"({args}) => {Result}";
    }

    /// <summary>
    /// The collection of dynamic arguments used to invoke the dynamic lambda expression.
    /// </summary>
    public ImmutableArray<DLambdaNodeArgument> DynamicArguments => [.. _DynamicArguments];
    readonly List<DLambdaNodeArgument> _DynamicArguments = [];

    /// <summary>
    /// Represents the chain of dynamic operations obtained from parsing the given dynamic lambda
    /// expression. This property contains the last binded operation, from whose parents the full
    /// chain can be explored.
    /// </summary>
    public DLambdaNode Result { get; private set; } = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Maintains the last binded node. This property is used to overcome some peculiarities of the
    /// way the DLR operates as, for instance, when some bindings are just swallowed as if they are
    /// not needed (which might be true, but it is not what we need).
    /// </summary>
    internal DLambdaNode? LastNode { get; set; }

    /// <summary>
    /// The internal instance used to actually parse dynamic lambda expressions.
    /// </summary>
    internal static DLambdaParser Instance { get; } = new();

    /// <summary>
    /// For the purposes of the dynamic lambda expression parser, the DLR acts as a unique shared
    /// resource whose execution and state must be isolated each execution of the parser. This is
    /// what we achieve using the protection of this lock.
    /// </summary>
    internal static object SyncRoot { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Maintains the surrogates that shall stand-up for the given objects.
    /// </summary>
    internal Dictionary<object, DLambdaNode> Surrogates = [];

    /// <summary>
    /// Returns the dynamic lambda node surrogate associated with the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal DLambdaNode ToLambdaNode(object? value)
    {
        if (value == null) return new DLambdaNodeValue(null);

        if (Surrogates.TryGetValue(value, out var node)) return node;

        node = value switch
        {
            DLambdaNode item => item,
            DLambdaMetaNode item => item.ValueAsNode,
            DynamicMetaObject item => ToLambdaNode(item.Value),

            _ => Surrogates[value] = new DLambdaNodeValue(value)
        };

        return node;
    }

    /// <summary>
    /// Returns the array of dynamic lambda node surrogates associated with each value element
    /// of the given one.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    internal DLambdaNode[] ToLambdaNodes(object?[]? values)
    {
        if (values == null) return [];

        var items = new DLambdaNode[values.Length];
        for (int i = 0; i < values.Length; i++) items[i] = ToLambdaNode(values[i]);
        return items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to return a validated name suitable for dynamic elements.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static string ValidateName(string name)
    {
        name = name.NotNullNotEmpty(true);

        if (!VALID_FIRST.Contains(name[0])) throw new ArgumentException(
            "First character of name is invalid.")
            .WithData(name);

        for (int i = 1; i < name.Length; i++)
            if (!VALID_OTHER.Contains(name[i])) throw new ArgumentException(
            "Name contains invalid characters.")
            .WithData(name);

        return name;
    }

    readonly static string VALID_FIRST =
        "_$@"
        + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        + "abcdefghijklmnopqrstuvwxyz";

    readonly static string VALID_OTHER = VALID_FIRST + "#" + "0123456789";

    /// <summary>
    /// Invoked to return a validated collection of arguments.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="canBeEmpty"></param>
    /// <returns></returns>
    internal static ImmutableArray<DLambdaNode> ValidateArguments(
        IEnumerable<DLambdaNode> args,
        bool canBeEmpty)
    {
        ArgumentNullException.ThrowIfNull(args);

        var list = args is ImmutableArray<DLambdaNode> temp ? temp : [.. args];

        if (list.Length == 0 && !canBeEmpty) throw new ArgumentException(
            "Collection of arguments cannot be empty.");

        if (list.Length != 0 && list.Any(static x => x is null)) throw new ArgumentException(
            "Collection of arguments carries null elements.")
            .WithData(args);

        return list;
    }

    /// <summary>
    /// Invoked to return a validated collection of type arguments.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    internal static ImmutableArray<Type> ValidateTypeArguments(IEnumerable<Type> args)
    {
        ArgumentNullException.ThrowIfNull(args);

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
    internal const ConsoleColor NewMetaColor = ConsoleColor.DarkGray;
    internal const ConsoleColor NodeBindedColor = ConsoleColor.Yellow;
    internal const ConsoleColor MetaBindedColor = ConsoleColor.DarkYellow;
    internal const ConsoleColor ValidateLambdaColor = ConsoleColor.Cyan;
    internal const ConsoleColor UpdateLambdaColor = ConsoleColor.Magenta;

    static readonly Lock DebugSync = new();

    /// <summary>
    /// Invoked to print the given message in the debug environment.
    /// </summary>
    [Conditional("DEBUG_DYNAMIC_LAMBDA")]
    internal static void ToDebug(string message)
    {
        lock (DebugSync)
        {
            Debug.WriteLine(message);
            Debug.Flush();
        }
    }

    /// <summary>
    /// Invoked to print the given message in the debug environment.
    /// </summary>
    [Conditional("DEBUG_DYNAMIC_LAMBDA")]
    internal static void ToDebug(ConsoleColor forecolor, string message)
    {
        lock (DebugSync)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            ToDebug(message);
            Console.ForegroundColor = oldfore;
        }
    }

    /// <summary>
    /// Invoked to print the given message in the debug environment.
    /// </summary>
    [Conditional("DEBUG_DYNAMIC_LAMBDA")]
    internal static void ToDebug(ConsoleColor forecolor, ConsoleColor backcolor, string message)
    {
        lock (DebugSync)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            ToDebug(message);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }
    }
}