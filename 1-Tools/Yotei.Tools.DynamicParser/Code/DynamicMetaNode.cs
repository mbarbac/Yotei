namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of binding dynamic operations on its associated nodes.
/// </summary>
internal class DynamicMetaNode : DynamicMetaObject
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="master"></param>
    /// <param name="expression"></param>
    /// <param name="restrictions"></param>
    /// <param name="node"></param>
    public DynamicMetaNode(
        DynamicMetaObject master,
        Expression expression, BindingRestrictions restrictions, DynamicNode node)
        : base(expression, restrictions, node)
    {
        DynamicId = Interlocked.Increment(ref _LastDynamicId);
        DynamicMaster = master ?? throw new ArgumentNullException(nameof(master));
        DynamicParser.DebugPrint($"- New: {this}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    override public string ToString() => $"Meta#{DynamicId}:{DynamicNode.ToDebugString()}";

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public long DynamicId { get; }
    static long _LastDynamicId = 0;

    /// <summary>
    /// Returns the next available node id.
    /// </summary>
    /// <returns></returns>
    internal static long GetNextDynamicId() => Interlocked.Increment(ref _LastDynamicId);

    /// <summary>
    /// The default meta object associated with this instance.
    /// </summary>
    public DynamicMetaObject DynamicMaster { get; }

    /// <summary>
    /// The dynamic node carried by this instance.
    /// </summary>
    public DynamicNode DynamicNode => Value is DynamicNode node
        ? node
        : throw new InvalidOperationException(
            "This meta object carries no valid dynamic node.")
            .WithData(Value);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindGetMember(
        GetMemberBinder binder)
    {
        var meta = DynamicMaster.BindGetMember(binder); return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindSetMember(
        SetMemberBinder binder, DynamicMetaObject value)
    {
        DynamicParser.DebugPrint($"- SetMember:");
        DynamicParser.DebugPrint($"- This: {this}");
        DynamicParser.DebugPrint($"- Name: {binder.Name}");

        var item = DynamicParser.ValueToDynamicNode(value);
        DynamicParser.DebugPrint($"- Value: {item.ToDebugString()}");

        var member = new DynamicNodeMember(DynamicNode, binder.Name);
        var node = new DynamicNodeSetter(member, item);

        binder.FallbackSetMember(this, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new DynamicMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetMember(this, value, meta);
        DynamicParser.DebugPrint($"-- Result: {meta}");
        DynamicParser.DebugPrint();
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindGetIndex(
        GetIndexBinder binder, DynamicMetaObject[] indexes)
    {
        var meta = DynamicMaster.BindGetIndex(binder, indexes); return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        DynamicParser.DebugPrint($"- SetIndex:");
        DynamicParser.DebugPrint($"- This: {this}");

        var list = DynamicParser.ValuesToDynamicNodes(indexes);
        DynamicParser.DebugPrint(list.Select(x => $"- Index: {x.ToDebugString()}"));

        var item = DynamicParser.ValueToDynamicNode(value);
        DynamicParser.DebugPrint($"- Value: {item.ToDebugString()}");

        var member = new DynamicNodeIndexed(DynamicNode, list);
        var node = new DynamicNodeSetter(member, item);

        binder.FallbackSetIndex(this, indexes, value);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new DynamicMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackSetIndex(this, indexes, value, meta);
        DynamicParser.DebugPrint($"-- Result: {meta}");
        DynamicParser.DebugPrint();
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindInvoke(
        InvokeBinder binder, DynamicMetaObject[] args)
    {
        var meta = DynamicMaster.BindInvoke(binder, args); return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindInvokeMember(
        InvokeMemberBinder binder, DynamicMetaObject[] args)
    {
        var meta = DynamicMaster.BindInvokeMember(binder, args); return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder, DynamicMetaObject arg)
    {
        DynamicParser.DebugPrint($"- Binary:");
        DynamicParser.DebugPrint($"- This: {this}");
        DynamicParser.DebugPrint($"- Operation: {binder.Operation}");

        var item = DynamicParser.ValueToDynamicNode(arg);
        DynamicParser.DebugPrint($"- Target: {item.ToDebugString()}");

        var node = new DynamicNodeBinary(DynamicNode, binder.Operation, item);

        binder.FallbackBinaryOperation(this, arg);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);
        var meta = new DynamicMetaNode(
            new DynamicMetaObject(nodeExpr, rest, node),
            nodeExpr, rest, node);

        binder.FallbackBinaryOperation(this, arg, meta);
        DynamicParser.DebugPrint($"-- Result: {meta}");
        DynamicParser.DebugPrint();
        return meta;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindUnaryOperation(
        UnaryOperationBinder binder)
    {
        DynamicParser.DebugPrint($"- Unary:");
        DynamicParser.DebugPrint($"- This: {this}");
        DynamicParser.DebugPrint($"- Operation: {binder.Operation}");

        var node = new DynamicNodeUnary(binder.Operation, DynamicNode);

        binder.FallbackUnaryOperation(this);
        var updateExpr = binder.GetUpdateExpression(typeof(bool));

        var nodeExpr = Expression.Constant(node);
        var rest = node.GetBindingRestrictions(updateExpr);

        // Artifacts...
        if (binder.Operation == ExpressionType.IsTrue ||
            binder.Operation == ExpressionType.IsFalse)
        {
            var obj = false;
            var objExpr = Expression.Constant(obj);

            var meta = new DynamicMetaNode(
                new DynamicMetaObject(nodeExpr, rest, node),
                objExpr, rest, node);

            binder.FallbackUnaryOperation(this, meta);
            DynamicParser.DebugPrint($"-- Result: {meta}");
            DynamicParser.DebugPrint();
            return meta;
        }

        // Standard case...
        else
        {
            var meta = new DynamicMetaNode(
                new DynamicMetaObject(nodeExpr, rest, node),
                nodeExpr, rest, node);

            binder.FallbackUnaryOperation(this, meta);
            DynamicParser.DebugPrint($"-- Result: {meta}");
            DynamicParser.DebugPrint();
            return meta;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// This operation is not supported.
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindConvert(
        ConvertBinder binder)
    {
        throw new NotSupportedException(
            "Dynamic 'Convert' operations are not supported." +
            "Please use 'x.Cast(type, expr)' or 'x.Cast<Type>(expr)' instead.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// This operation is not supported.
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindCreateInstance(
        CreateInstanceBinder binder, DynamicMetaObject[] args)
    {
        throw new NotSupportedException(
            "Dynamic 'Create Instance' operations are not supported.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// This operation is not supported.
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindDeleteIndex(
        DeleteIndexBinder binder, DynamicMetaObject[] indexes)
    {
        throw new NotSupportedException(
            "Dynamic 'Delete Index' operations are not supported.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// This operation is not supported.
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicMetaObject BindDeleteMember(
        DeleteMemberBinder binder)
    {
        throw new NotSupportedException(
            "Dynamic 'Delete Member' operations are not supported.");
    }
}