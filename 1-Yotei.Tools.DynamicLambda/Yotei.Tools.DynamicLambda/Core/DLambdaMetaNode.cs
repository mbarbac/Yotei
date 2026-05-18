namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// <inheritdoc cref="DynamicMetaObject"/>
/// </summary>
internal class DLambdaMetaNode : DynamicMetaObject
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DLambdaMetaNode(
        DynamicMetaObject master,
        Expression expression,
        BindingRestrictions restrictions,
        DLambdaNode node)
        : base(expression, restrictions, node)
    {
        DLambdaMetaMaster = master.ThrowWhenNull();
        DLambdaId = NextDLambdaId();
        DLambdaParser.ToDebug(DLambdaParser.NewMetaColor, $"- META new: {this}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"[Meta]#{DLambdaId}({ValueAsNode.ToDebugString()})";

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public ulong DLambdaId { get; }
    static ulong DLastLambdaId = 0;

    internal static ulong NextDLambdaId() => Interlocked.Increment(ref DLastLambdaId);

    /// <summary>
    /// The default meta object associated with this instance.
    /// </summary>
    public DynamicMetaObject DLambdaMetaMaster { get; }

    /// <summary>
    /// The actual lambda node carried by this instance.
    /// </summary>
    public DLambdaNode ValueAsNode => Value is DLambdaNode node
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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindBinary:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Operation: {binder.Operation}");

        var item = DLambdaParser.Instance.ToLambdaNode(arg);
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Target: {item.ToDebugString()}");

        var node = new DLambdaNodeBinary(ValueAsNode, binder.Operation, item);
        DLambdaParser.Instance.LastNode = node;

        binder.FallbackBinaryOperation(this, arg);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetDBindingRestrictions(updateExpr);
        var meta = new DLambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackBinaryOperation(this, arg, meta);

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Result: {meta}");
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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindConvert:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Type: {binder.Type.EasyName()}");

        var node = new DLambdaNodeConvert(binder.Type, ValueAsNode);
        DLambdaParser.Instance.LastNode = node;

        binder.FallbackConvert(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        // Creating a compatible object to keep the ball rolling, and adding it to the surrogates
        // so that from that value we'll later find the original node. Otherwise, the indexes and
        // arguments received by other methods will just be the plain values, and not the convert
        // nodes...
        var ret = CreateCompatible(binder.ReturnType);
        if (ret != null) DLambdaParser.Instance.Surrogates[ret] = node;

        var par = Expression.Variable(binder.ReturnType, "ret");
        var exp = Expression.Block(
            new ParameterExpression[] { par },
            Expression.Assign(par, Expression.Constant(ret, binder.ReturnType)));

        var rest = node.GetDBindingRestrictions(updateExpr);
        var meta = new DLambdaMetaNode(
            new DynamicMetaObject(exp, rest, ret!),
            exp, rest, node);

        binder.FallbackConvert(this, meta);

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;

        // Invoked to create an object compatible with the given type. We have to intercept the
        // creation of 'string' because documentation says that 'GetUninitializedObject' does not
        // create uninitialized ones because 'because empty instances of immutable types serve no
        // purpose'. Whatever.
        object? CreateCompatible(Type type)
        {
            if (type.IsAssignableTo(typeof(DLambdaNode))) return ValueAsNode;
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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindSetIndex:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");

        var list = DLambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Index: {temp.ToDebugString()}");

        var item = DLambdaParser.Instance.ToLambdaNode(value);
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Value: {item.ToDebugString()}");

        var member = new DLambdaNodeIndexed(ValueAsNode, list);
        var node = new DLambdaNodeSetter(member, item);
        DLambdaParser.Instance.LastNode = node;

        binder.FallbackSetIndex(this, indexes, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetDBindingRestrictions(updateExpr);
        var meta = new DLambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetIndex(this, indexes, value, meta);

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Result: {meta}");
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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindSetMember:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Name: {binder.Name}");

        var item = DLambdaParser.Instance.ToLambdaNode(value);
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Value: {item.ToDebugString()}");

        var member = new DLambdaNodeMember(ValueAsNode, binder.Name);
        var node = new DLambdaNodeSetter(member, item);
        DLambdaParser.Instance.LastNode = node;

        binder.FallbackSetMember(this, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetDBindingRestrictions(updateExpr);
        var meta = new DLambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetMember(this, value, meta);

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindUnary:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Operation: {binder.Operation}");

        var node = new DLambdaNodeUnary(binder.Operation, ValueAsNode);
        DLambdaParser.Instance.LastNode = node;

        binder.FallbackUnaryOperation(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetDBindingRestrictions(updateExpr);

        DLambdaMetaNode meta;

        // Binding artifacts...
        if (binder.Operation is ExpressionType.IsTrue or ExpressionType.IsFalse)
        {
            // This is a choice: we will understand that dynamic nodes are logically false when
            // used as a boolean value, being 'false' the default boolean value itself...
            var obj = false;
            var objExpr = Expression.Constant(obj);

            meta = new DLambdaMetaNode(
                new DynamicMetaObject(nodeExpr, rest, node),
                objExpr, rest, node);

            binder.FallbackUnaryOperation(this, meta);
        }

        // Standard case...
        else
        {
            meta = new DLambdaMetaNode(
                new DynamicMetaObject(nodeExpr, rest, node),
                nodeExpr, rest, node);

            binder.FallbackUnaryOperation(this, meta);
        }

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Result: {meta}");
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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindGetIndex:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var index in indexes)
        {
            var item = DLambdaParser.Instance.ToLambdaNode(index);
            DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Index: {item.ToDebugString()}");
        }

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Delegated...");
        var meta = DLambdaMetaMaster.BindGetIndex(binder, indexes);
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindGetMember:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Member: {binder.Name}");

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Delegated...");
        var meta = DLambdaMetaMaster.BindGetMember(binder);
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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindInvoke:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = DLambdaParser.Instance.ToLambdaNode(arg);
            DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Delegated...");
        var meta = DLambdaMetaMaster.BindInvoke(binder, args);
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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindMethod:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = DLambdaParser.Instance.ToLambdaNode(arg);
            DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Name: {binder.Name}");

        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Delegated...");
        var meta = DLambdaMetaMaster.BindInvokeMember(binder, args);
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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindCreateInstance:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Argument: {arg?.Value}");

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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindDeleteIndex:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");
        foreach (var index in indexes)
            DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Index: {index}");

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
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"* META BindDeleteMember:");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- This: {this}");
        DLambdaParser.ToDebug(DLambdaParser.MetaBindedColor, $"- Member: {binder.Name}");

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}