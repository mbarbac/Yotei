namespace Yotei.Generators.Tree;

// ========================================================
internal static class Extensions
{
    /// <summary>
    /// Transforms the captured item into a type-alike captured one.
    /// </summary>
    /// <param name="captured"></param>
    /// <returns></returns>
    public static ICapturedType AsCapturedType(this ICaptured captured)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        return captured switch
        {
            ICapturedType item => item,
            ICapturedProperty item => item.CapturedType,
            ICapturedField item => item.CapturedType,

            _ => throw new UnreachableException($"Invalid captured element: {captured}")
        };
    }
}