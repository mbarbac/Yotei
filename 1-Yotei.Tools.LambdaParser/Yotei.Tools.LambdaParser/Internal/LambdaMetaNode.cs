namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="DynamicMetaObject"/>. This type is an internal one not intended for public
/// usage.
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
        throw null;
    }
}