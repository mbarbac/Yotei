
namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a node in a chain of dynamic operations.
/// </summary>
public abstract class LambdaNode : DynamicObject, ICloneable
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    public LambdaNode(LambdaParser parser)
    {
        LambdaParser = parser.ThrowWhenNull();
        LambdaId = NextLambdaId();
    }

    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract LambdaNode Clone();
    object ICloneable.Clone() => Clone();

    /// <inheritdoc/>
    public override string ToString() => nameof(LambdaNode);

    /// <summary>
    /// Returns a string representation of this instance suitable for debug purposes.
    /// </summary>
    /// <returns></returns>
    public string ToDebugString()
    {
        var type = GetType().EasyName();
        return $"[{type}](Id:{LambdaId}, {ToString()})";
    }

    // ----------------------------------------------------

    /// <summary>
    /// The parser this instance is associated with.
    /// </summary>
    public LambdaParser LambdaParser { get; }

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override DynamicMetaObject GetMetaObject(Expression expression)
    {
        var master = base.GetMetaObject(expression);
        var rest = BindingRestrictions.GetInstanceRestriction(expression, this);
        var meta = new LambdaMetaNode(master, expression, rest, this);

        return meta;
    }

    // ---------------------------------------------------- Overriden

    /// <inheritdoc/>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        throw null;
    }

    /// <inheritdoc/>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        throw null;
    }

    /// <inheritdoc/>
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        throw null;
    }

    /// <inheritdoc/>
    public override bool TryInvokeMember(
        InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        throw null;
    }

    // ---------------------------------------------------- Intercepted by meta node...

    /*public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result)*/
    /*public override bool TryConvert(ConvertBinder binder, out object? result)*/
    /*public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)*/
    /*public override bool TrySetMember(SetMemberBinder binder, object? value)*/
    /*public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)*/

    // ---------------------------------------------------- Not Supported...

    /*public override bool TryCreateInstance(CreateInstanceBinder binder, object?[]? args, [NotNullWhen(true)] out object? result)*/
    /*public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)*/
    /*public override bool TryDeleteMember(DeleteMemberBinder binder)*/
}