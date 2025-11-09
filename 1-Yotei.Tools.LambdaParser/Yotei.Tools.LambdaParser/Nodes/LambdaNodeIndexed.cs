namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent an indexed get operation on a member in a chain of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class LambdaNodeIndexed : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public LambdaNodeIndexed(
        LambdaNode host,
        IEnumerable<LambdaNode> indexes) : base(host)
    {
        LambdaIndexes = LambdaParser.ValidateArguments(indexes, canBeEmpty: false);
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var str = $"{LambdaHost}[{string.Join(", ", LambdaIndexes.Select(static x => x.ToString()))}]";
        return str;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of indexes of the indexed get operation, which cannot be an empty one.
    /// </summary>
    public ImmutableArray<LambdaNode> LambdaIndexes { get; }
}