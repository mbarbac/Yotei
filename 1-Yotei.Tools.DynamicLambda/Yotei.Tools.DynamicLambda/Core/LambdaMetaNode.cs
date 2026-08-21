namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// <inheritdoc cref="DynamicMetaObject"/>
/// </summary>
internal class LambdaMetaNode : DynamicMetaObject
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public LambdaMetaNode(
        DynamicMetaObject master,
        Expression expression,
        BindingRestrictions restrictions,
        LambdaNode node)
        : base(expression, restrictions, node)
    {
        LambdaMetaMaster = master.ThrowWhenNull();
        LambdaId = NextDLambdaId();
        LambdaParser.ToDebug(LambdaParser.NewMetaColor, $"- META new: {this}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"[Meta]#{LambdaId}({ValueAsNode.ToDebugString()})";

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public ulong LambdaId { get; }
    static ulong LastLambdaId = 0;

    internal static ulong NextDLambdaId() => Interlocked.Increment(ref LastLambdaId);

    /// <summary>
    /// The default meta object associated with this instance.
    /// </summary>
    public DynamicMetaObject LambdaMetaMaster { get; }

    /// <summary>
    /// The actual lambda node carried by this instance.
    /// </summary>
    public LambdaNode ValueAsNode => Value is LambdaNode node
        ? node
        : throw new InvalidOperationException(
            "This meta object carries no valid dynamic lambda node.")
            .WithData(Value);

    // ---------------------------------------------------- Overriden

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="arg"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder,
        DynamicMetaObject arg)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindBinary:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Operation: {binder.Operation}");

        var item = LambdaParser.Instance.ToLambdaNode(arg);
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Target: {item.ToDebugString()}");

        var node = new LambdaNodeBinary(ValueAsNode, binder.Operation, item);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackBinaryOperation(this, arg);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetDBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackBinaryOperation(this, arg, meta);

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    [SuppressMessage("", "IDE0300")]
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindConvert:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Type: {binder.Type.EasyName()}");

        var node = new LambdaNodeConvert(binder.Type, ValueAsNode);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackConvert(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        // Creating a compatible object to keep the ball rolling, and adding it to the surrogates
        // so that from that value we'll later find the original node. Otherwise, the indexes and
        // arguments received by other methods will just be the plain values, and not the convert
        // nodes...
        var ret = CreateCompatible(binder.ReturnType);
        if (ret != null) LambdaParser.Instance.Surrogates[ret] = node;

        var par = Expression.Variable(binder.ReturnType, "ret");
        var exp = Expression.Block(
            new ParameterExpression[] { par },
            Expression.Assign(par, Expression.Constant(ret, binder.ReturnType)));

        var rest = node.GetDBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(exp, rest, ret!),
            exp, rest, node);

        binder.FallbackConvert(this, meta);

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;

        // Invoked to create an object compatible with the given type. We have to intercept the
        // creation of 'string' because documentation says that 'GetUninitializedObject' does not
        // create uninitialized ones because 'because empty instances of immutable types serve no
        // purpose'. Whatever.
        object? CreateCompatible(Type type)
        {
            if (type.IsAssignableTo(typeof(LambdaNode))) return ValueAsNode;
            if (type == typeof(string)) return Guid.NewGuid().ToString();
            try
            {
                var r = RuntimeHelpers.GetUninitializedObject(type);
                if (r is not null) return r;
            }
            catch { }
            return new object();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="indexes"><inheritdoc/></param>
    /// <param name="value"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindSetIndex:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");

        var list = LambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Index: {temp.ToDebugString()}");

        var item = LambdaParser.Instance.ToLambdaNode(value);
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Value: {item.ToDebugString()}");

        var member = new LambdaNodeIndexed(ValueAsNode, list);
        var node = new LambdaNodeSetter(member, item);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackSetIndex(this, indexes, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetDBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetIndex(this, indexes, value, meta);

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="value"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindSetMember:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Name: {binder.Name}");

        var item = LambdaParser.Instance.ToLambdaNode(value);
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Value: {item.ToDebugString()}");

        var member = new LambdaNodeMember(ValueAsNode, binder.Name);
        var node = new LambdaNodeSetter(member, item);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackSetMember(this, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetDBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetMember(this, value, meta);

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindUnary:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Operation: {binder.Operation}");

        var node = new LambdaNodeUnary(binder.Operation, ValueAsNode);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackUnaryOperation(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetDBindingRestrictions(updateExpr);

        LambdaMetaNode meta;

        // Binding artifacts...
        if (binder.Operation is ExpressionType.IsTrue or ExpressionType.IsFalse)
        {
            // This is a choice: we will understand that dynamic nodes are logically false when
            // used as a boolean value, being 'false' the default boolean value itself...
            var obj = false;
            var objExpr = Expression.Constant(obj);

            meta = new LambdaMetaNode(
                new DynamicMetaObject(nodeExpr, rest, node),
                objExpr, rest, node);

            binder.FallbackUnaryOperation(this, meta);
        }

        // Standard case...
        else
        {
            meta = new LambdaMetaNode(
                new DynamicMetaObject(nodeExpr, rest, node),
                nodeExpr, rest, node);

            binder.FallbackUnaryOperation(this, meta);
        }

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    // ---------------------------------------------------- Delegated to underlying dynamic node...

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="indexes"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindGetIndex:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var index in indexes)
        {
            var item = LambdaParser.Instance.ToLambdaNode(index);
            LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Index: {item.ToDebugString()}");
        }

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindGetIndex(binder, indexes);
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindGetMember:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Member: {binder.Name}");

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindGetMember(binder);
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="args"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindInvoke:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = LambdaParser.Instance.ToLambdaNode(arg);
            LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindInvoke(binder, args);
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="args"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindInvokeMember(
        InvokeMemberBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindMethod:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = LambdaParser.Instance.ToLambdaNode(arg);
            LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Name: {binder.Name}");

        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindInvokeMember(binder, args);
        return meta;
    }

    // ---------------------------------------------------- Not supported

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="args"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindCreateInstance(
        CreateInstanceBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindCreateInstance:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Argument: {arg?.Value}");

        throw new NotSupportedException(
            "'BindCreateInstance' operations are not supported.")
            .WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="indexes"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindDeleteIndex(
        DeleteIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindDeleteIndex:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");
        foreach (var index in indexes)
            LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Index: {index}");

        throw new NotSupportedException(
            "'BindDeleteIndex' operations are not supported.")
            .WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
    {
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"* META BindDeleteMember:");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.ToDebug(LambdaParser.MetaBindedColor, $"- Member: {binder.Name}");

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}