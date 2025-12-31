#pragma warning disable IDE0300

namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// <inheritdoc cref="DynamicMetaObject"/>
/// </summary>
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
        LambdaParser.Print(LambdaParser.NewMetaColor, $"- META new: {this}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindBinary:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Operation: {binder.Operation}");

        var item = LambdaParser.Instance.ToLambdaNode(arg);
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Target: {item.ToDebugString()}");

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

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        // HIGH: EasyName related
        //LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindConvert:");
        //LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        //LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Type: {binder.Type.EasyName()}");

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

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindSetIndex:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        var list = LambdaParser.Instance.ToLambdaNodes(indexes);
        foreach (var temp in list)
            LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Index: {temp.ToDebugString()}");

        var item = LambdaParser.Instance.ToLambdaNode(value);
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Value: {item.ToDebugString()}");

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

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindSetMember:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Name: {binder.Name}");

        var item = LambdaParser.Instance.ToLambdaNode(value);
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Value: {item.ToDebugString()}");

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

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindUnary:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Operation: {binder.Operation}");

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

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    // ---------------------------------------------------- Delegated to underlying node...

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"><inheritdoc/></param>
    /// <param name="indexes"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindGetIndex:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var index in indexes)
        {
            var item = LambdaParser.Instance.ToLambdaNode(index);
            LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Index: {item.ToDebugString()}");
        }

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Delegated...");
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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindGetMember:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Member: {binder.Name}");

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Delegated...");
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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindInvoke:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = LambdaParser.Instance.ToLambdaNode(arg);
            LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Delegated...");
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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindMethod:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        foreach (var arg in args)
        {
            var item = LambdaParser.Instance.ToLambdaNode(arg);
            LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Argument: {item.ToDebugString()}");
        }

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Delegated...");
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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindCreateInstance:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Argument: {arg?.Value}");

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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindDeleteIndex:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        foreach (var index in indexes)
            LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Index: {index}");

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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* META BindDeleteMember:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Member: {binder.Name}");

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}