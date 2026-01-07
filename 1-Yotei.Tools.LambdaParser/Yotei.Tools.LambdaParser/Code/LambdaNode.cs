#pragma warning disable IDE0300

namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a node in a chain of dynamic operations. Instances of this type are used to specify
/// the arbitrary logic that instances represent, so that they can be combined to represent a
/// chain of dynamic operations.
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
    /// Obtains a debug representation of this instance.
    /// </summary>
    /// <returns></returns>
    public string ToDebugString()
    {
        // HIGH: EasyName related...
        //var name = GetType().EasyName();
        //return $"[{name}]#{LambdaId}/{LambdaVersion}({ToString()})";
        throw null;
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="expression"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
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
    /// <para>
    /// The DLR caches the results it obtains using both the type of the call site and the type
    /// of the arguments used. For the purposes of <see cref="LambdaParser"/> this mechanism will
    /// render the same nodes over and over again, instead of new binded ones. So, this method
    /// is a hack that intercepts the original mechanism by using a custom binding restriction
    /// that forces the cache to discard previous nodes and use new binded ones.
    /// </para>
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

        LambdaParser.Print(
            LambdaParser.ValidateLambdaColor,
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

        LambdaParser.Print(
            LambdaParser.UpdateLambdaColor,
            $"- VERSION Updating: {old} to {neo}, {ToDebugString()}");

        // This permits to grab this instance even when the dynamic binding is not invoked.
        // Which happens, for instance, the 2nd time a conversion is invoked.
        LambdaParser.Instance.LastNode = this;
    }

    static MethodInfo UpdateLambdaVersionInfo
        => typeof(LambdaNode).GetMethod(nameof(UpdateLambdaVersion), LAMBDA_FLAGS)!;

    // ---------------------------------------------------- Overriden

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="indexes"><inheritdoc/></param>
    /// <param name="result"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"* GetIndex:");
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- This: {ToDebugString()}");

        var list = LambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Index: {temp.ToDebugString()}");

        var node = new LambdaNodeIndexed(this, list);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="result"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"* GetMember:");
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- This: {ToDebugString()}");
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Name: {binder.Name}");

        var node = new LambdaNodeMember(this, binder.Name);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="args"><inheritdoc/></param>
    /// <param name="result"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"* Invoke:");
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- This: {ToDebugString()}");

        var list = LambdaParser.Instance.ToLambdaNodes(args);
        foreach (var temp in list)
            LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Argument: {temp.ToDebugString()}");

        var node = new LambdaNodeInvoke(this, list);
        LambdaParser.Instance.LastNode = node;
        result = node;

        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="args"><inheritdoc/></param>
    /// <param name="result"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override bool TryInvokeMember(
        InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"* Method:");
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- This: {ToDebugString()}");
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Name: {binder.Name}");

        var list = LambdaParser.Instance.ToLambdaNodes(args);
        foreach (var temp in list)
            LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Argument: {temp.ToDebugString()}");

        LambdaNode node;

        // Intercepting 'Coalesce' methods...
        if (this is LambdaNodeArgument && binder.Name == "Coalesce" && list.Length == 2)
        {
            LambdaParser.Print(LambdaParser.NodeBindedColor, $"* Intercepting 'Coalesce' method...");

            node = new LambdaNodeCoalesce(list[0], list[1]);
            LambdaParser.Instance.LastNode = node;
            result = node;
        }

        // Intercepting 'Ternary' methods...
        else if (this is LambdaNodeArgument && binder.Name == "Ternary" && list.Length == 3)
        {
            LambdaParser.Print(LambdaParser.NodeBindedColor, $"* Intercepting 'Ternary' method...");

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
        LambdaParser.Print(LambdaParser.NodeBindedColor, $"- Result: {node.ToDebugString()}");
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