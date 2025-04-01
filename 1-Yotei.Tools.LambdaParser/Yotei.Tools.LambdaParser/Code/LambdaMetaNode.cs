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

        LambdaHelpers.PrintMeta(this);
    }

    /// <inheritdoc/>
    public override string ToString() => $"[Meta#{LambdaId}]{ValueNode.ToDebugString()}";

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

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder,
        DynamicMetaObject arg)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindBinary:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Operation: {binder.Operation}");

        var item = LambdaParser.Instance.ToLambdaNode(arg);
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Target: {item.ToDebugString()}");

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

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindConvert:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Type: {binder.Type.EasyName()}");

        var node = new LambdaNodeConvert(binder.Type, ValueNode);
        LambdaParser.Instance.LastNode = node;

        binder.FallbackConvert(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        // Creating a compatible object to keep the ball rolling, and adding it to the surrogates
        // so that from that value we'll find later the original node...
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

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Result: {meta}");
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

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindSetIndex:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");

        var list = LambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Index: {temp.ToDebugString()}");

        var item = LambdaParser.Instance.ToLambdaNode(value);
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Value: {item.ToDebugString()}");

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

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindSetMember:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Name: {binder.Name}");

        var item = LambdaParser.Instance.ToLambdaNode(value);
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Value: {item.ToDebugString()}");

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

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindUnary:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Operation: {binder.Operation}");

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

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    // ---------------------------------------------------- Delegated to underlying node...

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindGetIndex:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");

        foreach (var index in indexes)
        {
            var item = LambdaParser.Instance.ToLambdaNode(index);
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Index: {item.ToDebugString()}");
        }

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindGetIndex(binder, indexes);
        return meta;
    }

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindGetMember:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Member: {binder.Name}");

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindGetMember(binder);
        return meta;
    }

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindInvoke:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = LambdaParser.Instance.ToLambdaNode(arg);
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindInvoke(binder, args);
        return meta;
    }

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindInvokeMember(
        InvokeMemberBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindMethod:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = LambdaParser.Instance.ToLambdaNode(arg);
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindInvokeMember(binder, args);
        return meta;
    }

    // ---------------------------------------------------- Not supported

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindCreateInstance(
        CreateInstanceBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindCreateInstance:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Argument: {arg?.Value}");

        throw new NotSupportedException(
            "'BindCreateInstance' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindDeleteIndex(
        DeleteIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindDeleteIndex:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        foreach (var index in indexes)
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Index: {index}");

        throw new NotSupportedException(
            "'BindDeleteIndex' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/> -----------------
    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindDeleteMember:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Member: {binder.Name}");

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}