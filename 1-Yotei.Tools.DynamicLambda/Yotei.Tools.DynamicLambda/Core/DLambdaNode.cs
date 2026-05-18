namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a node in a chain of dynamic operations bound to a given <see langword="dynamic"/>
/// argument. Instances of this type are used to describe the arbitrary generic logic used in a
/// given context.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class DLambdaNode : DynamicObject
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DLambdaNode()
    {
        DLambdaId = NextDLambdaId();
        DLambdaVersion = NextDLambdaVersion();
    }

    /// <summary>
    /// Obtains a string representation of this instance for debug purposes.
    /// </summary>
    /// <returns></returns>
    public string ToDebugString()
    {
        var type = GetType().EasyName();
        return $"[{type}]#{DLambdaId}/{DLambdaVersion}({ToString()})";
    }

    /// <summary>
    /// Returns the dynamic argument this instance is ultimately bound to, or <see langword="null"/>
    /// if it cannot be determined.
    /// </summary>
    /// <returns></returns>
    public abstract DLambdaNodeArgument? GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public ulong DLambdaId { get; }
    static ulong DLastLambdaId = 0;

    internal static ulong NextDLambdaId() => Interlocked.Increment(ref DLastLambdaId);

    /// <summary>
    /// Maintains the current version of this instance, which is used to prevent the DLR to cache
    /// old instances instead of generatic new ones for new bindings.
    /// </summary>
    internal ulong DLambdaVersion { get; set; }
    static ulong DLastLambdaVersion = 0;

    internal static ulong NextDLambdaVersion() => Interlocked.Increment(ref DLastLambdaVersion);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="expression"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject GetMetaObject(Expression expression)
    {
        var master = base.GetMetaObject(expression);
        var rest = BindingRestrictions.GetInstanceRestriction(expression, this);
        var meta = new DLambdaMetaNode(master, expression, rest, this);

        return meta;
    }

    /// <summary>
    /// Obtains binding restrictions that validate that the version of this instance is equal to
    /// the latest one and, if not, update it.
    /// <para>
    /// For performance reasons, the DLR caches the results of the bindings using both the type of
    /// the call site and the type of the arguments used. For <see cref="DLambdaParser"/> purposes,
    /// this mechanism will produce the same nodes over and over again, instead of producing new
    /// ones each binding, which is what we one.
    /// <br/> So, this method is a hack that intercepts the DLR mechanism by using a custom binding
    /// restriction that forces the cache to discard previous nodes (as far as the internal version
    /// has changed) and use new binded ones.
    /// </para>
    /// </summary>
    /// <param name="updateExpr"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0300")]
    internal BindingRestrictions GetDBindingRestrictions(Expression updateExpr)
    {
        var nodeExpr = Expression.Constant(this);
        var argExpr = Expression.Parameter(typeof(object));

        var condition = Expression.Block(
            new[] { argExpr },
            Expression.Assign(argExpr, nodeExpr),
            Expression.Condition(
                Expression.IsFalse(
                    Expression.Call(
                        Expression.Convert(argExpr, typeof(DLambdaNode)),
                        ValidateDLambdaVersionInfo)),
                Expression.Block(
                    Expression.Call(
                        Expression.Convert(argExpr, typeof(DLambdaNode)),
                        UpdateDLambdaVersionInfo),
                    updateExpr),
                Expression.Constant(true)));

        var rest = BindingRestrictions.GetExpressionRestriction(condition);
        return rest;
    }

    /// <summary>
    /// Flags to find the method info of the version-related methods.
    /// </summary>
    static readonly BindingFlags DLAMBDA_FLAGS =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Determines if the version of this instance is the same as the latest one, or not. This is
    /// used to signal the DLR to use a fresh new instance to bind the current operation, instead
    /// of using a cached one.
    /// </summary>
    internal bool ValidateDLambdaVersion()
    {
        var result = DLambdaVersion == DLastLambdaVersion;
        var valid = result ? "Valid" : "Invalid";

        DLambdaParser.ToDebug(
            DLambdaParser.ValidateLambdaColor,
            $"- VERSION {valid}: {ToDebugString()}");

        return result;
    }

    static MethodInfo ValidateDLambdaVersionInfo
        => typeof(DLambdaNode).GetMethod(nameof(ValidateDLambdaVersion), DLAMBDA_FLAGS)!;

    /// <summary>
    /// Updates the version of this instance to the latest one, which is also incremented along
    /// the way. Note: setting the last node to this permits to grab this insatnce even when the
    /// dynamic binding is not invoked - for instance: it happens the 2nd time a conversion is
    /// invoked.
    /// </summary>
    void UpdateDLambdaVersion()
    {
        var old = DLambdaVersion;
        var neo = DLambdaVersion = NextDLambdaVersion();

        DLambdaParser.ToDebug(
            DLambdaParser.UpdateLambdaColor,
            $"- VERSION Updating: {old} to {neo}, {ToDebugString()}");

        DLambdaParser.Instance.LastNode = this;
    }

    static MethodInfo UpdateDLambdaVersionInfo
        => typeof(DLambdaNode).GetMethod(nameof(UpdateDLambdaVersion), DLAMBDA_FLAGS)!;

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
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"* GetIndex:");
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- This: {ToDebugString()}");

        var list = DLambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Index: {temp.ToDebugString()}");

        var node = new DLambdaNodeIndexed(this, list);
        DLambdaParser.Instance.LastNode = node;
        result = node;

        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Result: {node.ToDebugString()}");
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
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"* GetMember:");
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- This: {ToDebugString()}");
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Name: {binder.Name}");

        var node = new DLambdaNodeMember(this, binder.Name);
        DLambdaParser.Instance.LastNode = node;
        result = node;

        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Result: {node.ToDebugString()}");
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
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"* Invoke:");
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- This: {ToDebugString()}");

        var list = DLambdaParser.Instance.ToLambdaNodes(args);
        foreach (var temp in list)
            DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Argument: {temp.ToDebugString()}");

        var node = new DLambdaNodeInvoke(this, list);
        DLambdaParser.Instance.LastNode = node;
        result = node;

        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Result: {node.ToDebugString()}");
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
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"* Method:");
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- This: {ToDebugString()}");
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Name: {binder.Name}");

        var list = DLambdaParser.Instance.ToLambdaNodes(args);
        foreach (var temp in list)
            DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Argument: {temp.ToDebugString()}");

        DLambdaNode node;

        // Intercepting 'Coalesce' methods...
        if (this is DLambdaNodeArgument && binder.Name == "Coalesce" && list.Length == 2)
        {
            DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"* Intercepting 'Coalesce' method...");

            node = new DLambdaNodeCoalesce(list[0], list[1]);
            DLambdaParser.Instance.LastNode = node;
            result = node;
        }

        // Intercepting 'Ternary' methods...
        else if (this is DLambdaNodeArgument && binder.Name == "Ternary" && list.Length == 3)
        {
            DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"* Intercepting 'Ternary' method...");

            node = new DLambdaNodeTernary(list[0], list[1], list[2]);
            DLambdaParser.Instance.LastNode = node;
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
                ? new DLambdaNodeMethod(this, binder.Name, list)
                : new DLambdaNodeMethod(this, binder.Name, types, list);

            DLambdaParser.Instance.LastNode = node;
            result = node;
        }

        // Finishing...
        DLambdaParser.ToDebug(DLambdaParser.NodeBindedColor, $"- Result: {node.ToDebugString()}");
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