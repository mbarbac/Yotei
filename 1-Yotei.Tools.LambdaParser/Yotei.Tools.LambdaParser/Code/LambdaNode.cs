namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a node in a chain of dynamic operations.
/// </summary>
public abstract class LambdaNode : DynamicObject, ICloneable
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public LambdaNode()
    {
        LambdaId = NextLambdaId();
        LambdaVersion = NextLambdaVersion();
    }

    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract LambdaNode Clone();
    object ICloneable.Clone() => Clone();

    /// <inheritdoc/>
    public override string ToString() => nameof(LambdaNode);

    /// <summary>
    /// Returns a string representation of this instance suitable for debug purposes.
    /// </summary>
    /// <returns></returns>
    public string ToDebugString()
    {
        var type = GetType().EasyName();
        return $"[{type}#{LambdaId}]({ToString()}) [Vs:{LambdaVersion}]";
    }

    /// <summary>
    /// Returns the dynamic argument this instance is ultimately associated with, or <c>null</c>
    /// if this information cannot be obtained.
    /// </summary>
    /// <returns></returns>
    public abstract LambdaNodeArgument? GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public long LambdaId { get; }
    static long LastLambdaId = 0;

    /// <summary>
    /// Gets the next available id.
    /// </summary>
    /// <returns></returns>
    public static long NextLambdaId() => Interlocked.Increment(ref LastLambdaId);

    /// <summary>
    /// The current version of this instance.
    /// </summary>
    internal long LambdaVersion { get; set; }
    static long LastLambdaVersion = 0;

    /// <summary>
    /// Gets the next available lambda version.
    /// </summary>
    /// <returns></returns>
    internal static long NextLambdaVersion() => Interlocked.Increment(ref LastLambdaVersion);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override DynamicMetaObject GetMetaObject(Expression expression)
    {
        var master = base.GetMetaObject(expression);
        var rest = BindingRestrictions.GetInstanceRestriction(expression, this);
        var meta = new LambdaMetaNode(master, expression, rest, this);

        return meta;
    }

    // ----------------------------------------------------

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

        LambdaHelpers.Print(
            LambdaHelpers.ValidateLambdaColor,
            $"- VERSION {valid}: {ToDebugString()}");

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

        LambdaHelpers.Print(
            LambdaHelpers.ValidateLambdaColor,
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
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"* GetIndex:");
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- This: {ToDebugString()}");

        var list = LambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Index: {temp.ToDebugString()}");

        var node = new LambdaNodeIndexed(this, list);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"* GetMember:");
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- This: {ToDebugString()}");
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Name: {binder.Name}");

        var node = new LambdaNodeMember(this, binder.Name);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"* Invoke:");
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- This: {ToDebugString()}");

        var list = LambdaParser.Instance.ToLambdaNodes(args);
        foreach (var temp in list)
            LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Argument: {temp.ToDebugString()}");

        var node = new LambdaNodeInvoke(this, list);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryInvokeMember(
        InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"* Method:");
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- This: {ToDebugString()}");
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Name: {binder.Name}");

        var list = LambdaParser.Instance.ToLambdaNodes(args);
        foreach (var temp in list)
            LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Argument: {temp.ToDebugString()}");

        LambdaNode node;

        // Intercepting 'Coalesce' methods...
        if (this is LambdaNodeArgument && binder.Name == "Coalesce" && list.Length == 2)
        {
            LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"* Intercepting 'Coalesce' method...");

            node = new LambdaNodeCoalesce(list[0], list[1]);
            LambdaParser.Instance.LastNode = node;
            result = node;
        }

        // Intercepting 'Ternary' methods...
        else if (this is LambdaNodeArgument && binder.Name == "Ternary" && list.Length == 3)
        {
            LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"* Intercepting 'Ternary' method...");

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
        LambdaHelpers.Print(LambdaHelpers.NodeBindedColor, $"- Result: {node.ToDebugString()}");
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
}