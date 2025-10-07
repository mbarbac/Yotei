namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represent a node in a tree of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class LambdaNode : DynamicObject
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public LambdaNode()
    {
        LambdaId = NextLambdaId();
        LambdaVersion = NextLambdaVersion();
    }

    /// <summary>
    /// Obtains a debug string representation of this instance.
    /// </summary>
    /// <returns></returns>
    public string ToDebugString()
    {
        var name = GetType().EasyName();
        return $"[{name}]#{LambdaId}/{LambdaVersion}({ToString()})";
    }

    /// <summary>
    /// Returns the dynamic argument this instance is ultimately associated with, or null if any.
    /// </summary>
    /// <returns></returns>
    public abstract LambdaNodeArgument? GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public ulong LambdaId { get; }
    static ulong LastLambdaId = 0;

    internal static ulong NextLambdaId() => Interlocked.Increment(ref LastLambdaId);

    /// <summary>
    /// The current version of this instance.
    /// </summary>
    internal ulong LambdaVersion { get; set; }
    static ulong LastLambdaVersion = 0;

    internal static ulong NextLambdaVersion() => Interlocked.Increment(ref LastLambdaVersion);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override DynamicMetaObject GetMetaObject(Expression expression)
    {
        var master = base.GetMetaObject(expression);
        var rest = BindingRestrictions.GetInstanceRestriction(expression, this);
        var meta = new LambdaMetaNode(master, expression, rest, this);

        return meta;
    }

    /// <summary>
    /// Obtains binding restrictions that validate that the version of this instance is equal to
    /// the latest one and, if not, update it.
    /// <br/> The DLR caches the results it obtains using both the type of the call site and its
    /// arguments. For the purposes of <see cref="LambdaParser"/> this mechanism may render the
    /// same node over and over again, instead of a new binded one. So this method is a hack that
    /// intercepts the original mechanism by using a custom binding restriction that forces the
    /// cache to discard previous nodes and use the new binded one when needed.
    /// </summary>
    /// <param name="updateExpr"></param>
    /// <returns></returns>
    internal BindingRestrictions GetBindingRestrictions(Expression updateExpr)
    {
        var nodeExpr = Expression.Constant(this);
        var argExpr = Expression.Parameter(typeof(object));

        var condition = Expression.Block(
            new[] { argExpr },
            Expression.Assign(argExpr, nodeExpr),
            Expression.Condition(
                Expression.IsFalse(
                    Expression.Call(
                        Expression.Convert(argExpr, typeof(LambdaNode)),
                        ValidateLambdaVersionInfo)),
                Expression.Block(
                    Expression.Call(
                        Expression.Convert(argExpr, typeof(LambdaNode)),
                        UpdateLambdaVersionInfo),
                    updateExpr),
                Expression.Constant(true)));

        var rest = BindingRestrictions.GetExpressionRestriction(condition);
        return rest;
    }

    /// <summary>
    /// Flags to find the method info of the version-related methods.
    /// </summary>
    static readonly BindingFlags LAMBDA_FLAGS =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Determines if the version of this instance is the same as the latest one, or not.
    /// </summary>
    internal bool ValidateLambdaVersion()
    {
        var result = LambdaVersion == LastLambdaVersion;
        var valid = result ? "Valid" : "Invalid";

        LambdaParser.Print(ValidateLambdaColor, $"- VERSION {valid}: {ToDebugString()}");
        return result;
    }

    static MethodInfo ValidateLambdaVersionInfo
        => typeof(LambdaNode).GetMethod(nameof(ValidateLambdaVersion), LAMBDA_FLAGS)!;

    /// <summary>
    /// Updates the version of this instance to the lates one, which is also incremented.
    /// </summary>
    void UpdateLambdaVersion()
    {
        var old = LambdaVersion;
        var neo = LambdaVersion = NextLambdaVersion();

        LambdaParser.Print(UpdateLambdaColor,
            $"- VERSION Updating: {old} to {neo}, {ToDebugString()}");

        // Hack that permits to grab this instance even when the dynamic binding is not invoked.
        // This happens, for instance, the 2nd time a conversion is invoked.
        LambdaParser.Instance.LastNode = this;
    }

    static MethodInfo UpdateLambdaVersionInfo
        => typeof(LambdaNode).GetMethod(nameof(UpdateLambdaVersion), LAMBDA_FLAGS)!;

    // ---------------------------------------------------- Overriden

    /// <inheritdoc/>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        LambdaParser.Print(NodeBindedColor, $"* GetIndex:");
        LambdaParser.Print(NodeBindedColor, $"- This: {ToDebugString()}");

        var list = LambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            LambdaParser.Print(NodeBindedColor, $"- Index: {temp.ToDebugString()}");

        var node = new LambdaNodeIndexed(this, list);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaParser.Print(NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        LambdaParser.Print(NodeBindedColor, $"* GetMember:");
        LambdaParser.Print(NodeBindedColor, $"- This: {ToDebugString()}");
        LambdaParser.Print(NodeBindedColor, $"- Name: {binder.Name}");

        var node = new LambdaNodeMember(this, binder.Name);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaParser.Print(NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        LambdaParser.Print(NodeBindedColor, $"* Invoke:");
        LambdaParser.Print(NodeBindedColor, $"- This: {ToDebugString()}");

        var list = LambdaParser.Instance.ToLambdaNodes(args);
        foreach (var temp in list)
            LambdaParser.Print(NodeBindedColor, $"- Argument: {temp.ToDebugString()}");

        var node = new LambdaNodeInvoke(this, list);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaParser.Print(NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryInvokeMember(
        InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        LambdaParser.Print(NodeBindedColor, $"* Method:");
        LambdaParser.Print(NodeBindedColor, $"- This: {ToDebugString()}");
        LambdaParser.Print(NodeBindedColor, $"- Name: {binder.Name}");

        var list = LambdaParser.Instance.ToLambdaNodes(args);
        foreach (var temp in list)
            LambdaParser.Print(NodeBindedColor, $"- Argument: {temp.ToDebugString()}");

        LambdaNode node;

        // Intercepting 'Coalesce' methods...
        if (this is LambdaNodeArgument && binder.Name == "Coalesce" && list.Length == 2)
        {
            LambdaParser.Print(NodeBindedColor, $"* Intercepting 'Coalesce' method...");

            node = new LambdaNodeCoalesce(list[0], list[1]);
            LambdaParser.Instance.LastNode = node;
            result = node;
        }

        // Intercepting 'Ternary' methods...
        else if (this is LambdaNodeArgument && binder.Name == "Ternary" && list.Length == 3)
        {
            LambdaParser.Print(NodeBindedColor, $"* Intercepting 'Ternary' method...");

            node = new LambdaNodeTernary(list[0], list[1], list[2]);
            LambdaParser.Instance.LastNode = node;
            result = node;
        }

        // Regular methods...
        else
        {
            var types = Array.Empty<Type>();

            if (binder.GetType().Name == "CSharpInvokeMemberBinder") // Not public!
            {
                var flags = BindingFlags.Instance | BindingFlags.Public;
                var info = binder.GetType().GetProperty("TypeArguments", flags);
                types = (Type[])info!.GetValue(binder)!;
            }

            node = types.Length == 0
                ? new LambdaNodeMethod(this, binder.Name, list)
                : new LambdaNodeMethod(this, binder.Name, types, list);

            LambdaParser.Instance.LastNode = node;
            result = node;
        }

        // Finishing...
        LambdaParser.Print(NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    // ---------------------------------------------------- Intercepted by the meta node...

    /*public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result)*/
    /*public override bool TryConvert(ConvertBinder binder, out object? result)*/
    /*public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)*/
    /*public override bool TrySetMember(SetMemberBinder binder, object? value)*/
    /*public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)*/

    // ---------------------------------------------------- Not Supported...

    /*public override bool TryCreateInstance(CreateInstanceBinder binder, object?[]? args, [NotNullWhen(true)] out object? result)*/
    /*public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)*/
    /*public override bool TryDeleteMember(DeleteMemberBinder binder)*/

    // ----------------------------------------------------

    public const ConsoleColor NewNodeColor = ConsoleColor.White;
    public const ConsoleColor NewMetaColor = ConsoleColor.Gray;
    public const ConsoleColor NodeBindedColor = ConsoleColor.Yellow;
    public const ConsoleColor MetaBindedColor = ConsoleColor.Blue;
    public const ConsoleColor ValidateLambdaColor = ConsoleColor.Cyan;
    public const ConsoleColor UpdateLambdaColor = ConsoleColor.Magenta;

    // ----------------------------------------------------

    /// <summary>
    /// Validates that the given name can be used to named dynamic elements.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ValidateLambdaName(string name)
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
    /// Validates that the given collection of lambda nodes to be used as the arguments of methods
    /// ans indexers.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="canbeEmpty"></param>
    /// <returns></returns>
    public static IImmutableList<LambdaNode> ValidateLambdaArguments(
        IEnumerable<LambdaNode> args,
        bool canbeEmpty)
    {
        args.ThrowWhenNull();

        var list = args is IImmutableList<LambdaNode> temp ? temp : args.ToImmutableList();

        if (list.Count == 0 && !canbeEmpty) throw new ArgumentException(
            "Collection of arguments cannot be empty.");

        if (list.Count != 0 && list.Any(x => x is null)) throw new ArgumentException(
            "Collection of arguments carries null elements.")
            .WithData(args);

        return list;
    }

    /// <summary>
    /// Validates that the given collection of lambda nodes to be used as the type arguments of a
    /// method.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IImmutableList<Type> ValidateTypeArguments(IEnumerable<Type> args)
    {
        args.ThrowWhenNull();

        var list = args is IImmutableList<Type> temp ? temp : args.ToImmutableList();

        if (list.Count == 0) throw new EmptyException(
            "Collection of type arguments cannot be empty.");

        if (list.Any(x => x is null)) throw new ArgumentException(
            "Collection of type arguments carries null elements.")
            .WithData(args);

        return list;
    }
}