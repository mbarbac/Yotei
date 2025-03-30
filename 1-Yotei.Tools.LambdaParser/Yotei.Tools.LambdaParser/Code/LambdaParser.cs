namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lambda expressions
/// having dynamic arguments, returning the chain of dynamic operations bounded to them.
/// </summary>
public class LambdaParser
{
    // ----------------------------------------------------

    /// <summary>
    /// Returns a dynamic lambda node associated with the given value, which can either be an
    /// existing surrogate or an ad-hoc instance created for that value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal LambdaNode ToLambdaNode(object? value)
    {
        return value switch
        {
            LambdaNode item => item,
            LambdaMetaNode item => item.ValueNode,
            DynamicMetaObject item => ToLambdaNode(item.Value),

            _ => new LambdaNodeValue(this, value)
        };
    }

    /// <summary>
    /// Returns an array of dynamic lambda nodes, each associated with one of the given values,
    /// which can either be an existing surrogate or an ad-hoc instance created for that values.
    /// If the given array is null, then an empty one is returned.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    internal LambdaNode[] ToLambdaNodes(object?[]? values)
    {
        if (values == null) return [];

        var items = new LambdaNode[values.Length];
        for (int i = 0; i < values.Length; i++) items[i] = ToLambdaNode(values[i]);
        return items;
    }
}