namespace Yotei.Tools;

// ========================================================
/// <inheritdoc cref="DynamicMetaObject"/>
internal class LambdaMetaNode : DynamicMetaObject
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="master"></param>
    /// <param name="expression"></param>
    /// <param name="restrictions"></param>
    /// <param name="node"></param>
    public LambdaMetaNode(
        DynamicMetaObject master,
        Expression expression,
        BindingRestrictions restrictions,
        LambdaNode node)
        : base(expression, restrictions, node)
    {
        LambdaMetaMaster = master.ThrowWhenNull();
        LambdaId = NextLambdaId();

        LambdaParser.Print(LambdaNode.NewMetaColor, $"- META new: {this}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"[Meta]#{LambdaId}({ValueNode.ToDebugString()})";

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public ulong LambdaId { get; }
    static ulong LastLambdaId = 0;

    internal static ulong NextLambdaId() => Interlocked.Increment(ref LastLambdaId);

    /// <summary>
    /// The default meta object associated with this instance.
    /// </summary>
    public DynamicMetaObject LambdaMetaMaster { get; }

    /// <summary>
    /// The actual lambda node carried by this instance.
    /// </summary>
    public LambdaNode ValueNode => Value is LambdaNode node
        ? node
        : throw new InvalidOperationException(
            "This meta object carries no valid dynamic lambda node.")
            .WithData(Value);

    // ---------------------------------------------------- Overriden

    /// <inheritdoc/>
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder,
        DynamicMetaObject arg)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindBinary:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Operation: {binder.Operation}");

        var item = LambdaParser.Instance.ToLambdaNode(arg);
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Target: {item.ToDebugString()}");

        var node = new LambdaNodeBinary(ValueNode, binder.Operation, item);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackBinaryOperation(this, arg);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackBinaryOperation(this, arg, meta);

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindConvert:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Type: {binder.Type.EasyName()}");

        var node = new LambdaNodeConvert(binder.Type, ValueNode);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackConvert(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        // Creating a compatible object to keep the ball rolling, and adding it to the surrogates
        // so that from that value we'll find later the original node. Otherwise, the indexes and
        // arguments received by other methods will just be the plain values, and not the convert
        // nodes...
        var ret = CreateCompatible(binder.ReturnType);
        if (ret != null) LambdaParser.Instance.Surrogates[ret] = node;

        var par = Expression.Variable(binder.ReturnType, "ret");
        var exp = Expression.Block(
            new ParameterExpression[] { par },
            Expression.Assign(par, Expression.Constant(ret, binder.ReturnType)));

        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(exp, rest, ret!),
            exp, rest, node);

        binder.FallbackConvert(this, meta);

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Result: {meta}");
        return meta;

        // Invoked to create an object compatible with the given type. We have to intercept the
        // creation of 'string' because documentation says that 'GetUninitializedObject' does not
        // create uninitialized ones because 'because empty instances of immutable types serve no
        // purpose'...
        object? CreateCompatible(Type type)
        {
            if (type.IsAssignableTo(typeof(LambdaNode))) return ValueNode;
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

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindSetIndex:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");

        var list = LambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Index: {temp.ToDebugString()}");

        var item = LambdaParser.Instance.ToLambdaNode(value);
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Value: {item.ToDebugString()}");

        var member = new LambdaNodeIndexed(ValueNode, list);
        var node = new LambdaNodeSetter(member, item);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackSetIndex(this, indexes, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetIndex(this, indexes, value, meta);

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindSetMember:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Name: {binder.Name}");

        var item = LambdaParser.Instance.ToLambdaNode(value);
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Value: {item.ToDebugString()}");

        var member = new LambdaNodeMember(ValueNode, binder.Name);
        var node = new LambdaNodeSetter(member, item);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackSetMember(this, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetMember(this, value, meta);

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindUnary:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Operation: {binder.Operation}");

        var node = new LambdaNodeUnary(binder.Operation, ValueNode);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackUnaryOperation(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);

        LambdaMetaNode meta;

        // Binding artifacts...
        if (binder.Operation is ExpressionType.IsTrue or ExpressionType.IsFalse)
        {
            // This is a choice, by which we understand that dynamic nodes are logically false
            // when used as a boolean value, being 'false' the default boolean value itself...
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

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    // ---------------------------------------------------- Delegated to underlying node...

    /// <inheritdoc/>
    public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindGetIndex:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");

        foreach (var index in indexes)
        {
            var item = LambdaParser.Instance.ToLambdaNode(index);
            LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Index: {item.ToDebugString()}");
        }

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindGetIndex(binder, indexes);
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindGetMember:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Member: {binder.Name}");

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindGetMember(binder);
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindInvoke:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = LambdaParser.Instance.ToLambdaNode(arg);
            LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindInvoke(binder, args);
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindInvokeMember(
        InvokeMemberBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindMethod:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = LambdaParser.Instance.ToLambdaNode(arg);
            LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }

        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindInvokeMember(binder, args);
        return meta;
    }

    // ---------------------------------------------------- Not supported

    /// <inheritdoc/>
    public override DynamicMetaObject BindCreateInstance(
        CreateInstanceBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindCreateInstance:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Argument: {arg?.Value}");

        throw new NotSupportedException(
            "'BindCreateInstance' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindDeleteIndex(
        DeleteIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindDeleteIndex:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");
        foreach (var index in indexes)
            LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Index: {index}");

        throw new NotSupportedException(
            "'BindDeleteIndex' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
    {
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"* META BindDeleteMember:");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaNode.MetaBindedColor, $"- Member: {binder.Name}");

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}