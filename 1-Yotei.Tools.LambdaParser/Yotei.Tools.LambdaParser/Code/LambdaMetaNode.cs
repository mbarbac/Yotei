#if !DEBUG_LAMBDAPARSER
#undef DEBUG
# endif

using static Yotei.Tools.Diagnostics.DebugWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools;

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
        Expression expression, BindingRestrictions restrictions, LambdaNode node)
        : base(expression, restrictions, node)
    {
        LambdaMetaMaster = master.ThrowWhenNull();
        LambdaId = Interlocked.Increment(ref _LastLambdaId);

        Indent(); WriteLine(Blue, $"- New: {this}");
        Unindent();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindSetMember(
        SetMemberBinder binder, DynamicMetaObject value)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        Indent();
        WriteLine(Green, $"* BindSetMember:");
        WriteLine($"- This: {this}");
        WriteLine($"- Name: {binder.Name}");
        var item = LambdaParser.ToLambdaNode(value, parser);
        WriteLine($"- Value: {item.ToDebugString()}");
        Unindent();

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
        Indent(); WriteLine(Yellow, $"- Result: {meta}");
        Unindent();
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="indexes"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        Indent();
        WriteLine(Green, $"* BindSetIndex:");
        WriteLine($"- This: {this}");
        var list = LambdaParser.ToLambdaNodes(indexes, parser);
        foreach (var temp in list) WriteLine($"- Index: {temp.ToDebugString()}");
        var item = LambdaParser.ToLambdaNode(value, parser);
        WriteLine($"- Value: {item.ToDebugString()}");
        Unindent();

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
        Indent(); WriteLine(Yellow, $"- Result: {meta}");
        Unindent();
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder, DynamicMetaObject arg)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        Indent();
        WriteLine(Green, $"* BindBinary:");
        WriteLine($"- This: {this}");
        WriteLine($"- Operation: {binder.Operation}");
        var item = LambdaParser.ToLambdaNode(arg, parser);
        WriteLine($"- Target: {item.ToDebugString()}");
        Unindent();

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
        Indent(); WriteLine(Yellow, $"- Result: {meta}");
        Unindent();
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        Indent();
        WriteLine(Green, $"* BindUnary:");
        WriteLine($"- This: {this}");
        WriteLine($"- Operation: {binder.Operation}");
        Unindent();

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
        Indent(); WriteLine(Yellow, $"- Result: {meta}");
        Unindent();
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="binder"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        var parser = LambdaNode.GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        Indent();
        WriteLine(Green, $"* BindConvert:");
        WriteLine($"- This: {this}");
        WriteLine($"- Type: {binder.Type.EasyName()}");
        Unindent();

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

        Indent(); WriteLine(Yellow, $"- Result: {meta}");
        Unindent();

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

    /// <summary>
    /// <inheritdoc/> DELEGATES OPERATION TO THE MASTER INSTANCE.
    /// </summary>
    /// <param name="binder"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindGetMember(
        GetMemberBinder binder)
    {
        Indent();
        WriteLine(Green, $"* BindGetMember:");
        WriteLine($"- This: {this}");
        Unindent();

        var meta = LambdaMetaMaster.BindGetMember(binder);
        return meta;
    }

    /// <summary>
    /// <inheritdoc/> DELEGATES OPERATION TO THE MASTER INSTANCE.
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="indexes"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindGetIndex(
        GetIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        Indent();
        WriteLine(Green, $"* BindGetIndex:");
        WriteLine($"- This: {this}");
        Unindent();

        var meta = LambdaMetaMaster.BindGetIndex(binder, indexes);
        return meta;
    }

    /// <summary>
    /// <inheritdoc/> DELEGATES OPERATION TO THE MASTER INSTANCE.
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindInvoke(
        InvokeBinder binder,
        DynamicMetaObject[] args)
    {
        Indent();
        WriteLine(Green, $"* BindInvoke:");
        WriteLine($"- This: {this}");
        Unindent();

        var meta = LambdaMetaMaster.BindInvoke(binder, args);
        return meta;
    }

    /// <summary>
    /// <inheritdoc/> DELEGATES OPERATION TO THE MASTER INSTANCE.
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindInvokeMember(
        InvokeMemberBinder binder,
        DynamicMetaObject[] args)
    {
        Indent();
        WriteLine(Green, $"* BindMethod:");
        WriteLine($"- This: {this}");
        Unindent();

        var meta = LambdaMetaMaster.BindInvokeMember(binder, args);
        return meta;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> OPERATION NOT SUPPORTED.
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindCreateInstance(
        CreateInstanceBinder binder,
        DynamicMetaObject[] args)
    {
        Indent();
        WriteLine(Green, $"* BindCreateInstance:");
        WriteLine($"- This: {this}");
        Unindent();

        throw new NotSupportedException(
            "'BindCreateInstance' operations are not supported.")
            .WithData(this);
    }

    /// <summary>
    /// <inheritdoc/> OPERATION NOT SUPPORTED.
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="indexes"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindDeleteIndex(
        DeleteIndexBinder binder,
        DynamicMetaObject[] indexes)
    {
        Indent();
        WriteLine(Green, $"* BindDeleteIndex:");
        WriteLine($"- This: {this}");
        Unindent();

        throw new NotSupportedException(
            "'BindDeleteIndex' operations are not supported.")
            .WithData(this);
    }

    /// <summary>
    /// <inheritdoc/> OPERATION NOT SUPPORTED.
    /// </summary>
    /// <param name="binder"></param>
    /// <returns></returns>
    public override DynamicMetaObject BindDeleteMember(
        DeleteMemberBinder binder)
    {
        Indent();
        WriteLine(Green, $"* BindDeleteMember:");
        WriteLine($"- This: {this}");
        Unindent();

        throw new NotSupportedException(
            "'BindDeleteMember' operations are not supported.")
            .WithData(this);
    }
}