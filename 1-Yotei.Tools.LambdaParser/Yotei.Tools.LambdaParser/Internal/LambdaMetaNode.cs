namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="DynamicMetaObject"/>.
/// <br/> This is an internal type not intended for public usage.
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
        LambdaId = Interlocked.Increment(ref LastLambdaId);

        LambdaParser.Print(LambdaParser.NewMetaColor, $"- New Meta: {this}");
    }
    
    /// <inheritdoc/>
    public override string ToString() => $"Meta#{LambdaId}({LambdaNode.ToDebugString()})";

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public long LambdaId { get; }
    static long LastLambdaId = 0;

    /// <summary>
    /// The default meta object associated with this instance.
    /// </summary>
    public DynamicMetaObject LambdaMetaMaster { get; }

    /// <summary>
    /// The actual node carried by this instance.
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

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindSetMember:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Name: {binder.Name}");
        var item = LambdaParser.ToLambdaNode(value, parser);
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Value: {item.ToDebugString()}");

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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindSetIndex:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        var list = LambdaParser.ToLambdaNodes(indexes, parser);
        foreach (var temp in list) LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Index: {temp.ToDebugString()}");
        var item = LambdaParser.ToLambdaNode(value, parser);
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Value: {item.ToDebugString()}");

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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder, DynamicMetaObject arg)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindBinary:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Operation: {binder.Operation}");
        var item = LambdaParser.ToLambdaNode(arg, parser);
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Target: {item.ToDebugString()}");

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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindUnary:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Operation: {binder.Operation}");

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
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    [SuppressMessage("", "IDE0300")]
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindConvert:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Type: {binder.Type.EasyName()}");

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

        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- Result: {meta}");
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

    /// <inheritdoc/>
    /// <remarks>
    /// DELEGATES OPERATION TO MASTER INSTANCE.
    /// </remarks>
    public override DynamicMetaObject BindGetMember(
        GetMemberBinder binder)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindGetMember:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        var meta = LambdaMetaMaster.BindGetMember(binder);
        return meta;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// DELEGATES OPERATION TO MASTER INSTANCE.
    /// </remarks>
    public override DynamicMetaObject BindGetIndex(
        GetIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindGetIndex:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        var meta = LambdaMetaMaster.BindGetIndex(binder, indexes);
        return meta;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// DELEGATES OPERATION TO MASTER INSTANCE.
    /// </remarks>
    public override DynamicMetaObject BindInvoke(
        InvokeBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindInvoke:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        var meta = LambdaMetaMaster.BindInvoke(binder, args);
        return meta;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// DELEGATES OPERATION TO MASTER INSTANCE.
    /// </remarks>
    public override DynamicMetaObject BindInvokeMember(
        InvokeMemberBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindMethod:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        var meta = LambdaMetaMaster.BindInvokeMember(binder, args);
        return meta;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// OPERATION NOT SUPPORTED.
    /// </remarks>
    public override DynamicMetaObject BindCreateInstance(
        CreateInstanceBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindCreateInstance:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        throw new NotSupportedException(
            "'BindCreateInstance' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// OPERATION NOT SUPPORTED.
    /// </remarks>
    public override DynamicMetaObject BindDeleteIndex(
        DeleteIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindDeleteIndex:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        throw new NotSupportedException(
            "'BindDeleteIndex' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// OPERATION NOT SUPPORTED.
    /// </remarks>
    public override DynamicMetaObject BindDeleteMember(
        DeleteMemberBinder binder)
    {
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"* BindDeleteMember:");
        LambdaParser.Print(LambdaParser.MetaBindedColor, $"- This: {this}");

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}