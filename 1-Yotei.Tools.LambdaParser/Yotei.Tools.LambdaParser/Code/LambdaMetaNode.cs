namespace Yotei.Tools;

// ========================================================
/// <inheritdoc cref="DynamicMetaObject"/>
internal class LambdaMetaNode : DynamicMetaObject
{
    public LambdaMetaNode(
        DynamicMetaObject master,
        Expression expression,
        BindingRestrictions restrictions,
        LambdaNode node)
        : base(expression, restrictions, node)
    {
        LambdaMetaMaster = master.ThrowWhenNull();
        LambdaId = Interlocked.Increment(ref LastLambdaId);

        LambdaDebug.Print(LambdaDebug.NewMetaColor, $"- New Meta: {this}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"[Meta#{LambdaId}]{LambdaNode.ToDebugString()}";

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
    /// The actual lambda node carried by this instance.
    /// </summary>
    public LambdaNode LambdaNode => Value is LambdaNode node
        ? node
        : throw new InvalidOperationException(
            "This meta object carries no valid dynamic lambda node.")
            .WithData(Value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetMember(
        SetMemberBinder binder, DynamicMetaObject value)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindSetMember:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Name: {binder.Name}");
        var item = LambdaParser.ToLambdaNode(value, parser);
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Value: {item.ToDebugString()}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindSetIndex:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        var list = LambdaParser.ToLambdaNodes(indexes, parser);
        foreach (var temp in list) LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Index: {temp.ToDebugString()}");
        var item = LambdaParser.ToLambdaNode(value, parser);
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Value: {item.ToDebugString()}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder, DynamicMetaObject arg)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindBinary:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Operation: {binder.Operation}");
        var item = LambdaParser.ToLambdaNode(arg, parser);
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Target: {item.ToDebugString()}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindUnary:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Operation: {binder.Operation}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Result: {meta}");
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindConvert:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Type: {binder.Type.EasyName()}");

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

        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Result: {meta}");
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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindGetMember:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Member: {binder.Name}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindGetIndex:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        foreach (var index in indexes)
            LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Index: {index}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindInvoke:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Argument: {arg}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindMethod:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Argument: {arg}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindCreateInstance:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Argument: {arg}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindDeleteIndex:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        foreach (var index in indexes)
            LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Index: {index}");

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
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"* BindDeleteMember:");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- This: {this}");
        LambdaDebug.Print(LambdaDebug.MetaBindedColor, $"- Member: {binder.Name}");

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}