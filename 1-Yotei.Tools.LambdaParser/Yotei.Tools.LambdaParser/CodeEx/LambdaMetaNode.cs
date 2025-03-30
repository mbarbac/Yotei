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

        LambdaHelpers.Print(LambdaHelpers.NewMetaColor, $"- New Meta: {this}");
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

    /// <inheritdoc/>
    public override DynamicMetaObject BindBinaryOperation(
        BinaryOperationBinder binder,
        DynamicMetaObject arg)
    {
        throw null;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        throw null;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
    {
        throw null;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
        throw null;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        throw null;
    }

    // ---------------------------------------------------- Delegated to underlying node...

    /// <inheritdoc/>
    public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindGetIndex:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        foreach (var index in indexes)
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Index: {index}");

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindGetIndex(binder, indexes);
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindGetMember:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Member: {binder.Name}");

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindGetMember(binder);
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindInvoke:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Argument: {arg}");

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindInvoke(binder, args);
        return meta;
    }

    /// <inheritdoc/>
    public override DynamicMetaObject BindInvokeMember(
        InvokeMemberBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindMethod:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Argument: {arg}");

        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Delegated...");
        var meta = LambdaMetaMaster.BindInvokeMember(binder, args);
        return meta;
    }

    // ---------------------------------------------------- Not supported

    /// <inheritdoc/>
    public override DynamicMetaObject BindCreateInstance(
        CreateInstanceBinder binder,
        DynamicMetaObject[] args)
    {
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"* META BindCreateInstance:");
        LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- This: {this}");
        foreach (var arg in args)
            LambdaHelpers.Print(LambdaHelpers.MetaBindedColor, $"- Argument: {arg}");

        throw new NotSupportedException(
            "'BindCreateInstance' operations are not supported.")
            .WithData(this);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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