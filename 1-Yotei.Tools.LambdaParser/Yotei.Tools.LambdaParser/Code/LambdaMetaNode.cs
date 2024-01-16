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
        Expression expression, BindingRestrictions restrictions, LambdaNode node)
        : base(expression, restrictions, node)
    {
        LambdaMetaMaster = master.ThrowWhenNull();
        LambdaId = Interlocked.Increment(ref _LastLambdaId);

        LambdaParser.Print(this, $"- New: {this}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"Meta#{LambdaId}({LambdaNode.ToDebugString()})";

    /// <summary>
    /// The unique Id of this instance.
    /// </summary>
    public long LambdaId { get; }
    static long _LastLambdaId = 0;

    // <summary>
    /// The default meta object associated with this instance.
    /// </summary>
    public DynamicMetaObject LambdaMetaMaster { get; }

    /// <summary>
    /// The dynamic node carried by this instance.
    /// </summary>
    public LambdaNode LambdaNode => Value is LambdaNode node
        ? node
        : throw new InvalidOperationException(
            "This meta object carries no valid dynamic node.")
            .WithData(Value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetMember(
        SetMemberBinder binder, DynamicMetaObject value)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* BindSetMember:");
        LambdaParser.Print(this, $"- This: {this}");
        LambdaParser.Print(this, $"- Name: {binder.Name}");
        var item = LambdaParser.ToLambdaNode(value, parser);
        LambdaParser.Print(this, $"- Value: {item.ToDebugString()}");

        var member = new LambdaNodeMember(LambdaNode, binder.Name);
        var node = new LambdaNodeSetter(member, item);

        if (parser != null) parser.LastNode = node; // Needed!

        binder.FallbackSetMember(this, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetMember(this, value, meta);

        // Finishing...
        LambdaParser.Print(this, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* BindSetIndex:");
        LambdaParser.Print(this, $"- This: {this}");
        var list = LambdaParser.ToLambdaNodes(indexes, parser);
        foreach (var temp in list) LambdaParser.Print(this, $"- Index: {temp.ToDebugString()}");
        var item = LambdaParser.ToLambdaNode(value, parser);
        LambdaParser.Print(this, $"- Value: {item.ToDebugString()}");

        var member = new LambdaNodeIndexed(LambdaNode!, list);
        var node = new LambdaNodeSetter(member, item);

        if (parser != null) parser.LastNode = node; // Needed!

        binder.FallbackSetIndex(this, indexes, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetIndex(this, indexes, value, meta);

        // Finishing...
        LambdaParser.Print(this, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder, DynamicMetaObject arg)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* BindBinary:");
        LambdaParser.Print(this, $"- This: {this}");
        LambdaParser.Print(this, $"- Operation: {binder.Operation}");
        var item = LambdaParser.ToLambdaNode(arg, parser);
        LambdaParser.Print(this, $"- Target: {item.ToDebugString()}");

        var node = new LambdaNodeBinary(LambdaNode!, binder.Operation, item);

        binder.FallbackBinaryOperation(this, arg);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackBinaryOperation(this, arg, meta);

        // Finishing...
        LambdaParser.Print(this, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* BindUnary:");
        LambdaParser.Print(this, $"- This: {this}");
        LambdaParser.Print(this, $"- Operation: {binder.Operation}");

        var node = new LambdaNodeUnary(binder.Operation, LambdaNode!);

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

        // Finishing...
        LambdaParser.Print(this, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* BindConvert:");
        LambdaParser.Print(this, $"- This: {this}");
        LambdaParser.Print(this, $"- Type: {binder.Type.EasyName()}");

        var node = new LambdaNodeConvert(LambdaNode!, binder.Type);

        if (parser != null) parser.LastNode = node; // Needed!

        binder.FallbackConvert(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var ret = GetCompatible(binder.ReturnType, parser, node);
        var par = Expression.Variable(binder.ReturnType, "ret");
        var exp = Expression.Block(
            new ParameterExpression[] { par },
            Expression.Assign(par, Expression.Constant(ret, binder.ReturnType)));

        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new LambdaMetaNode(
            new DynamicMetaObject(exp, rest, ret!),
            exp, rest, node);

        binder.FallbackConvert(this, meta);

        LambdaParser.Print(this, $"- Result: {meta}");
        return meta;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to get a type-compatible object for conversion purposes. If the parser is not
    /// null, then the result is added to the Surrogates dictionary.
    /// </summary>
    object GetCompatible(Type type, LambdaParser? parser, LambdaNodeConvert surrogate)
    {
        var r = CreateCompatible(type);

        if (parser != null) parser.Surrogates[r] = surrogate;
        return r;
    }

    /// <summary>
    /// Invoked to create a compatible object, which will be used as the key of the Surrogates
    /// dictionary of a given parser.
    /// <para>
    /// The documentation mentions that <see cref="RuntimeHelpers.GetUninitializedObject(Type)"/>
    /// does not create uninitialized strings ('because empty instances of immutable types serve
    /// no purpose'). So, we need to intercept strings and provide unique ones.
    /// </para>
    /// </summary>
    object CreateCompatible(Type type)
    {
        if (type.IsAssignableTo(typeof(LambdaNode))) return LambdaNode;

        if (type == typeof(string)) return Guid.NewGuid().ToString();

        try
        {
            var r = RuntimeHelpers.GetUninitializedObject(type);
            if (r is not null) return r;
        }
        catch { }
        return new object();
    }

    // ----------------------------------------------------

    /// <inheritdoc/> DELEGATES OPERATION TO THE MASTER INSTANCE.
    public override DynamicMetaObject BindGetMember(
        GetMemberBinder binder)
    {
        LambdaParser.Print(this, $"* BindGetMember:");
        LambdaParser.Print(this, $"- This: {this}");

        var meta = LambdaMetaMaster.BindGetMember(binder);
        return meta;
    }

    /// <inheritdoc/> DELEGATES OPERATION TO THE MASTER INSTANCE.
    public override DynamicMetaObject BindGetIndex(
        GetIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        LambdaParser.Print(this, $"* BindGetIndex:");
        LambdaParser.Print(this, $"- This: {this}");

        var meta = LambdaMetaMaster.BindGetIndex(binder, indexes);
        return meta;
    }

    /// <inheritdoc/> DELEGATES OPERATION TO THE MASTER INSTANCE.
    public override DynamicMetaObject BindInvoke(
        InvokeBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.Print(this, $"* BindInvoke:");
        LambdaParser.Print(this, $"- This: {this}");

        var meta = LambdaMetaMaster.BindInvoke(binder, args);
        return meta;
    }

    /// <inheritdoc/> DELEGATES OPERATION TO THE MASTER INSTANCE.
    public override DynamicMetaObject BindInvokeMember(
        InvokeMemberBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.Print(this, $"* BindMethod:");
        LambdaParser.Print(this, $"- This: {this}");

        var meta = LambdaMetaMaster.BindInvokeMember(binder, args);
        return meta;
    }

    // ----------------------------------------------------

    /// <inheritdoc/> OPERATION NOT SUPPORTED.
    public override DynamicMetaObject BindCreateInstance(
        CreateInstanceBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.Print(this, $"* BindCreateInstance:");
        LambdaParser.Print(this, $"- This: {this}");

        throw new NotSupportedException(
            "'BindCreateInstance' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/> OPERATION NOT SUPPORTED.
    public override DynamicMetaObject BindDeleteIndex(
        DeleteIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        LambdaParser.Print(this, $"* BindDeleteIndex:");
        LambdaParser.Print(this, $"- This: {this}");

        throw new NotSupportedException(
            "'BindDeleteIndex' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/> OPERATION NOT SUPPORTED.
    public override DynamicMetaObject BindDeleteMember(
        DeleteMemberBinder binder)
    {
        LambdaParser.Print(this, $"* BindDeleteMember:");
        LambdaParser.Print(this, $"- This: {this}");

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}