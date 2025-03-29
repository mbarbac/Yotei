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
    public LambdaNode LambdaNode => Value is LambdaNode node
        ? node
        : throw new InvalidOperationException(
            "This meta object carries no valid dynamic lambda node.")
            .WithData(Value);
}